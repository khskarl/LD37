using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum MatchState
{
	Lobby,
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
	Actor[] players = new Actor[0];
	int[] numLives = new int[0];
	Queue<int> deadList = new Queue<int>();

	int numPlayersAliveMatch;
	bool isMatchOver = false;


	/* FSM related */
	public MatchState state = MatchState.StartMatch;
	
	Dictionary<MatchState, Action> stateAction = new Dictionary<MatchState, Action>();

	float timeEnteredState = 0f;


	/* Minigames */
	public int minigameIndex = 0;
	public Minigame currMinigame;
	public Minigame[] minigames;
	bool allowRespawn = false;
	public int level = 1;
	public int numPlayersAlive;


	/* References */
	public Actor actorPrefab;

	public AudioSource music;
	public AudioSource shortCountdownSound;
	public AudioSource longCountdownSound;
	public AudioSource victoryTheme;

	public Transform titleScreen;

	/* */
	TextCopy stateGUIText;
	Text debugGUIText;
	Text levelGUIText;
	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad(gameObject);

		debugGUIText = GameObject.Find("DebugGameText").GetComponent<Text>();
		stateGUIText = GameObject.Find("StateText").GetComponent<TextCopy>();
		levelGUIText = GameObject.Find("LevelText").GetComponent<Text>();

		stateAction.Add(MatchState.Lobby, this.StateLobby);
		stateAction.Add(MatchState.StartMatch,    this.StartMatchState);
		stateAction.Add(MatchState.StartMinigame, this.StartMinigameState);
		stateAction.Add(MatchState.Minigame,      this.MinigameState);
		stateAction.Add(MatchState.EndMinigame,   this.EndMinigameState);
		stateAction.Add(MatchState.EndMatch,      this.EndMatchState);
		

	}

	void Start()
	{
		state = MatchState.Lobby;
		//BeginMatch();
	}

	void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			state = MatchState.Lobby;
			titleScreen.gameObject.SetActive(true);
		}

		if (state != MatchState.Lobby)
		{
			//DebugPlayersLives();
		}

		if (stateAction.ContainsKey(state))
			stateAction[state]();
	}

	public int GetMatchNumLives()
	{
		return settings.numLives;
	}

	public int GetMatchNumPlayers()
	{
		return settings.numPlayers;
	}

	public void IncreaseMatchNumLives()
	{
		settings.numLives += 1;
		GameObject.Find("lblNumLives").GetComponent<Text>().text = GetMatchNumLives().ToString();
	}

	public void DecreaseMatchNumLives()
	{
		if (settings.numLives > 1)
			settings.numLives -= 1;

		GameObject.Find("lblNumLives").GetComponent<Text>().text = GetMatchNumLives().ToString();
	}

	public void IncreaseMatchNumPlayers()
	{
		settings.numPlayers += 1;

		GameObject.Find("lblNumPlayers").GetComponent<Text>().text = GetMatchNumPlayers().ToString();
	}

	public void DecreaseMatchNumPlayers()
	{
		if (settings.numPlayers > 1)
			settings.numPlayers -= 1;

		GameObject.Find("lblNumPlayers").GetComponent<Text>().text = GetMatchNumPlayers().ToString();
	}

	public void BeginMatch()
	{
		titleScreen.gameObject.SetActive(false);
		SetupPlayers();
		numPlayersAlive = numPlayersAliveMatch;
		level = 1;
		SetLevelText(level.ToString());
		RoomManager.instance.RecoverAll();
		EnterStartMatch();
	}

	void EndMatch()
	{

	}

	/* State machine */
	void EnterLobby()
	{

	}
	void StateLobby()
	{
	}

	void EnterStartMatch()
	{
		
		numPlayersAlive = numPlayersAliveMatch;

		timeEnteredState = Time.time;
		state = MatchState.StartMatch;
		PlayLongCountdown();
	}
	void StartMatchState()
	{
		int timeToStartMatch = Mathf.CeilToInt(4 - (Time.time - timeEnteredState));
		SetStateText("Starting match in " + timeToStartMatch);

		currMinigame = minigames[minigameIndex].GetComponent<Minigame>();

		if (timeToStartMatch == 0)
		{
			EnterStartMinigame();
			PlayMusic();
		}
	}

	void EnterStartMinigame()
	{
		timeEnteredState = Time.time;
		state = MatchState.StartMinigame;
		PlayShortCountdown();
	}
	void StartMinigameState()
	{
		int timeLeft = Mathf.CeilToInt(2 - GetTimeInState());
		//SetStateText(currMinigame.name + '\n' + currMinigame.objective);
		SetStateText("");
		if (timeLeft == 0)
			EnterMinigame();
	}

	void EnterMinigame()
	{
		currMinigame.SetLevel(level);
		currMinigame.Begin();
		timeEnteredState = Time.time;
		state = MatchState.Minigame;
	}
	void MinigameState()
	{
		int timeLeft = Mathf.CeilToInt(15f - GetTimeInState());
		SetStateText(currMinigame.name + '\n' + currMinigame.objective + '\n' + timeLeft);

		currMinigame.Loop();

		if (allowRespawn)
		{
			RespawnDeadList();
		}
		if (timeLeft == 0 || currMinigame.HasEnded())
			EnterEndMinigame();
	}

	void EnterEndMinigame()
	{
		currMinigame.End();
		state = MatchState.EndMinigame;

		if (numPlayersAliveMatch <= 1 && settings.numPlayers > 1 || numPlayersAliveMatch == 0 && settings.numPlayers == 1)
		{
			EnterEndMatch();
		}

		RespawnDeadList();
		minigameIndex += 1;
		minigameIndex = minigameIndex % minigames.Length;
		if (minigameIndex == 0)
		{
			level += 1;
			SetLevelText(level.ToString());
		}
		currMinigame = minigames[minigameIndex].GetComponent<Minigame>();
		numPlayersAlive = numPlayersAliveMatch;

		timeEnteredState = Time.time;
	}
	void EndMinigameState()
	{
		int timeLeft = Mathf.CeilToInt(2f - (Time.time - timeEnteredState));
		SetStateText("Ending minigame");

		if (timeLeft == 0)
		{
			EnterStartMinigame();
		}
	}

	public int GetLevel()
	{
		return level;
	}

	void EnterEndMatch()
	{
		timeEnteredState = Time.time;
		state = MatchState.EndMatch;
		StopMusic();
		PlayVictoryTheme();
	}

	void EndMatchState()
	{
		Actor winner = GetWinner();
		Debug.Log(winner.name + " wins!");
		SetStateText("Match is over :D\n" + winner.name + " wins!");
	}

	/* Utils */

	public void PlayLongCountdown()
	{
		longCountdownSound.Play();
	}

	public void PlayVictoryTheme()
	{
		victoryTheme.Play();
	}

	public void PlayShortCountdown()
	{
		shortCountdownSound.Play();
	}

	public void PlayMusic()
	{
		music.Play();
	}

	public void StopMusic()
	{
		music.Stop();
	}

	public void Delete(List<GameObject> gos)
	{
		foreach (GameObject go in gos)
		{
			GameObject.Destroy(go);
		}
	} 
		

	void SetupPlayers()
	{
		if (players.Length != 0)
		{
			foreach (Actor player in players)
			{
				Kill(player);
			}
		}
		deadList.Clear();

		players = new Actor[settings.numPlayers];
		numLives = new int[settings.numPlayers];

		for (int i = 0; i < settings.numPlayers; i++)
		{
			Actor actor = Instantiate(actorPrefab, new Vector3((i % 4)* 2 - 4, 10 * (int)(i / 4 + 1), 0), Quaternion.identity) as Actor;
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
			numLives[i] = settings.numLives;

		numPlayersAliveMatch = settings.numPlayers;
	}
			
	public void Kill(Actor target)
	{
		target.PlayDeathSound();
		RemoveLife(target);
		target.transform.position = new Vector3(1000, 1000, 1000); // Send them to hell
		deadList.Enqueue(target.id);
		numPlayersAlive -= 1;

	}

	public void RemoveLife(Actor target)
	{
		if (numLives[target.id] <= 0)
			return;

		numLives[target.id] -= 1;

		if (numLives[target.id] <= 0)
			numPlayersAliveMatch -= 1;
		
	}

	public void RespawnDeadList()
	{
		int[] deadIDs = deadList.ToArray();
		foreach (int id in deadIDs)
		{
			if (GetNumLives(id) <= 0)
				continue;
			
			players[id].transform.position = new Vector3(id, 10, -6);
			players[id].movement.Stop();
		}
		deadList.Clear();
	}

	public void Respawn(Actor target)
	{
		target.movement.Stop();
		target.transform.position = new Vector3(0 , 10, 0);
	}

	Actor GetWinner()
	{
		Actor winner = new Actor();
		
		for (int i = 0; i < settings.numPlayers; i++)
		{
			if (numLives[i] > 0)
			{
				winner = players[i];
			}
		}

		return winner;
	}

	public int GetNumLives(int id)
	{
		if (numLives.Length <= id || id < 0)
		{
			return -1;
		}
		return numLives[id];
	}

	public float GetTimeInState()
	{
		return Time.time - timeEnteredState;
	}

	/* Debug Stuff */
	private void SetDebugText(string text)
	{
		debugGUIText.text = text;
	}

	private void SetStateText(string text)
	{
		//stateGUIText.text = text;
		stateGUIText.SetText(text);
	}

	private void SetLevelText(string text)
	{
		levelGUIText.text = "Level " + text;
	}

	private void DebugPlayersLives()
	{
		string debugText = "";
		debugText += "Match Alive: " + numPlayersAliveMatch + '\n';
		debugText += "Minigame Alive: " + numPlayersAlive + '\n';

		for (int i = 0; i < settings.numPlayers; i++)
		{
			debugText += "P" + (i + 1) + ": " + numLives[i] + '\n';
		}

		SetDebugText(debugText);
	}
}
