using UnityEngine;
using System.Collections;

public class Metabolism : MonoBehaviour {

	public float liquidWaterInStomach = 400F;
	public float foodWaterInStomach = 200F;
	public float sugarInStomach = 90F;
	public float proteinInStomach = 120F;
	public float fatInStomach = 80F;
	public float fibreInStomach = 50F;
	public float waterInGut = 200F;
	public float sugarInGut = 90F;
	public float proteinInGut = 120F;
	public float fatInGut = 80F;
	public float fibreInGut = 50F;
	public float waterInBlood = 44000F;
	public float sugarInBlood = 90F;
	public float proteinInBlood = 120F;
	public float fatInBlood = 50F;

	float timeCompression = 15F;
	float liquidWaterDigestionRate = 0.6F;
	float foodWaterDigestionRate = 0.4F;
	float sugarDigestionRate = 0.25F;
	float proteinDigestionRate = 0.1F;
	float fatDigestionRate = 0.05F;
	float fibreDigestionRate = 0.15F;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {

		// work out how much total mass we have in the stomach
		float totalStomachContents = foodWaterInStomach + sugarInStomach + proteinInStomach + fatInStomach + fibreInStomach;

		// work out the proportions of each food type in the stomach
		float waterProportion = foodWaterInStomach / totalStomachContents;
		float sugarProportion = sugarInStomach / totalStomachContents;
		float proteinProportion = proteinInStomach / totalStomachContents;
		float fatProportion = fatInStomach / totalStomachContents;
		float fibreProportion = fibreInStomach / totalStomachContents;

		// work out the average digestion rate of all contents
		float averageDigestionRate = (waterProportion * foodWaterDigestionRate + sugarProportion * sugarDigestionRate + proteinProportion * proteinDigestionRate + fatProportion * fatDigestionRate + fibreProportion * fibreDigestionRate) / 5;

		// if there is any liquid (non-food) water in the stomach
		if (liquidWaterInStomach > 0) {
			// work out how much is being digested this update
			float liquidWaterBeingDigested = liquidWaterDigestionRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of water is being digested, shift it all to the gut, otherwise shift the amount being digested
			if (liquidWaterBeingDigested > liquidWaterInStomach) {
				waterInGut += liquidWaterInStomach;
				liquidWaterInStomach = 0;
			} else {
				liquidWaterInStomach -= liquidWaterBeingDigested;
				waterInGut += liquidWaterBeingDigested;
			}
		}
		// if there is any food water in the stomach
		if (foodWaterInStomach > 0) {
			// work out how much is being digested this update
			float waterBeingDigested = waterProportion * averageDigestionRate * Time.fixedDeltaTime * timeCompression;

			// if the last bit of water is being digested, shift it all to the gut, otherwise shift the amount being digested
			if (waterBeingDigested > foodWaterInStomach) {
				waterInGut += foodWaterInStomach;
				foodWaterInStomach = 0;
			} else {
				foodWaterInStomach -= waterBeingDigested;
				waterInGut += waterBeingDigested;
			}
		}
		// if there is any sugar in the stomach
		if (sugarInStomach > 0) {
			// work out how much is being digested this update
			float sugarBeingDigested = sugarProportion * averageDigestionRate * Time.fixedDeltaTime * timeCompression;

			// if the last bit of sugar is being digested, shift it all to the gut, otherwise shift the amount being digested
			if (sugarBeingDigested > sugarInStomach) {
				sugarInGut += sugarInStomach;
				sugarInStomach = 0;
			} else {
				sugarInStomach -= sugarBeingDigested;
				sugarInGut += sugarBeingDigested;
			}
		}
		// if there is any protein in the stomach
		if (proteinInStomach > 0) {
			// work out how much is being digested this update
			float proteinBeingDigested = proteinProportion * averageDigestionRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of protein is being digested, shift it all to the gut, otherwise shift the amount being digested
			if (proteinBeingDigested > proteinInStomach) {
				proteinInGut += proteinInStomach;
				proteinInStomach = 0;
			} else {
				proteinInStomach -= proteinBeingDigested;
				proteinInGut += proteinBeingDigested;
			}
		}
		// if there is any fat in the stomach
		if (fatInStomach > 0) {
			// work out how much is being digested this update
			float fatBeingDigested = fatProportion * averageDigestionRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of fat is being digested, shift it all to the gut, otherwise shift the amount being digested
			if (fatBeingDigested > fatInStomach) {
				fatInGut += fatInStomach;
				fatInStomach = 0;
			} else {
				fatInStomach -= fatBeingDigested;
				fatInGut += fatBeingDigested;
			}
		}
		// if there is any fibre in the stomach
		if (fibreInStomach > 0) {
			// work out how much is being digested this update
			float fibreBeingDigested = fibreProportion * averageDigestionRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of fibre is being digested, shift it all to the gut, otherwise shift the amount being digested
			if (fibreBeingDigested > fibreInStomach) {
				fibreInGut += fibreInStomach;
				fibreInStomach = 0;
			} else {
				fibreInStomach -= fibreBeingDigested;
				fibreInGut += fibreBeingDigested;
			}
		}
	}
}
