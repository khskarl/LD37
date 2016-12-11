using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Kill (Actor target)
	{
		GameManager.instance.Kill(target);
	}

	void OnTriggerEnter(Collider other)
	{
		Actor actor = other.transform.GetComponent<Actor>();

		if (actor)
			this.Kill(actor);
	}
}
