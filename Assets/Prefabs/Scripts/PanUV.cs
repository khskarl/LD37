using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanUV : MonoBehaviour {
	public  float scrollSpeed = 0.1f;
	public float distance = 0.1f;

	public Renderer rend;
	void Start()
	{
		rend = GetComponent<Renderer>();
	}
	void Update()
	{
		float offset = Mathf.Sin(Time.time * scrollSpeed) * distance;
		rend.material.SetTextureOffset("_MainTex", new Vector2(0, -offset));
	}
}
