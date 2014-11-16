using UnityEngine;
using System.Collections;

public class DigestionPacket {

	public float liquidWaterInGut = 0;
	public float foodWaterInGut = 0;
	public float sugarInGut = 0;
	public float proteinInGut = 0;
	public float fatInGut = 0;
	public float fibreInGut = 0;
	public float liquidWaterReleaseTime = 0;
	public float foodWaterReleaseTime = 0;
	public float sugarReleaseTime = 0;
	public float proteinReleaseTime = 0;
	public float fatReleaseTime = 0;
	public float fibreReleaseTime = 0;
	public float liquidWaterReleaseRate = 0;
	public float foodWaterReleaseRate = 0;
	public float sugarReleaseRate = 0;
	public float proteinReleaseRate = 0;
	public float fatReleaseRate = 0;
	public float fibreReleaseRate = 0;
	public float timeCompression = 15;


	public DigestionPacket (float newLiquidWaterInGut, float newFoodWaterInGut, float newFoodWaterReleaseRate, float newSugarInGut, float newSugarReleaseRate, float newProteinInGut, float newProteinReleaseRate, float newFatInGut, float newFatReleaseRate, float newFibreInGut, float newFibreReleaseRate) {
		liquidWaterInGut = newLiquidWaterInGut;
		foodWaterInGut = newFoodWaterInGut;
		sugarInGut = newSugarInGut;
		proteinInGut = newProteinInGut;
		fatInGut = newFatInGut;
		fibreInGut = newFibreInGut;

		liquidWaterReleaseRate = 0.5F;
		foodWaterReleaseRate = newFoodWaterReleaseRate;
		sugarReleaseRate = newSugarReleaseRate;
		proteinReleaseRate = newProteinReleaseRate;
		fatReleaseRate = newFatReleaseRate;
		fibreReleaseRate = newFibreReleaseRate;


		liquidWaterReleaseTime = Time.time + 60 / timeCompression;
		foodWaterReleaseTime = Time.time + 1800 / timeCompression;
		sugarReleaseTime = Time.time + 600 / timeCompression;
		proteinReleaseTime = Time.time + 3600 / timeCompression;
		fatReleaseTime = Time.time + 10800 / timeCompression;
		fibreReleaseTime = Time.time + 3600 / timeCompression;
	}
}
