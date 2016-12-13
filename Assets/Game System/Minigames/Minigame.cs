using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Minigame : MonoBehaviour {

	public string name = "UNDEFINED";
	public string objective = "UNDEFINED";

	//float duration = 15f;
	public int level = 1;

	abstract public void Begin();

	abstract public void End();

	abstract public bool HasEnded();


	virtual public void Loop()
	{

	}

	public void SetLevel(int l)
	{
		level = l;
	}

	public int GetLevel()
	{
		return level;
	}


	virtual public void GiveLife(Actor target)
	{

	}


	virtual public void TakeLife(Actor target)
	{

	}

}
