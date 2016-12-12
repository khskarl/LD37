using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomManager : MonoBehaviour
{
	public static RoomManager instance = null;

	public int width = 8;
	public int depth = 8;

	Platform[,] platforms;
	List<Transform> walls;

	/* References */

	/* Prefabs */
	public Platform platformPrefab;
	public Transform wallPrefab;

	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

	}

	void Start()
	{
		platforms = new Platform[depth, width];
		walls = new List<Transform>();

		CreateRoom();
	}

	void Update()
	{
	}

	void CreateRoom()
	{
		CreatePlatforms();
		
		// Create walls
		CreateWallsFloor(0);
		CreateWallsFloor(-4);
		CreateWallsFloor(-8);
	}

	void CreatePlatforms()
	{
		float dx = 2;
		float offset_x = width - dx;
		float offset_z = depth - dx;

		// Create platforms
		for (int i = 0; i < depth; i++)
		{
			float z = i * dx - offset_z;
			for (int j = 0; j < width; j++)
			{
				float x = j * dx - offset_x;

				platforms[i, j] = Instantiate(platformPrefab, new Vector3(x, 0, z), Quaternion.identity) as Platform;
			}
		}
	}

	void CreateWallsFloor(float height)
	{
		float dx = 2;
		float offset_x = width - dx;
		float offset_z = depth - dx;

		for (int i = 0; i < width; i++)
		{
			float x = i * dx - offset_x;
			float z = -dx - offset_z;
			Transform wall = Instantiate(wallPrefab, new Vector3(x, height, z), Quaternion.identity) as Transform;
			walls.Add(wall);
			wall = Instantiate(wallPrefab, new Vector3(x, height, -z + dx), Quaternion.identity) as Transform;
		}

		for (int i = 0; i < depth; i++)
		{
			float z = i * dx - offset_z;
			float x = -dx - offset_x;
			Transform wall = Instantiate(wallPrefab, new Vector3(x, height, z), Quaternion.identity) as Transform;
			walls.Add(wall);
			wall = Instantiate(wallPrefab, new Vector3(-x + dx, height, z), Quaternion.identity) as Transform;
		}
	}



	public void OuterFall(int radius)
	{
		for (int i = 0; i < radius; i++)
		{
			for (int j = 0; j < width; j++)
			{
				platforms[i, j].EnterFall();
			}
		}

		for (int i = depth - radius; i < depth; i++)
		{
			for (int j = 0; j < width; j++)
			{
				platforms[i, j].EnterFall();
			}
		}


		for (int j = 0; j < radius; j++)
		{
			for (int i = 0; i < depth; i++)
			{
				platforms[i, j].EnterFall();
			}
		}
		for (int j = width - radius; j < width; j++)
		{
			for (int i = 0; i < depth; i++)
			{
				platforms[i, j].EnterFall();
			}
		}
	}

	public void OuterRecover(int radius)
	{
		for (int i = 0; i < radius; i++)
		{
			for (int j = 0; j < width; j++)
			{
				platforms[i, j].EnterRecover();
			}
		}

		for (int i = depth - radius; i < depth; i++)
		{
			for (int j = 0; j < width; j++)
			{
				platforms[i, j].EnterRecover();
			}
		}


		for (int j = 0; j < radius; j++)
		{
			for (int i = 0; i < depth; i++)
			{
				platforms[i, j].EnterRecover();
			}
		}
		for (int j = width - radius; j < width; j++)
		{
			for (int i = 0; i < depth; i++)
			{
				platforms[i, j].EnterRecover();
			}
		}
	}
}
