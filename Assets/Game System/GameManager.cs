using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
	
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
	MatchSettings settings = new MatchSettings(4, 5);

	/* Match variables */
	Actor[] players;
	Dictionary<Actor, int> lives;
	int numPlayersAlive;

	/* References */
	public Actor actorPrefab;

	/* */
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

		debugGUIText = GameObject.Find("DebugGame").GetComponent<Text>();

		BeginMatch();
	}

	void Start()
	{
		BeginMatch();
	}

	void Update()
	{
		string debugText = "";
		debugText += "Num Players: " + settings.numPlayers + '\n';
		debugText += "Alive: " + numPlayersAlive + '\n';

		int p = 1;
		foreach(var pair in lives)
		{
			debugText += "P" + p + ": " + pair.Value + '\n';
			p += 1;
		}

		SetDebugText(debugText);


	}

	void BeginMatch()
	{
		CreatePlayers();
	}

	void EndMatch()
	{
		Actor winner = GetWinner();
		SetDebugText(winner.name + " wins!");
	}

	void CreatePlayers()
	{
		players = new Actor[4];
		lives = new Dictionary<Actor, int>();

		for (int i = 0; i < settings.numPlayers; i++)
		{
			Actor actor = Instantiate(actorPrefab, new Vector3(i * 2 - 4, 10, 0), Quaternion.identity) as Actor;
			actor.input._playerID = (TeamUtility.IO.PlayerID)i;
			actor.name = "Player " + (i + 1);
			players[i] = actor;
			lives.Add(actor, settings.numLives);
		}

		numPlayersAlive = settings.numPlayers;
	}
			
	public void Kill(Actor target)
	{
		RemoveLife(target);

		if (lives[target] > 0)
			Respawn(target);

	}

	public void RemoveLife(Actor target)
	{
		if (lives[target] <= 0)
			return;

		lives[target] -= 1;

		if (lives[target] <= 0)
		{
			numPlayersAlive -= 1;
			
		}

		if (numPlayersAlive == 1)
		{
			EndMatch();
		}
	}

	public void Respawn(Actor target)
	{
		target.movement.Stop();
		target.transform.position = new Vector3(0, 10, 0);
	}

	Actor GetWinner()
	{
		Actor winner = new Actor();

		foreach (var pair in lives)
		{
			if (pair.Value > 0)
			{
				winner = pair.Key;
			}
		}

		return winner;
	}

	/* Debug Stuff */
	private void SetDebugText(string text)
	{
		debugGUIText.text = text;
	}
}
