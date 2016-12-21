using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMG : Minigame
{
	List<GameObject> lasers = new List<GameObject>();

	public Laser laserPrefab;

	LaserMG()
	{
		name = "Laser";
		objective = "Dodge the lasers!";		
	}


	override public void Begin()
	{
		// CreateLaser("laser_horizontal_constant_1", -2);
	}

	override public void End()
	{
		GameManager.instance.Delete(lasers);
	}

	override public bool HasEnded()
	{
		return GameManager.instance.numPlayersAlive == 0;
	}
	
	override public void Loop()
	{
		switch (GameManager.instance.GetLevel())
		{
			case 1:
				LevelOneLoop();
				break;

			case 2:
				LevelTwoLoop();
				break;

			case 3:
				LevelThreeLoop();
				break;

			case 4:
				LevelFourLoop();
				break;
			case 5:
				LevelFiveLoop();
				break;
			default:
				LevelFourLoop();
				break;
		}
	}
	
	void LevelOneLoop()
	{
		if (Time.time % 4 == 0)
		{
			CreateLaser("laser_horizontal_constant_1", 2);
		}
	}

	void LevelTwoLoop()
	{
		if (Time.time % 4 == 0)
		{
			CreateLaser("laser_horizontal_constant_1", 2);
		}
		if ((Time.time + 2) % 4 == 0)
		{
			CreateLaser("laser_horizontal_constant_1", -2);
		}
	}

	void LevelThreeLoop()
	{
		if (Time.time % 4 == 0)
		{
			CreateLaser("laser_horizontal_constant_1", 3);
			CreateLaser("laser_horizontal_constant_1", -3);
		}
	}

	void LevelFourLoop()
	{
		if (Time.time % 6 == 0)
		{
			CreateLaser("laser_horizontal_constant_1", -1);

			CreateLaser("laser_horizontal_constant_1", 1);
		}
		if ((Time.time + 2) % 4 == 0)
		{
			CreateLaser("laser_vertical_constant_1", 1);

			CreateLaser("laser_horizontal_constant_1", -1);
		}
	}

	void LevelFiveLoop()
	{
		if (Time.time % 6 == 0)
		{
			CreateLaser("laser_horizontal_constant_1", -2);

			CreateLaser("laser_horizontal_constant_1", 2);
		}
		if ((Time.time + 2) % 4 == 0)
		{
			CreateLaser("laser_horizontal_constant_1", 2);

			CreateLaser("laser_horizontal_constant_1", -2);
		}
	}

	void CreateLaser(string animationName, float speed)
	{
		Laser laser = GameObject.Instantiate(laserPrefab, Vector3.zero, Quaternion.identity) as Laser;
		laser.StartAnimation(animationName, speed);

		lasers.Add(laser.gameObject);
	}
}
