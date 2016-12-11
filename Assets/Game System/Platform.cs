using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Platform : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		//List<MeshFilter> meshes = new List<MeshFilter>();

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
	
	void FixedUpdate()
	{
		//WaveMovement();
	}

	void WaveMovement ()
	{
		Vector3 pos = transform.position;
		float height = Mathf.Sin(pos.x + Time.time) + Mathf.Cos(pos.z + Time.time);
		pos.y = height * 0.025f;

		transform.position = pos;
	}
}
