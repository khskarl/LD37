using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using Input = TeamUtility.IO.InputManager;

[RequireComponent(typeof(ActorInput))]
[RequireComponent(typeof(Actor))]
public class ActorInput : NetworkBehaviour {


	[SyncVar]
	public Vector2 walkDirection = Vector2.zero;

	[SyncVar]
	public bool jump = false;
	[SyncVar]
	public bool run = false;
	[SyncVar]
	public bool attack = false;
	[SyncVar]
	public bool attackDown = false;
	[SyncVar]
	public bool attackUp = false;
	[SyncVar]
	public float timeAttackDown = 0;

	// Defines the player input configuration ID this ActorControlPlayer uses
	//static int numPlayers = 0;
	public TeamUtility.IO.PlayerID _playerID;
	
	// Use this for initialization
	void Start()
	{
		//_playerID = (TeamUtility.IO.PlayerID)(numPlayers % 4);
		//numPlayers += 1;
	}

	// Update is called once per frame
	void Update()
	{
		if (isLocalPlayer == false)
			return;

		float h = -Input.GetAxisRaw("Horizontal", GetPlayerID());
		float v = -Input.GetAxisRaw("Vertical", GetPlayerID());

		Vector2 walkDir = new Vector2(h, v);

		if (walkDir.SqrMagnitude() > 0.1f * 0.1f)
		{
			this.walkDirection = walkDir;
		}
		else
			this.walkDirection = new Vector2(0, 0);

		this.jump = Input.GetButton("Jump", GetPlayerID());

		//this.run = Input.GetButton("Run", GetPlayerID());

		if (this.attack == false && Input.GetButton("Attack", GetPlayerID()) == true)
		{
			timeAttackDown = Time.time;
			this.attackDown = true;
			this.attackUp   = false;
		}
		else if (this.attack == true && Input.GetButton("Attack", GetPlayerID()) == false)
		{
			this.attackUp   = true;
			this.attackDown = false;
		}
		else
		{
			this.attackUp   = false;
			this.attackDown = false;
		}

		this.attack = Input.GetButton("Attack", GetPlayerID());
	}

	TeamUtility.IO.PlayerID GetPlayerID()
	{
		return (TeamUtility.IO.PlayerID)((int)_playerID % 4);
	}

	public float TimeSinceAttackDown()
	{
		return Time.time - timeAttackDown;
	}

}
