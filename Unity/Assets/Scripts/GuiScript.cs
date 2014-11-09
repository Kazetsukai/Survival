using UnityEngine;
using System.Collections;
using System;

public class GuiScript : MonoBehaviour {

	public float WaterMillilitres = 44000;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate() {
		WaterMillilitres -= Time.fixedDeltaTime;
	}

	void OnGUI()
	{
		var text = "Time: " + GetTime ().ToShortTimeString () + '\n' +
						"Water: " + (Math.Round (WaterMillilitres, 2)) + "ml" + '\n' +
						"" + '\n' +
						"" + '\n' +
						"" + '\n' +
						"" + '\n' +
						"";

		GUI.Box (new Rect (10, 10, 200, 300), text);
	}

	DateTime GetTime()
	{
		return new DateTime (2034, 4, 23, 8, 0, 0) + TimeSpan.FromMinutes (Time.timeSinceLevelLoad / 4);
	}
}
