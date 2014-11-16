using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Metabolism : MonoBehaviour {

	public float liquidWaterInStomach = 400F;
	public float foodWaterInStomach = 200F;
	public float sugarInStomach = 90F;
	public float proteinInStomach = 120F;
	public float fatInStomach = 80F;
	public float fibreInStomach = 50F;
	public float liquidWaterInGut = 200F;
	public float foodWaterInGut = 200F;
	public float sugarInGut = 90F;
	public float proteinInGut = 120F;
	public float fatInGut = 80F;
	public float fibreInGut = 50F;
	public float waterInBlood = 44000F;
	public float sugarInBlood = 90F;
	public float proteinInBlood = 120F;
	public float fatInBlood = 50F;
	float foodWaterReleaseRate = 0;
	float sugarReleaseRate = 0;
	float proteinReleaseRate = 0;
	float fatReleaseRate = 0;
	float fibreReleaseRate = 0;



	float totalStomachContents;
	List<DigestionPacket> digestionPackets = new List<DigestionPacket>();

	float timeCompression = 15F;
	float liquidWaterDigestionRate = 0.6F;
	float foodWaterDigestionRate = 0.4F;
	float sugarDigestionRate = 0.25F;
	float proteinDigestionRate = 0.1F;
	float fatDigestionRate = 0.05F;
	float fibreDigestionRate = 0.15F;

	float currentFoodWaterDigestionRate;
	float currentSugarDigestionRate;
	float currentProteinDigestionRate;
	float currentFatDigestionRate;
	float currentFibreDigestionRate;

	public float bloodVolume = 3000F;
	float bloodVolumeMax = 6000F;
	float bloodReplenishmentRate = 0.08F * 6000F / (24F * 60F * 60F);
	float waterDepletionRate = 1600F / (24F * 60F * 60F);
	float energyDepletionRate = 7918F / (24F * 60F * 60F);
	float energyFromSugar = 17F;
	float energyFromProtein = 17F;
	float energyFromFat = 39F;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Eat") && totalStomachContents + liquidWaterInStomach < 1000F) {
			Eat();
		}
		if (Input.GetButtonDown("Drink") && totalStomachContents + liquidWaterInStomach < 1000F) {
			Drink();
		}
	}

	void FixedUpdate() {

		// Deplete water
		waterInBlood -= waterDepletionRate * Time.fixedDeltaTime * timeCompression;

		// Replenish blood
		bloodVolume = Mathf.Min (bloodVolumeMax, bloodVolume + bloodReplenishmentRate * Time.fixedDeltaTime * timeCompression);

		// Spend base energy
		float energyRequired = energyDepletionRate * Time.fixedDeltaTime * timeCompression;
		float sugarRequired = energyRequired / energyFromSugar;
		if (sugarRequired < sugarInBlood) {
			sugarInBlood -= sugarRequired;
		} else {
			energyRequired -= sugarInBlood * energyFromSugar;
			sugarInBlood = 0;
			float proteinRequired = energyRequired / energyFromProtein;
			if (proteinRequired < proteinInBlood) {
				proteinInBlood -= proteinRequired;
			} else {
				energyRequired -= proteinInBlood * energyFromProtein;
				proteinInBlood = 0;
				float fatRequired = energyRequired / energyFromFat;
				if (fatRequired < fatInBlood) {
					fatInBlood -= fatRequired;
				} else {
					energyRequired -= fatInBlood * energyFromFat;
					fatInBlood = 0;
				}
			}
		}

		// work out how much total mass we have in the stomach
		totalStomachContents = foodWaterInStomach + sugarInStomach + proteinInStomach + fatInStomach + fibreInStomach;

		if (totalStomachContents > 0 || liquidWaterInStomach > 0) {

			// if there is any liquid (non-food) water in the stomach
			if (liquidWaterInStomach > 0) {
				// work out how much is being digested this update
				float liquidWaterBeingDigested = liquidWaterDigestionRate * Time.fixedDeltaTime * timeCompression;
				
				// if the last bit of water is being digested, shift it all to the gut, otherwise shift the amount being digested
				if (liquidWaterBeingDigested > liquidWaterInStomach) {
					liquidWaterInStomach = 0;
				} else {
					liquidWaterInStomach -= liquidWaterBeingDigested;
				}
			}
			// if there is any food water in the stomach
			if (foodWaterInStomach > 0) {
				// work out how much is being digested this update
				float foodWaterBeingDigested = currentFoodWaterDigestionRate * Time.fixedDeltaTime * timeCompression;

				// if the last bit of water is being digested, shift it all to the gut, otherwise shift the amount being digested
				if (foodWaterBeingDigested > foodWaterInStomach) {
					foodWaterInStomach = 0;
				} else {
					foodWaterInStomach -= foodWaterBeingDigested;
				}
			}
			// if there is any sugar in the stomach
			if (sugarInStomach > 0) {
				// work out how much is being digested this update
				float sugarBeingDigested = currentSugarDigestionRate * Time.fixedDeltaTime * timeCompression;

				// if the last bit of sugar is being digested, shift it all to the gut, otherwise shift the amount being digested
				if (sugarBeingDigested > sugarInStomach) {
					sugarInStomach = 0;
				} else {
					sugarInStomach -= sugarBeingDigested;
				}
			}
			// if there is any protein in the stomach
			if (proteinInStomach > 0) {
				// work out how much is being digested this update
				float proteinBeingDigested = currentProteinDigestionRate * Time.fixedDeltaTime * timeCompression;
				
				// if the last bit of protein is being digested, shift it all to the gut, otherwise shift the amount being digested
				if (proteinBeingDigested > proteinInStomach) {
					proteinInStomach = 0;
				} else {
					proteinInStomach -= proteinBeingDigested;
				}
			}
			// if there is any fat in the stomach
			if (fatInStomach > 0) {
				// work out how much is being digested this update
				float fatBeingDigested = currentFatDigestionRate * Time.fixedDeltaTime * timeCompression;
				
				// if the last bit of fat is being digested, shift it all to the gut, otherwise shift the amount being digested
				if (fatBeingDigested > fatInStomach) {
					fatInStomach = 0;
				} else {
					fatInStomach -= fatBeingDigested;
				}
			}
			// if there is any fibre in the stomach
			if (fibreInStomach > 0) {
				// work out how much is being digested this update
				float fibreBeingDigested = currentFibreDigestionRate * Time.fixedDeltaTime * timeCompression;
				
				// if the last bit of fibre is being digested, shift it all to the gut, otherwise shift the amount being digested
				if (fibreBeingDigested > fibreInStomach) {
					fibreInStomach = 0;
				} else {
					fibreInStomach -= fibreBeingDigested;
				}
			}
		} else {
			currentFoodWaterDigestionRate = foodWaterDigestionRate;
			currentSugarDigestionRate = sugarDigestionRate;
			currentProteinDigestionRate = proteinDigestionRate;
			currentFatDigestionRate = fatDigestionRate;
			currentFibreDigestionRate = fibreDigestionRate;
		}
		// if there is any liquid water in the gut
		if (liquidWaterInGut > 0) {
			// work out how much is being released this update
			float liquidWaterBeingReleased = liquidWaterDigestionRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of liquid water is being released, shift it all to the blood, otherwise shift the amount being released
			if (liquidWaterBeingReleased > liquidWaterInGut) {
				waterInBlood += liquidWaterInGut;
				liquidWaterInGut = 0;
			} else {
				waterInBlood += liquidWaterBeingReleased;
				liquidWaterInGut -= liquidWaterBeingReleased;
			}
		}
		// if there is any food water in the gut
		if (foodWaterInGut > 0) {
			// work out how much is being released this update
			float foodWaterBeingReleased = foodWaterReleaseRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of food water is being released, shift it all to the blood, otherwise shift the amount being released
			if (foodWaterBeingReleased > foodWaterInGut) {
				waterInBlood += foodWaterInGut;
				foodWaterInGut = 0;
			} else {
				waterInBlood += foodWaterBeingReleased;
				foodWaterInGut -= foodWaterBeingReleased;
			}
		}
		// if there is any sugar in the gut
		if (sugarInGut > 0) {
			// work out how much is being released this update
			float sugarBeingReleased = sugarReleaseRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of sugar is being released, shift it all to the blood, otherwise shift the amount being released
			if (sugarBeingReleased > sugarInGut) {
				sugarInBlood += sugarInGut;
				sugarInGut = 0;
			} else {
				sugarInBlood += sugarBeingReleased;
				sugarInGut -= sugarBeingReleased;
			}
		}
		// if there is any protein in the gut
		if (proteinInGut > 0) {
			// work out how much is being released this update
			float proteinBeingReleased = proteinReleaseRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of protein is being released, shift it all to the blood, otherwise shift the amount being released
			if (proteinBeingReleased > proteinInGut) {
				proteinInBlood += proteinInGut;
				proteinInGut = 0;
			} else {
				proteinInBlood += proteinBeingReleased;
				proteinInGut -= proteinBeingReleased;
			}
		}
		// if there is any fat in the gut
		if (fatInGut > 0) {
			// work out how much is being released this update
			float fatBeingReleased = fatReleaseRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of fat is being released, shift it all to the blood, otherwise shift the amount being released
			if (fatBeingReleased > fatInGut) {
				fatInBlood += fatInGut;
				fatInGut = 0;
			} else {
				fatInBlood += fatBeingReleased;
				fatInGut -= fatBeingReleased;
			}
		}
		// if there is any fibre in the gut
		if (fibreInGut > 0) {
			// work out how much is being released this update
			float fibreBeingReleased = fibreReleaseRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of fibre is being released, shift it all to the blood, otherwise shift the amount being released
			if (fibreBeingReleased > fibreInGut) {
				fibreInGut = 0;
			} else {
				fibreInGut -= fibreBeingReleased;
			}
		}



		foreach (var digestionPacket in digestionPackets) {
			if (digestionPacket.liquidWaterInGut > 0 && Time.time > digestionPacket.liquidWaterReleaseTime) {
				//Debug.Log ("Releasing liquid water");
				liquidWaterInGut += digestionPacket.liquidWaterInGut;
				digestionPacket.liquidWaterInGut = 0;
			}
			if (digestionPacket.foodWaterInGut > 0 && Time.time > digestionPacket.foodWaterReleaseTime) {
				//Debug.Log ("Releasing food water");

				foodWaterReleaseRate = (foodWaterInGut * foodWaterReleaseRate + digestionPacket.foodWaterInGut * digestionPacket.foodWaterReleaseRate) / (foodWaterInGut + digestionPacket.foodWaterInGut);

				foodWaterInGut += digestionPacket.foodWaterInGut;
				digestionPacket.foodWaterInGut = 0;
			}
			if (digestionPacket.sugarInGut > 0 && Time.time > digestionPacket.sugarReleaseTime) {

				sugarReleaseRate = (sugarInGut * sugarReleaseRate + digestionPacket.sugarInGut * digestionPacket.sugarReleaseRate) / (sugarInGut + digestionPacket.sugarInGut);

				sugarInGut += digestionPacket.sugarInGut;
				digestionPacket.sugarInGut = 0;
			}
			if (digestionPacket.proteinInGut > 0 && Time.time > digestionPacket.proteinReleaseTime) {
				//Debug.Log ("Releasing protein");
				
				proteinReleaseRate = (proteinInGut * proteinReleaseRate + digestionPacket.proteinInGut * digestionPacket.proteinReleaseRate) / (proteinInGut + digestionPacket.proteinInGut);
				
				proteinInGut += digestionPacket.proteinInGut;
				digestionPacket.proteinInGut = 0;
			}
			if (digestionPacket.fatInGut > 0 && Time.time > digestionPacket.fatReleaseTime) {
				//Debug.Log ("Releasing fat");
				
				fatReleaseRate = (fatInGut * fatReleaseRate + digestionPacket.fatInGut * digestionPacket.fatReleaseRate) / (fatInGut + digestionPacket.fatInGut);
				
				fatInGut += digestionPacket.fatInGut;
				digestionPacket.fatInGut = 0;
			}
			if (digestionPacket.fibreInGut > 0 && Time.time > digestionPacket.fibreReleaseTime) {
				//Debug.Log ("Releasing fibre");
				
				fibreReleaseRate = (fibreInGut * fibreReleaseRate + digestionPacket.fibreInGut * digestionPacket.fibreReleaseRate) / (fibreInGut + digestionPacket.fibreInGut);
				
				fibreInGut += digestionPacket.fibreInGut;
				digestionPacket.fibreInGut = 0;
				digestionPackets.Remove(digestionPacket);
			}
		}
		//Debug.Log ("Number of packets is " + digestionPackets.Count);
	}

	void Eat() {
		foodWaterInStomach += 200F;
		sugarInStomach += 90F;
		proteinInStomach += 120F;
		fatInStomach += 80F;
		fibreInStomach += 50F;

		totalStomachContents = foodWaterInStomach + sugarInStomach + proteinInStomach + fatInStomach + fibreInStomach;
		CalculateStomachContentProportionsAndRates ();

		digestionPackets.Add (new DigestionPacket (0, 200, currentFoodWaterDigestionRate, 90, currentSugarDigestionRate, 120, currentProteinDigestionRate, 80, currentFatDigestionRate, 50, currentFibreDigestionRate));
	}

	void Drink() {
		liquidWaterInStomach += 400F;

		totalStomachContents = foodWaterInStomach + sugarInStomach + proteinInStomach + fatInStomach + fibreInStomach;
		CalculateStomachContentProportionsAndRates ();
		digestionPackets.Add (new DigestionPacket (400, 0, currentFoodWaterDigestionRate, 0, currentSugarDigestionRate, 0, currentProteinDigestionRate, 0, currentFatDigestionRate, 0, currentFibreDigestionRate));
	}

	void CalculateStomachContentProportionsAndRates() {
	
		// work out the proportions of each food type in the stomach
		float foodWaterProportion = foodWaterInStomach / totalStomachContents;
		float sugarProportion = sugarInStomach / totalStomachContents;
		float proteinProportion = proteinInStomach / totalStomachContents;
		float fatProportion = fatInStomach / totalStomachContents;
		float fibreProportion = fibreInStomach / totalStomachContents;
		
		// work out the average digestion rate of all contents
		float averageDigestionRate = (foodWaterProportion * foodWaterDigestionRate + sugarProportion * sugarDigestionRate + proteinProportion * proteinDigestionRate + fatProportion * fatDigestionRate + fibreProportion * fibreDigestionRate) / 5;
		
		currentFoodWaterDigestionRate = foodWaterProportion * averageDigestionRate;
		currentSugarDigestionRate = sugarProportion * averageDigestionRate;
		currentProteinDigestionRate = proteinProportion * averageDigestionRate;
		currentFatDigestionRate = fatProportion * averageDigestionRate;
		currentFibreDigestionRate = fibreProportion * averageDigestionRate;

	}
}
