using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Metabolism : MonoBehaviour {

	public GraphingTool GlycogenInMusclesGraph;

	public float liquidWaterInStomach = 400F;
	public float foodWaterInStomach = 200F;
	public float glucoseInStomach = 90F;
	public float proteinInStomach = 120F;
	public float fatInStomach = 80F;
	public float fibreInStomach = 50F;
	public float liquidWaterInGut = 200F;
	public float foodWaterInGut = 200F;
	public float glucoseInGut = 90F;
	public float proteinInGut = 120F;
	public float fatInGut = 80F;
	public float fibreInGut = 50F;
	public float waterInBlood = 44000F;
	public float glucoseInBlood = 9F;
	public float proteinInBlood = 120F;
	public float fatInBlood = 50F;
	public float phosphocreatineInMuscles = 120F;
	public float glycogenInMuscles = 280F;
	public float glycogenInLiver = 120F;
	public float insulinInBlood = 0F;
	float muscleMassMax = 35000F;
	float muscleMass = 35000F;
	float insulinHalfLife = 300F;
	float insulinReleaseAmount = ((250F / 24F / 60F / 60F) * Mathf.Pow(2F, (1F/300F)) - (250F / 24F / 60F / 60F));
	float targetSugarInBlood = 8.4F;
	float foodWaterReleaseRate = 0;
	float glucoseReleaseRate = 0;
	float proteinReleaseRate = 0;
	float fatReleaseRate = 0;
	float fibreReleaseRate = 0;

	Rigidbody rb;
	CustomCharacterController cc;

	float totalStomachContents;
	List<DigestionPacket> digestionPackets = new List<DigestionPacket>();

	float timeCompression = 15F;
	float liquidWaterDigestionRate = 0.6F;
	float foodWaterDigestionRate = 0.4F;
	float glucoseDigestionRate = 0.25F;
	float proteinDigestionRate = 0.1F;
	float fatDigestionRate = 0.05F;
	float fibreDigestionRate = 0.15F;

	float phosphocreatineToEnergyRatio = 0.55F;
	float glucoseToEnergyRatio = 15.55F;
	float fatToEnergyRatio = 37.7F;
	float proteinToEnergyRatio = 16.8F;

	float walkingEnergy = 0.228F;
	float joggingEnergy = 1F;
	float sprintingEnergy = 7.6F;

	float maxFatUtilisation = 0.2F;
	float maxGlucoseUtilisation = 0.572F;
	float maxGlycogenUtilisation = 0.228F;
	float maxPhosphocreatineUtilisation = 6.6F

	
	float currentFoodWaterDigestionRate;
	float currentGlucoseDigestionRate;
	float currentProteinDigestionRate;
	float currentFatDigestionRate;
	float currentFibreDigestionRate;

	public float bloodVolume = 3000F;
	float bloodVolumeMax = 6000F;
	float bloodReplenishmentRate = 0.08F * 6000F / (24F * 60F * 60F);
	float waterDepletionRate = 1600F / (24F * 60F * 60F);
	float energyDepletionRate = 7918F / (24F * 60F * 60F);
	float proteinDepletionRate = 50F / (24F * 60F * 60F);

	// Use this for initialization
	void Start () {
		rb = GetComponentInParent<Rigidbody> ();
		cc = GetComponentInParent<CustomCharacterController> ();
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

		// Spend base energy
		SpendBaseEnergy ();

		// Deplete water
		waterInBlood -= waterDepletionRate * Time.fixedDeltaTime * timeCompression;

		// Replenish blood
		bloodVolume = Mathf.Min (bloodVolumeMax, bloodVolume + bloodReplenishmentRate * Time.fixedDeltaTime * timeCompression);

		// Deplete protein
		float proteinToDeplete = proteinDepletionRate * Time.fixedDeltaTime * timeCompression;
		if (proteinToDeplete < proteinInBlood) {
			proteinInBlood -= proteinToDeplete;
		} else {
			proteinToDeplete -= proteinInBlood;
			proteinInBlood = 0;
			muscleMass -= proteinToDeplete;
			if (muscleMass <= 0) {
				GetComponentInParent<Death>().Die ();
			}
		}

		// Deplete insulin
		if (insulinInBlood > 0) {
			insulinInBlood = insulinInBlood / Mathf.Pow (2, (Time.fixedDeltaTime * timeCompression / insulinHalfLife));
		} else {
			insulinInBlood = -(Mathf.Abs(insulinInBlood) / Mathf.Pow (2, (Time.fixedDeltaTime * timeCompression / insulinHalfLife)));
		}

		// Adjust insulin based on sugar levels in blood
		if (targetSugarInBlood < glucoseInBlood) {
			insulinInBlood += insulinReleaseAmount * Time.fixedDeltaTime * timeCompression;
		} else if (targetSugarInBlood > glucoseInBlood) {
			insulinInBlood -= insulinReleaseAmount * Time.fixedDeltaTime * timeCompression * 3;
		}

		// Transfer sugar/glycogen between liver/blood
		if (glycogenInLiver < 120F && insulinInBlood > 0 || glycogenInLiver > 0F && insulinInBlood < 0) {
			glycogenInLiver += insulinInBlood * Time.fixedDeltaTime * timeCompression;
			glucoseInBlood -= insulinInBlood * Time.fixedDeltaTime * timeCompression;
		}

		// Increase phosphocreatine stores
		float exertionFactor = 1;
		if (Input.GetAxis("Vertical") > 0) {
			if (Input.GetAxis("Sprint") > 0) {
				exertionFactor = 0;
			} else if (Input.GetAxis("Walk") > 0) {
				exertionFactor = 0.5F;
			} else {
				exertionFactor = 0;
			}
		} else if (Input.GetAxis("Vertical") < 0 || Input.GetAxis("Horizontal") != 0) {
			exertionFactor = 0.5F;
		}
			
		float phosphocreatineToRestore = Mathf.Max(Mathf.Min (120F - phosphocreatineInMuscles, 2F / 3F) * Time.fixedDeltaTime * exertionFactor, 0);
		float sugarRequiredToRestorePhosphocreatine = phosphocreatineToRestore * phosphocreatineToEnergyRatio / glucoseToEnergyRatio;
		if (sugarRequiredToRestorePhosphocreatine < glucoseInBlood) {
			glucoseInBlood -= sugarRequiredToRestorePhosphocreatine;
			phosphocreatineInMuscles += phosphocreatineToRestore;
		} else {
			phosphocreatineInMuscles += glucoseInBlood * glucoseToEnergyRatio / phosphocreatineToEnergyRatio;
			glucoseInBlood = 0;
		}
		

		// Increase muscle glycogen stores from liver
		float muscleGlycogenIncrease = Mathf.Max (0.0165F - glycogenInMuscles / 10000F, 0.002667F) * Time.fixedDeltaTime * timeCompression;
		if (muscleGlycogenIncrease < glycogenInLiver) {
			glycogenInLiver -= muscleGlycogenIncrease;
			glycogenInMuscles += muscleGlycogenIncrease;
		} else {
			glycogenInMuscles += glycogenInLiver;
			glycogenInLiver = 0;
		}

		// work out how much total mass we have in the stomach
		totalStomachContents = foodWaterInStomach + glucoseInStomach + proteinInStomach + fatInStomach + fibreInStomach;

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
			if (glucoseInStomach > 0) {
				// work out how much is being digested this update
				float sugarBeingDigested = currentGlucoseDigestionRate * Time.fixedDeltaTime * timeCompression;

				// if the last bit of sugar is being digested, shift it all to the gut, otherwise shift the amount being digested
				if (sugarBeingDigested > glucoseInStomach) {
					glucoseInStomach = 0;
				} else {
					glucoseInStomach -= sugarBeingDigested;
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
			currentGlucoseDigestionRate = glucoseDigestionRate;
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
		if (glucoseInGut > 0) {
			// work out how much is being released this update
			float sugarBeingReleased = glucoseReleaseRate * Time.fixedDeltaTime * timeCompression;
			
			// if the last bit of sugar is being released, shift it all to the blood, otherwise shift the amount being released
			if (sugarBeingReleased > glucoseInGut) {
				glucoseInBlood += glucoseInGut;
				glucoseInGut = 0;
			} else {
				glucoseInBlood += sugarBeingReleased;
				glucoseInGut -= sugarBeingReleased;
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

				glucoseReleaseRate = (glucoseInGut * glucoseReleaseRate + digestionPacket.sugarInGut * digestionPacket.sugarReleaseRate) / (glucoseInGut + digestionPacket.sugarInGut);

				glucoseInGut += digestionPacket.sugarInGut;
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
			if (digestionPacket.fibreInGut > 0 && Time.time > digestionPacket.fibreReleaseTime)  {
				//Debug.Log ("Releasing fibre");

				fibreReleaseRate = (fibreInGut * fibreReleaseRate + digestionPacket.fibreInGut * digestionPacket.fibreReleaseRate) / (fibreInGut + digestionPacket.fibreInGut);

				fibreInGut += digestionPacket.fibreInGut;
				digestionPacket.fibreInGut = 0;
			}
		}
		List<DigestionPacket> tempDigestionPackets = digestionPackets;
		foreach (var digestionPacket in tempDigestionPackets) {
			if (digestionPacket.liquidWaterInGut == 0 && digestionPacket.foodWaterInGut == 0 && digestionPacket.sugarInGut == 0 && digestionPacket.proteinInGut == 0 && digestionPacket.fatInGut == 0 && digestionPacket.fibreInGut == 0) {
				digestionPackets.Remove(digestionPacket);
			}
		}
		
		if (GlycogenInMusclesGraph != null)
		{
			GlycogenInMusclesGraph.LogDatum (glycogenInMuscles / 280f);
		}
	}

	void Eat() {
		foodWaterInStomach += 200F;
		glucoseInStomach += 90F;
		proteinInStomach += 120F;
		fatInStomach += 80F;
		fibreInStomach += 50F;

		totalStomachContents = foodWaterInStomach + glucoseInStomach + proteinInStomach + fatInStomach + fibreInStomach;
		CalculateStomachContentProportionsAndRates ();

		digestionPackets.Add (new DigestionPacket (0, 200, currentFoodWaterDigestionRate, 90, currentGlucoseDigestionRate, 120, currentProteinDigestionRate, 80, currentFatDigestionRate, 50, currentFibreDigestionRate));
	}

	void Drink() {
		liquidWaterInStomach += 400F;

		totalStomachContents = foodWaterInStomach + glucoseInStomach + proteinInStomach + fatInStomach + fibreInStomach;
		CalculateStomachContentProportionsAndRates ();
		digestionPackets.Add (new DigestionPacket (400, 0, currentFoodWaterDigestionRate, 0, currentGlucoseDigestionRate, 0, currentProteinDigestionRate, 0, currentFatDigestionRate, 0, currentFibreDigestionRate));
	}

	void CalculateStomachContentProportionsAndRates() {
	
		// work out the proportions of each food type in the stomach
		float foodWaterProportion = foodWaterInStomach / totalStomachContents;
		float sugarProportion = glucoseInStomach / totalStomachContents;
		float proteinProportion = proteinInStomach / totalStomachContents;
		float fatProportion = fatInStomach / totalStomachContents;
		float fibreProportion = fibreInStomach / totalStomachContents;
		
		// work out the average digestion rate of all contents
		float averageDigestionRate = (foodWaterProportion * foodWaterDigestionRate + sugarProportion * glucoseDigestionRate + proteinProportion * proteinDigestionRate + fatProportion * fatDigestionRate + fibreProportion * fibreDigestionRate) / 5;
		
		currentFoodWaterDigestionRate = foodWaterProportion * averageDigestionRate;
		currentGlucoseDigestionRate = sugarProportion * averageDigestionRate;
		currentProteinDigestionRate = proteinProportion * averageDigestionRate;
		currentFatDigestionRate = fatProportion * averageDigestionRate;
		currentFibreDigestionRate = fibreProportion * averageDigestionRate;

	}

	void SpendBaseEnergy() {
		float energyRequired = energyDepletionRate * Time.fixedDeltaTime * timeCompression;
		
		// Spend from sugar first
		float sugarRequired = energyRequired / glucoseToEnergyRatio;
		if (sugarRequired < glucoseInBlood) {
			glucoseInBlood -= sugarRequired;
		} else {
			
			// Spend from fat next
			energyRequired -= glucoseInBlood * glucoseToEnergyRatio;
			glucoseInBlood = 0;
			float fatRequired = energyRequired / fatToEnergyRatio;
			if (fatRequired < fatInBlood) {
				fatInBlood -= fatRequired;
			} else {
				// Spend from blood protein next
				energyRequired -= fatInBlood * fatToEnergyRatio;
				fatInBlood = 0;
				
				float proteinRequired = energyRequired / proteinToEnergyRatio;
				if (proteinRequired < proteinInBlood) {
					proteinInBlood -= proteinRequired;
				} else {
					// Spend from muscle mass next - starving!
					energyRequired -= proteinInBlood * proteinToEnergyRatio;
					proteinInBlood = 0;
					float muscleMassRequired = energyRequired / proteinToEnergyRatio; // calculate body mass reduction due to starvation
					if (muscleMassRequired < energyRequired) {
						muscleMass -= muscleMassRequired;
					} else {
						energyRequired -= muscleMass * proteinToEnergyRatio;
						muscleMass = 0;
						// pretty sure you're dead by now
						GetComponentInParent<Death>().Die();
					}
				}
			}
		}
	}
}
