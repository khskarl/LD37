using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

	public bool _isOn = false;

	GameObject laserBeam;
	public Animation anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animation>();
		laserBeam = transform.GetChild(1).gameObject;
	}
	
	public void StartAnimation(string animationName, float speed)
	{
		//anim = GetComponent<Animation>();
		anim.wrapMode = WrapMode.Once;
		anim[animationName].speed = speed;
		if (speed < 0)
		{
			anim[animationName].time = anim[animationName].length;
		}
		anim.Play(animationName);
	}

	public void TurnOn ()
	{
		_isOn = true;
		laserBeam.SetActive(true);
	}

	public void TurnOff ()
	{
		_isOn = false;
		laserBeam.SetActive(false);
	}
}
