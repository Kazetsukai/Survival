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
						"Water in gut: " + (Math.Round (player.waterInGut, 2)) + "ml" + '\n' +
						"Liquid water in stomach:" + (Math.Round (player.liquidWaterInStomach, 2)) + "ml" + '\n' +
						"Food water in stomach:" + (Math.Round (player.foodWaterInStomach, 2)) + "ml" + '\n' +
						"Sugar in gut:" + (Math.Round (player.sugarInGut, 2)) + "g" + '\n' +
						"Sugar in stomach:" + (Math.Round (player.sugarInStomach, 2)) + "g" + '\n' +
						"Protein in gut:" + (Math.Round (player.proteinInGut, 2)) + "g" + '\n' +
						"Protein in stomach:" + (Math.Round (player.proteinInStomach, 2)) + "g" + '\n' +
						"Fat in gut:" + (Math.Round (player.fatInGut, 2)) + "g" + '\n' +
						"Fat in stomach:" + (Math.Round (player.fatInStomach, 2)) + "g" + '\n' +
						"Fibre in gut:" + (Math.Round (player.fibreInGut, 2)) + "g" + '\n' +
						"Fibre in stomach:" + (Math.Round (player.fibreInStomach, 2)) + "g" + '\n';

		GUI.Box (new Rect (10, 10, 200, 300), text, style);
	}

	DateTime GetTime()
	{
		return new DateTime (2034, 4, 23, 8, 0, 0) + TimeSpan.FromMinutes (Time.timeSinceLevelLoad / 4);
	}
}
