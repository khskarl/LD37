using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

	/* Match Settings */
	int numPlayers = 4;
	int numLives   = 5;

	/* Match variables */
	Actor[] players;
	Dictionary<Actor, int> lives;

	/* References */
	public Actor actorPrefab;

	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		
		DontDestroyOnLoad(gameObject);

		InitGame();
	}

	void Start()
	{
		SpawnPlayers();
	}

	void SpawnPlayers()
	{
		players = new Actor[4];
		lives = new Dictionary<Actor, int>();

		for (int i = 0; i < numPlayers; i++)
		{
			Actor actor = Instantiate(actorPrefab, new Vector3(i * 2 - 4, 10, 0), Quaternion.identity) as Actor;
			actor.input._playerID = (TeamUtility.IO.PlayerID)i;
			players[i] = actor;
			lives.Add(actor, numLives);
		}
	}

	void InitGame()
	{

	}
		
	public void Kill(Actor target)
	{
		target.movement.Stop();
		target.transform.position = new Vector3(0, 10, 0);
	}
}
