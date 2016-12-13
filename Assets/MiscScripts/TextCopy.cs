using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCopy : MonoBehaviour {
	Text[] texts;

	// Use this for initialization
	void Start () {
		texts = GetComponentsInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetText(string text)
	{
		foreach (Text t in texts)
		{
			t.text = text;
		}
	}
}
