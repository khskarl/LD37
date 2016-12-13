using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTilesMG : Minigame
{
	Queue<Platform> platformsFalling = new Queue<Platform>();

	FallingTilesMG()
	{
		name = "Falling Tiles";
		objective = "Don't fall!";
	}


	override public void Begin()
	{
		FallRandomPlatform();
		FallRandomPlatform();
		FallRandomPlatform();
		FallRandomPlatform();
	}

	override public void End()
	{

	}

	override public void Loop()
	{
		if (Time.time % 3 == 0 && GetLevel() < 4)
		{
			FallRandomPlatform();
		}
	}


	override public bool HasEnded()
	{
		return GameManager.instance.numPlayersAlive <= 0;
	}
	
	private void FallRandomPlatform()
	{
		Platform platform = RoomManager.instance.GetRandomPlatform();
		FallPlatform(platform);
	}

	private void FallPlatform(Platform platform)
	{
		if (platformsFalling.Contains(platform))
			return;

		platformsFalling.Enqueue(platform);
		platform.EnterFall();
	}

	private void RecoverPlatform()
	{
		Platform platform = platformsFalling.Dequeue();
		platform.EnterRecover();
	}

}
