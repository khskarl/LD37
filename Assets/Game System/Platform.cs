using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformState
{
	Idle,
	Trumble,
	Fall,
	Recover
}

public class Platform : MonoBehaviour {
	public PlatformState state = PlatformState.Idle;
	Dictionary<PlatformState, Action> stateAction = new Dictionary<PlatformState, Action>();

	float timeEnteredState = 0f;

	void Start () {
		SetupMesh();
		
		stateAction.Add(PlatformState.Idle, this.StateIdle);
		stateAction.Add(PlatformState.Trumble, this.StateTrumble);
		stateAction.Add(PlatformState.Fall, this.StateFall);
		stateAction.Add(PlatformState.Recover, this.StateRecover);
	}

	void FixedUpdate()
	{
		if (stateAction.ContainsKey(state))
			stateAction[state]();
	}

	void StateIdle()
	{

	}

	void StateTrumble()
	{

	}

	public void EnterFall()
	{
		timeEnteredState = Time.time;
		state = PlatformState.Fall;
	}
	void StateFall()
	{
		Vector3 currPos = transform.position;
		float currHeight = currPos.y;

		float newHeight = currHeight - 2 * Time.fixedDeltaTime;

		currPos.y = newHeight;
		transform.position = currPos;
	}

	public void EnterRecover()
	{
		timeEnteredState = Time.time;
		state = PlatformState.Recover;
	}
	void StateRecover()
	{
		Vector3 currPos = transform.position;
		float currHeight = currPos.y;
		float targetHeight = 0;

		float t = Mathf.Max(Time.time - timeEnteredState, 2) / 2f;

		float newHeight = Mathf.Lerp(currHeight, targetHeight, t * Time.fixedDeltaTime);

		currPos.y = newHeight;
		transform.position = currPos;
	}

	void SetupMesh()
	{
		MeshFilter[] meshFilters = this.GetComponentsInChildren<MeshFilter>();
		float numTiles = 4;
		float dx = 1f / numTiles;
		for (int i = 0; i < meshFilters.Length; i++)
		{
			Mesh mesh = meshFilters[i].mesh;
			Vector3[] vertices = mesh.vertices;
			Vector2[] uvs = new Vector2[vertices.Length];

			if (uvs.Length < 4)
				continue;

			float ix = Mathf.FloorToInt(transform.position.x / 2 % numTiles) * dx;
			float iz = Mathf.FloorToInt(transform.position.z / 2 % numTiles) * dx;


			uvs[0] = new Vector2(ix, iz);
			uvs[3] = new Vector2(ix, iz + dx);
			uvs[2] = new Vector2(dx + ix, iz);
			uvs[1] = new Vector2(dx + ix, iz + dx);

			mesh.uv = uvs;
		}
	}
	
	void WaveMovement ()
	{
		Vector3 pos = transform.position;
		float height = Mathf.Sin(pos.x + Time.time) + Mathf.Cos(pos.z + Time.time);
		pos.y = height * 0.025f;

		transform.position = pos;
	}
}
