using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum MatchState
{
	StartMatch,
	StartMinigame,
	Minigame,
	EndMinigame,
	EndMatch
}

struct MatchSettings
{
	public MatchSettings(int argNumPlayers, int argNumLives)
	{
		numPlayers = argNumPlayers;
		numLives = argNumLives;
	}

	public int numPlayers;
	public int numLives;
}

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
	
	
	MatchSettings settings = new MatchSettings(4, 5);

	/* Match variables */
	Actor[] players;
	int[] lives;

	int numPlayersAlive;
	bool isMatchOver = false;


	/* FSM related */
	public MatchState state = MatchState.StartMatch;
	
	Dictionary<MatchState, Action> stateAction = new Dictionary<MatchState, Action>();

	float timeEnteredState = 0f;


	/* Minigames */
	public int minigameIndex = 0;
	public Minigame currMinigame;
	public Minigame[] minigames;

	/* References */
	public Actor actorPrefab;

	/* */
	Text stateGUIText;
	Text debugGUIText;
	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad(gameObject);
				
		stateAction.Add(MatchState.StartMatch,    this.StartMatchState);
		stateAction.Add(MatchState.StartMinigame, this.StartMinigameState);
		stateAction.Add(MatchState.Minigame,      this.MinigameState);
		stateAction.Add(MatchState.EndMinigame,   this.EndMinigameState);
		stateAction.Add(MatchState.EndMatch,      this.EndMatchState);
		
		debugGUIText = GameObject.Find("DebugGameText").GetComponent<Text>();
		stateGUIText = GameObject.Find("StateText").GetComponent<Text>();

	}

	void Start()
	{
		EnterStartMinigame();
		BeginMatch();
	}

	void FixedUpdate()
	{
		DebugPlayersLives();

		if (stateAction.ContainsKey(state))
			stateAction[state]();
	}

	void BeginMatch()
	{
		SetupPlayers();
	}

	void EndMatch()
	{

	}

	/* State machine */
	void EnterStartMatch()
	{
		timeEnteredState = Time.time;
		state = MatchState.StartMatch;
	}
	void StartMatchState()
	{
		int timeToStartMatch = Mathf.CeilToInt(5 - (Time.time - timeEnteredState)); 
		SetStateText("Starting match in " + timeToStartMatch);

		if (timeToStartMatch == 0)
			EnterStartMinigame();
	}

	void EnterStartMinigame()
	{
		currMinigame = minigames[minigameIndex].GetComponent<Minigame>();
		timeEnteredState = Time.time;
		state = MatchState.StartMinigame;
	}
	void StartMinigameState()
	{
		int timeLeft = Mathf.CeilToInt(2 - (Time.time - timeEnteredState));
		SetStateText("Starting minigame " + currMinigame.name + " in " + timeLeft);

		if (timeLeft == 0)
			EnterMinigame();
	}

	void EnterMinigame()
	{
		currMinigame.Begin();
		timeEnteredState = Time.time;
		state = MatchState.Minigame;
	}
	void MinigameState()
	{
		int timeLeft = Mathf.CeilToInt(13.5f - (Time.time - timeEnteredState));
		SetStateText("Minigame! " + currMinigame.name + ": " + timeLeft);

		if (timeLeft == 0)
			EnterEndMinigame();
	}

	void EnterEndMinigame()
	{
		currMinigame.End();
		minigameIndex += 1;
		minigameIndex = minigameIndex % minigames.Length;

		timeEnteredState = Time.time;
		state = MatchState.EndMinigame;
	}
	void EndMinigameState()
	{
		int timeLeft = Mathf.CeilToInt(2f - (Time.time - timeEnteredState));
		SetStateText("Ending minigame");

		if (numPlayersAlive <= 1)
		{
			EnterEndMatch();
		}
		else if (timeLeft == 0)
		{
			EnterStartMinigame();
		}
	}

	void EnterEndMatch()
	{
		timeEnteredState = Time.time;
		state = MatchState.EndMatch;
	}
	void EndMatchState()
	{
		Actor winner = GetWinner();
		Debug.Log(winner.name + " wins!");
		SetStateText("Match is over :D\n" + winner.name + " wins!");
	}

	/* Utils */

	void NextMinigame()
	{

	}

	void SetupPlayers()
	{
		players = new Actor[settings.numPlayers];
		lives = new int[settings.numPlayers];

		for (int i = 0; i < settings.numPlayers; i++)
		{
			Actor actor = Instantiate(actorPrefab, new Vector3(i * 2 - 4, 10, 0), Quaternion.identity) as Actor;
			actor.id = i;
			actor.input._playerID = (TeamUtility.IO.PlayerID)i;
			actor.name = "Player " + (i + 1);

			players[i] = actor;
		}

		ResetLives();
	}

	void ResetLives()
	{
		for (int i = 0; i < settings.numPlayers; i++)
			lives[i] = settings.numLives;

		numPlayersAlive = settings.numPlayers;
	}
			
	public void Kill(Actor target)
	{
		RemoveLife(target);

		if (lives[target.id] > 0)
			Respawn(target);

	}

	public void RemoveLife(Actor target)
	{
		if (lives[target.id] <= 0)
			return;

		lives[target.id] -= 1;

		if (lives[target.id] <= 0)
			numPlayersAlive -= 1;

		Debug.Log(numPlayersAlive + " players left!");

		if (numPlayersAlive == 1)
			EnterEndMatch();
	}

	public void Respawn(Actor target)
	{
		target.movement.Stop();
		target.transform.position = new Vector3(0, 10, 0);
	}

	Actor GetWinner()
	{
		Actor winner = new Actor();
		
		for (int i = 0; i < settings.numPlayers; i++)
		{
			if (lives[i] > 0)
			{
				winner = players[i];
			}
		}

		return winner;
	}

	/* Debug Stuff */
	private void SetDebugText(string text)
	{
		debugGUIText.text = text;
	}

	private void SetStateText(string text)
	{
		stateGUIText.text = text;
	}

	private void DebugPlayersLives()
	{
		string debugText = "";
		debugText += "Alive: " + numPlayersAlive + '\n';

		for (int i = 0; i < settings.numPlayers; i++)
		{
			debugText += "P" + (i + 1) + ": " + lives[i] + '\n';
		}

		SetDebugText(debugText);
	}
}
