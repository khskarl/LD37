using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorCombat : MonoBehaviour {


	/* References */
	Actor _actor;
	ActorMovement _movement;

	public Transform hitEffectPrefab;

	// Use this for initialization
	void Start ()
	{
		_actor =    this.GetComponent<Actor>();
		_movement = this.GetComponent<ActorMovement>();
	}
		
	void HandleTackle(Actor source, Actor target)
	{
		Vector3 posMedian = (source.transform.position + target.transform.position) / 2f;
		Quaternion direction = Quaternion.identity;
		if (source.movement._launchVelocity != Vector3.zero)
		{
			direction = Quaternion.LookRotation(source.movement._launchVelocity);
		}
		if (source.state == State.Attack && target.state != State.Attack)
		{
			// TODO: Change sound accordingly to the velocity
			_actor.hitSound.Play();
			InstantiateHitEffect(posMedian, direction);

			Vector3 sourceVelocity = source.movement._launchVelocity;
			target.combat.ReceiveTackle(source, sourceVelocity * 2f);

			Vector3 inverseVelocity = sourceVelocity * -0.5f ;
			source.movement.LaunchOverride(inverseVelocity);
		}
		else if (source.state == State.Attack && target.state == State.Attack)
		{
			// TODO: Change sound accordingly to the velocity
			_actor.hitSound.Play();
			InstantiateHitEffect(posMedian, direction);
			Vector3 sourceVelocity = source.movement._launchVelocity;
			//Vector3 targetVelocity = target.movement._launchVelocity;
			Vector3 pushbackVelocity = sourceVelocity * -0.5f;
			source.movement.LaunchOverride(pushbackVelocity);
		}
	}

	void ReceiveTackle(Actor source, Vector3 force)
	{
		_actor.EnterHurt();
		_movement.Launch(force);
	}
	
	void InstantiateHitEffect(Vector3 hitPos, Quaternion direction)
	{
		GameObject.Instantiate(hitEffectPrefab, hitPos, direction);
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Actor hitActor = hit.collider.transform.GetComponent<Actor>();

		if (hitActor)
		{
			HandleTackle(_actor, hitActor);
		}
	}
}
