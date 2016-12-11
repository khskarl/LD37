﻿using UnityEngine;
using System.Collections;
using Input = TeamUtility.IO.InputManager;

[RequireComponent(typeof(ActorInput))]
[RequireComponent(typeof(Actor))]
public class ActorInput : MonoBehaviour {

	public Vector2 walkDirection = Vector2.zero;

	public bool jump = false;
	public bool run = false;
	public bool attack = false;
	public bool attackDown = false;
	public bool attackUp = false;
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
		// Eu: The input is inverted because I'm stupid
		float h = -Input.GetAxisRaw("Horizontal", _playerID);
		float v = -Input.GetAxisRaw("Vertical", _playerID);

		Vector2 walkDir = new Vector2(h, v);

		if (walkDir.SqrMagnitude() > 0.1f * 0.1f)
		{
			this.walkDirection = walkDir;
		}
		else
			this.walkDirection = new Vector2(0, 0);

		this.jump = Input.GetButton("Jump", _playerID);

		//this.run = Input.GetButton("Run", _playerID);

		if (this.attack == false && Input.GetButton("Attack", _playerID) == true)
		{
			timeAttackDown = Time.time;
			this.attackDown = true;
			this.attackUp   = false;
		}
		else if (this.attack == true && Input.GetButton("Attack", _playerID) == false)
		{
			this.attackUp   = true;
			this.attackDown = false;
		}
		else
		{
			this.attackUp   = false;
			this.attackDown = false;
		}

		this.attack = Input.GetButton("Attack", _playerID);
	}

	public float TimeSinceAttackDown()
	{
		return Time.time - timeAttackDown;
	}

}