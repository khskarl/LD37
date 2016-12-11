using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Actor))]
[RequireComponent(typeof(CharacterController))]
public class ActorMovement : MonoBehaviour {
	/*-----------------*/
	/* Global Internal */
	/*-----------------*/
	float _dt;
	public bool _isGrounded = false;
	public bool _isWalking;
	public bool _isRunning  = false;
	
	Vector3 _targetLookDir;
	Vector3 _lookDir;

	Vector3 _groundNormal = Vector3.up;

	Vector3 _forward = Vector3.forward;
	Vector3 _right 	 = Vector3.right;	
		
	/*---------------------*/
	/* Movement Parameters */
	/*---------------------*/
	//
	public bool isAffectedByGravity = true;


	// Walking (1f = 1m)
	public float _currSpeed = 0f;
	public float walkSpeed 	= 4f;	
	public float runSpeed 	= 8f;
	public float walkDamp 	= 0.05f;

	// Turning
	public float turnSpeed = 8f;
	
	/*----------------------*/
	/* Velocity Motion Data */
	/*----------------------*/
	public Vector3 _walkVelocity;
	public Vector3 _targetWalkVelocity;
	public Vector3 _gravityVelocity = Vector3.zero;
	public Vector3 _launchVelocity = Vector3.zero;
	
	public Vector3 _motion = Vector3.zero;

	Vector3 _walkAcceleration = Vector3.zero;

	/*--------*/
	/* Timers */
	/*--------*/
	public float _timeLand;
	public float _timeAir;

	/*------------*/
	/* References */
	/*------------*/
	Actor _actor;
	CharacterController _controller;

	void Start () 
	{
		#region Find references 
		_controller = GetComponent<CharacterController>();
		_actor = GetComponent<Actor>();
		#endregion

		#region Set Initial Values
		
		_lookDir 	   = transform.forward;
		_targetLookDir = transform.forward;
		#endregion
	}

	void Update () {
	}

	void FixedUpdate () {
		// Initial computations
		_dt = Time.fixedDeltaTime;
						
		// Turn character towards walking direction smoothly
		TurnTowardsWalk();

		ComputeWalkingVelocity();

		if (isAffectedByGravity)
			ComputeGravityVelocity();
	
		ApplyMotion();

		// Computations to prepare for next frame

		if (_isGrounded) {
			_gravityVelocity = Vector3.zero;
			_launchVelocity.y = 0;
		}	

		// Apply friction to LaunchVelocity
		ApplyFrictionToLaunchVelocity();

		// Make sure it won't keep walking without input
		_targetWalkVelocity = Vector3.zero;

		// Just pass values
		_currSpeed = _walkVelocity.magnitude;


		// Is character grounded?
		ComputeIsGrounded();
	}


	void ComputeIsGrounded () {
		bool wasGrounded = _isGrounded;
		bool isGrounded  = _controller.isGrounded;

		if (wasGrounded) {
			if(isGrounded == false){
				_timeLand = Time.time;
			}
		} else {
			if(isGrounded == true){
				_timeAir = Time.time;
			}
		}

		_isGrounded = isGrounded;
	}

	void TurnTowardsWalk () {
		_lookDir = Vector3.Slerp(_lookDir, _targetLookDir, turnSpeed * _dt);

		transform.rotation = Quaternion.LookRotation(_lookDir);		
	}

	void ApplyFrictionToLaunchVelocity () {
		float friction = 0.50f; // 0.5f = Airfriction

		if (_isGrounded == true) 
		{
			friction = 0.05f;
		} 

		Vector3 launchAccel = Vector3.zero;
		_launchVelocity = Vector3.SmoothDamp(_launchVelocity, 
		                                     Vector3.zero, 
		                                     ref launchAccel, 
		                                     friction, 
		                                     150, 
		                                     _dt);
	}

	void ComputeWalkingVelocity () {
		float friction = walkDamp; // 0.5f = Airfriction

		if (_isGrounded == false)
		{
			friction *= 8;
		} 
		// Update walking velocity
		_walkVelocity = Vector3.SmoothDamp(_walkVelocity, 
										   _targetWalkVelocity, 
										   ref _walkAcceleration,
										   friction,
										   150,
										   _dt);
		
	}
	

	void ComputeGravityVelocity () {
		// Update gravity velocity
		_gravityVelocity += Physics.gravity * 0.06f;
	}

	void ApplyMotion () {
		// Apply all velocity to final motion
		_motion = _walkVelocity + _gravityVelocity + _launchVelocity;

		// Apply Motion
		_controller.Move(_motion * _dt);
	}

	/* Messy non intuitive function, gotta clean up */
	public void Walk (Vector2 axis, bool shouldRun) {
		// Update direction and check if character
		float magnitude = axis.magnitude;
		axis.Normalize();

		_isRunning = shouldRun;
		_isWalking = CheckIsWalking(magnitude);

		if(_isWalking)
		{
			_targetLookDir = (axis.y * _forward + axis.x * _right);
		}

		//

		float targetSpeed;

		if(_isRunning)
		{
			targetSpeed = runSpeed;
		}
		else
		{
			targetSpeed = walkSpeed;
		}

		targetSpeed = Mathf.Lerp(0, targetSpeed, magnitude);

		_targetWalkVelocity = targetSpeed * _targetLookDir;
	
	}

	public void Stop()
	{
		_walkVelocity    = Vector3.zero;
		_gravityVelocity = Vector3.zero;
		_launchVelocity  = Vector3.zero;
	}

	public void SetLookDirection(Vector3 direction)
	{
		if (direction.sqrMagnitude < 0.1f * 0.1f)
			return;

		_targetLookDir = (direction.y * _forward + direction.x * _right).normalized;
	}

	public void Jump ()
	{
		_gravityVelocity.y = 10;
		_isGrounded = false;
	}

	public void Tackle (float force)
	{
		_gravityVelocity.y = 5;
		_launchVelocity = _targetLookDir * force;
		_isGrounded = false;
	}


	public void Launch (Vector3 velocity) {
		_launchVelocity += velocity;
	}

	public void LaunchOverride (Vector3 velocity) {
		_launchVelocity = velocity;
	}

	bool CheckIsWalking (float magnitude) {
		float tolerance = 0.05f;
		return Mathf.Abs(magnitude) > tolerance;
	}
	
	void ComputeGroundAxis () {
		_forward = Vector3.ProjectOnPlane(_forward, _groundNormal).normalized;
		_right	 = Vector3.ProjectOnPlane(_right, 	_groundNormal).normalized;
	}
}
