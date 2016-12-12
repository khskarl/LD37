using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// In case we need/want a team and friendly fire system
enum TeamColor
{
	NONE,
	RED,
	GREEN,
	BLUE,
	PURPLE
};

public enum State
{
	Idle,
	Walk,
	Charge,
	Attack,
	Jump,
	Hurt
}

public class Actor : MonoBehaviour {
	/*  */
	public int id;
	public string name = "UNDEFINED";
	TeamColor teamColor = TeamColor.NONE; // Eu: Not being used at the moment

	public State state = State.Idle;

	/*  */
	Dictionary<State, Action> stateAction = new Dictionary<State, Action>();
	
	/* References */
	public ActorMovement 	movement;
	public ActorInput		input;
	public ActorCombat		combat;

	Animation anim;
	
	/* Debug Stuff */
	TextMesh debugTextMesh;
	
	void Awake () {
		movement =	GetComponent<ActorMovement>();
		input =		GetComponent<ActorInput>();
		combat =	GetComponent<ActorCombat>();

		anim = GetComponent<Animation>();
		debugTextMesh = GetComponentInChildren<TextMesh>();
	}

	void Start ()
	{
		/* */
		stateAction.Add(State.Idle,   this.Idle);
		stateAction.Add(State.Walk,   this.Walk);
		stateAction.Add(State.Charge, this.Charge);
		stateAction.Add(State.Attack, this.Attack);
		stateAction.Add(State.Jump,   this.Jump);
		stateAction.Add(State.Hurt,   this.Hurt);
	}
	
	// Update is called once per frame
	void Update () {

		StateMachineLoop();

		if (debugTextMesh)
			DebugState();
		
	}

	void StateMachineLoop()
	{
		if (stateAction.ContainsKey(state))
			stateAction[state]();		
	}

	void Idle()
	{

		anim.Play("chicken_idle");

		float speed = input.walkDirection.SqrMagnitude();
		if (speed > 0.01f)
			state = State.Walk;
		
		movement.Walk(input.walkDirection, input.run);

		if (input.jump)
			EnterJump();
		
		if (input.attack && input.TimeSinceAttackDown() > 0.05f)
		{
			state = State.Charge;
		}
		else if (input.attackUp)
		{
			movement.Tackle(5);
			state = State.Attack;
		}
	}

	void Walk()
	{
		anim.Play("chicken_walk");

		float speed = input.walkDirection.SqrMagnitude();
		if (speed <= 0.01f)
		{
			state = State.Idle;
		}
		movement.Walk(input.walkDirection, input.run);

		if (input.jump)
			EnterJump();

		if (input.attack && input.TimeSinceAttackDown() > 0.1f)
		{
			EnterCharge();
		}
		else if (input.attackUp && input.TimeSinceAttackDown() <= 0.1f)
		{
			movement.Tackle(5);
			state = State.Attack;
		}
	}

	void EnterCharge()
	{
		state = State.Charge;
	}

	void Charge()
	{
		anim.Play("chicken_charge");
			
		movement.SetLookDirection(input.walkDirection);
		
		if (input.attackUp || input.attack == false)
		{
			float timeCharged = Mathf.Clamp(input.TimeSinceAttackDown(), 0, 2);
			float timeChargedNormalized = timeCharged / 2;
			float force = Mathf.Lerp(5, 20, timeChargedNormalized);

			//Debug.Log(timeCharged);
			movement.Tackle(force);
			state = State.Attack;
		}
	}

	void Attack()
	{
		if (movement._isGrounded == true /* or when actor hits another actor */ )
		{
			state = State.Idle;
		}

	}

	void EnterJump()
	{
		movement.Jump();
		state = State.Jump;
	}

	void Jump()
	{
		movement.Walk(input.walkDirection, false);
		if (movement._isGrounded == true)
		{
			state = State.Idle;
		}
	}

	public void EnterHurt()
	{
		anim.Play("chicken_bounce");
		anim.Blend("chicken_idle", 1, 0.3f);
		state = State.Hurt;
	}

	void Hurt()
	{
		if (anim.IsPlaying("chicken_bounce") == false)
			state = State.Idle;
	}

	/* Debug stuff */
	void DebugState()
	{
		debugTextMesh.text = state.ToString();
	}

}
