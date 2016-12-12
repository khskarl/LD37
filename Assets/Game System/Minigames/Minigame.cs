using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour {

	public string name = "UNDEFINED";
	public string objective = "UNDEFINED";

	float duration = 13.5f;
	float level = 1;
	
	virtual public void Begin()
	{

	}

	virtual public void End()
	{

	}
	
	virtual public void HasEnded()
	{

	}

	virtual public void GiveLife(Actor target)
	{

	}


	virtual public void TakeLife(Actor target)
	{

	}

}
