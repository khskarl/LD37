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


	public AudioSource hurtSound;
	public AudioSource hitSound;
	public AudioSource deathSound;

	/* Debug Stuff */
	TextMesh debugTextMesh;
	TextMesh lifeCounterTextMesh;

	void Awake () {
		movement =	GetComponent<ActorMovement>();
		input =		GetComponent<ActorInput>();
		combat =	GetComponent<ActorCombat>();

		anim = GetComponent<Animation>();

		TextMesh[] textMeshes = GetComponentsInChildren<TextMesh>();
		foreach (TextMesh textMesh in textMeshes)
		{
			if (textMesh.transform.name == "DebugText")
				debugTextMesh = textMesh;

			if (textMesh.transform.name == "LifeCounter")
				lifeCounterTextMesh = textMesh;
		}

	}

	void Start ()
	{
		/* */
		stateAction.Add(State.Idle,   this.Idle);
		stateAction.Add(State.Walk,   this.StateWalk);
		stateAction.Add(State.Charge, this.StateCharge);
		stateAction.Add(State.Attack, this.StateAttack);
		stateAction.Add(State.Jump,   this.StateJump);
		stateAction.Add(State.Hurt,   this.StateHurt);
	}
	
	// Update is called once per frame
	void Update () {

		StateMachineLoop();
		UpdateLifeCounter();

		if (debugTextMesh)
			DebugState();
		
	}

	public void PlayDeathSound()
	{
		deathSound.Play();
	}

	/* State Machine Stuff */

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

	void StateWalk()
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

	void StateCharge()
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

	void StateAttack()
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

	void StateJump()
	{
		movement.Walk(input.walkDirection, false);
		if (movement._isGrounded == true)
		{
			state = State.Idle;
		}

		if (input.attack && input.TimeSinceAttackDown() > 0.1f)
		{
			EnterCharge();
		}
	}

	public void EnterHurt()
	{
		hurtSound.Play();
		anim.Play("chicken_bounce");
		anim.Blend("chicken_idle", 1, 0.3f);
		state = State.Hurt;
	}

	void StateHurt()
	{
		if (anim.IsPlaying("chicken_bounce") == false)
			state = State.Idle;
	}

		/* Debug stuff */
	void DebugState()
	{
		if (debugTextMesh)
		{
			debugTextMesh.text = state.ToString();
		}
	}

	void UpdateLifeCounter()
	{
		lifeCounterTextMesh.text = "x ";
		lifeCounterTextMesh.text += GameManager.instance.GetNumLives(id).ToString();
	}

}
