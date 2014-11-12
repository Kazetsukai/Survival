using UnityEngine;
using System.Collections;
using System;

public class GuiScript : MonoBehaviour {

	public GameObject targetPlayer;
	Metabolism player;
	public GUIStyle style;

	// Use this for initialization
	void Start () {
		player = targetPlayer.GetComponent<Metabolism> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI()
	{
		var text = "Time: " + GetTime ().ToShortTimeString () + '\n' +
						"Body water: " + (Math.Round (player.waterInBody, 2)) + "ml" + '\n' +
						"Water in stomach:" + (Math.Round (player.waterInStomach, 2)) + "ml" + '\n' +
						"Sugar in blood:" + (Math.Round (player.sugarInBlood, 2)) + "g" + '\n' +
						"Sugar in stomach:" + (Math.Round (player.sugarInStomach, 2)) + "g" + '\n' +
						"Protein in blood:" + (Math.Round (player.proteinInBlood, 2)) + "g" + '\n' +
						"Protein in stomach:" + (Math.Round (player.proteinInStomach, 2)) + "g" + '\n' +
						"Fat in blood:" + (Math.Round (player.fatInBlood, 2)) + "g" + '\n' +
						"Fat in stomach:" + (Math.Round (player.fatInStomach, 2)) + "g" + '\n' +
						"Fibre in stomach:" + (Math.Round (player.fibreInStomach, 2)) + "g" + '\n';

		GUI.Box (new Rect (10, 10, 200, 300), text, style);
	}

	DateTime GetTime()
	{
		return new DateTime (2034, 4, 23, 8, 0, 0) + TimeSpan.FromMinutes (Time.timeSinceLevelLoad / 4);
	}
}
