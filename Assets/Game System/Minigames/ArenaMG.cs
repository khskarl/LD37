using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaMG : Minigame {
			
	ArenaMG()
	{
		name = "Arena";
		objective = "Defeat the others!";
	}

	override public void Begin()
	{
		RoomManager.instance.OuterFall(1);
	}

	override public void End()
	{
		RoomManager.instance.OuterRecover(1);
	}

	override public bool HasEnded()
	{
		return GameManager.instance.numPlayersAlive <= 1;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
