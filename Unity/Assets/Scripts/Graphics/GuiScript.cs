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
			"Blood: " + (Math.Round (player.bloodVolume, 2)) + "ml" + '\n' +
			"Water in blood: " + (Math.Round (player.waterInBlood, 2)) + "ml" + '\n' +
			"Liquid water in gut: " + (Math.Round (player.liquidWaterInGut, 2)) + "ml" + '\n' +
			"Food water in gut: " + (Math.Round (player.foodWaterInGut, 2)) + "ml" + '\n' +
			"Liquid water in stomach:" + (Math.Round (player.liquidWaterInStomach, 2)) + "ml" + '\n' +
			"Food water in stomach:" + (Math.Round (player.foodWaterInStomach, 2)) + "ml" + '\n' +
			"Glycogen in liver:" + (Math.Round (player.glycogenInLiver, 2)) + "g" + '\n' +
			"Glycogen in muscles:" + (Math.Round (player.glycogenInMuscles, 2)) + "g" + '\n' +
			"Insulin in blood:" + (Math.Round (player.insulinInBlood * 100000, 2)) + '\n' +
			"Glucagon in blood:" + (Math.Round (player.glucagonInBlood * 100000, 2)) + '\n' +
			"Sugar in blood:" + (Math.Round (player.glucoseInBlood, 2)) + "g" + '\n' +
			"Sugar in gut:" + (Math.Round (player.glucoseInGut, 2)) + "g" + '\n' +
			"Sugar in stomach:" + (Math.Round (player.glucoseInStomach, 2)) + "g" + '\n' +
			"PCr in muscles:" + (Math.Round (player.phosphocreatineInMuscles, 2)) + "g" + '\n' +
			"Protein in blood:" + (Math.Round (player.proteinInBlood, 2)) + "g" + '\n' +
			"Protein in gut:" + (Math.Round (player.proteinInGut, 2)) + "g" + '\n' +
			"Protein in stomach:" + (Math.Round (player.proteinInStomach, 2)) + "g" + '\n' +
			"Fat in blood:" + (Math.Round (player.fatInBlood, 2)) + "g" + '\n' +
			"Fat in gut:" + (Math.Round (player.fatInGut, 2)) + "g" + '\n' +
			"Fat in stomach:" + (Math.Round (player.fatInStomach, 2)) + "g" + '\n' +
			"Fibre in gut:" + (Math.Round (player.fibreInGut, 2)) + "g" + '\n' +
			"Fibre in stomach:" + (Math.Round (player.fibreInStomach, 2)) + "g" + '\n' +
			"Lactate system saturation:" + (Math.Round (player.lactateSaturation, 2)) + '\n' +
			"Exertion factor:" + (Math.Round (player.exertionFactor, 2)) + '\n' +
			"Muscle mass:" + (Math.Round (player.muscleMass, 2)) + 'g' + '\n';

		GUI.Box (new Rect (10, 10, 200, 300), text, style);
	}

	DateTime GetTime()
	{
		return new DateTime (2034, 4, 23, 8, 0, 0) + TimeSpan.FromMinutes (Time.timeSinceLevelLoad / 4);
	}
}
