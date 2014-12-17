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
	public float lactateSaturation = 100F;
	public float glycogenInMuscles = 280F;
	public float glycogenInLiver = 120F;
	public float insulinInBlood = 0F;
	public float glucagonInBlood = 0F;
	public float muscleMassMax = 35000F;
	public float muscleMass = 35000F;
	float insulinHalfLife = 300F;
//	float insulinReleaseAmount = ((250F / 24F / 60F / 60F) * Mathf.Pow(2F, (1F/300F)) - (250F / 24F / 60F / 60F));
	float insulinReleaseAmount = 0.0001F;
	float glucagonHalfLife = 15F * 6F;
//	float glucagonReleaseAmount = ((250F / 24F / 60F / 60F) * Mathf.Pow(2F, (1F/300F)) - (250F / 24F / 60F / 60F));
	float glucagonReleaseAmount = 0.0001F;
	float targetSugarInBlood = 8.4F;
	float sugarInBloodHighLevel = 13F;
	float sugarInBloodLowLevel = 7F;
	float foodWaterReleaseRate = 0F;
	float glucoseReleaseRate = 0F;
	float proteinReleaseRate = 0F;
	float fatReleaseRate = 0F;
	float fibreReleaseRate = 0F;

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


	float maxFatUtilisation = 0.2F;
	float maxMuscleUtilisation = 0.2F;
	float maxGlucoseUtilisation = 0.8F;
	float maxGlycogenUtilisation = 2F;
	float maxPhosphocreatineUtilisation = 4.6F;

	
	float currentFoodWaterDigestionRate;
	float currentGlucoseDigestionRate;
	float currentProteinDigestionRate;
	float currentFatDigestionRate;
	float currentFibreDigestionRate;

	public float bloodVolume = 3000F;
	float bloodVolumeMax = 6000F;
	float bloodReplenishmentRate = 0.08F * 6000F / (24F * 60F * 60F);
	float muscleMassRegenerationRate = 1F / 15F;
	float waterDepletionRate = 1600F / (24F * 60F * 60F);
	float energyDepletionRate = 7918F / (24F * 60F * 60F);
	float proteinDepletionRate = 50F / (24F * 60F * 60F);

	// Use this for initialization
	void Start () {
		rb = GetComponentInParent<Rigidbody> ();
		cc = GetComponentInParent<CustomCharacterController> ();
		/*digestionPackets.Add (new DigestionPacket (0F, 200F, 0.01728395F, 90F, 0.007777778F, 120F, 0.01037037F, 80F, 0.00691358F, 50F, 0.004320988F));
		digestionPackets [0].fatReleaseTime = Time.time;
		digestionPackets [0].fibreReleaseTime = Time.time;
		digestionPackets [0].foodWaterReleaseTime = Time.time;
		digestionPackets [0].liquidWaterReleaseTime = Time.time;
		digestionPackets [0].proteinReleaseTime = Time.time;
		digestionPackets [0].sugarReleaseTime = Time.time;*/
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
		// Work out blood loss multiplier
		float bloodLossMultiplier = bloodVolume / bloodVolumeMax;

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

		// Regenerate muscle mass
		if (muscleMass < muscleMassMax) {
			float muscleMassToIncrease = Mathf.Min (proteinInBlood, (muscleMassMax - muscleMass), muscleMassRegenerationRate * Time.fixedDeltaTime * timeCompression);
			proteinInBlood -= muscleMassToIncrease;
			muscleMass += muscleMassToIncrease;
		}

		// Deplete insulin
		if (insulinInBlood > 0) {
			insulinInBlood = insulinInBlood / Mathf.Pow (2, (Time.fixedDeltaTime * timeCompression / (insulinHalfLife / bloodLossMultiplier)));
		}

		// Deplete glucagon
		if (glucagonInBlood > 0) {
			glucagonInBlood = glucagonInBlood / Mathf.Pow (2, (Time.fixedDeltaTime * timeCompression / (glucagonHalfLife / bloodLossMultiplier)));
		}

		float insulinReleaseMultiplier = (glucoseInBlood - targetSugarInBlood) / (sugarInBloodHighLevel - targetSugarInBlood);
		float glucagonReleaseMultiplier = (targetSugarInBlood - glucoseInBlood) / (targetSugarInBlood - sugarInBloodLowLevel);

		// Adjust insulin based on sugar levels in blood
		if (glucoseInBlood > targetSugarInBlood) {
			insulinInBlood += insulinReleaseAmount * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier * insulinReleaseMultiplier;
		} else if (glucoseInBlood < targetSugarInBlood) {
			glucagonInBlood += glucagonReleaseAmount * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier * glucagonReleaseMultiplier;
		}

		// Transfer sugar/glycogen between liver/blood by insulin
		float glucoseToTransfer = Mathf.Max (0, Mathf.Min (insulinInBlood * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier, glucoseInBlood));

		glycogenInLiver += glucoseToTransfer;
		glucoseInBlood -= glucoseToTransfer;

		// Transfer sugar/glycogen between liver/blood by glucagon
		glucoseToTransfer = Mathf.Max (0, Mathf.Min (glucagonInBlood * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier, glycogenInLiver));

		glycogenInLiver -= glucoseToTransfer;
		glucoseInBlood += glucoseToTransfer;

		// Increase phosphocreatine stores
		float phosphocreatineToRestore = Mathf.Max(Mathf.Min (120F - phosphocreatineInMuscles, 2F / 3F) * Time.fixedDeltaTime * bloodLossMultiplier, 0);
		float sugarRequiredToRestorePhosphocreatine = phosphocreatineToRestore * phosphocreatineToEnergyRatio / glucoseToEnergyRatio;
		if (sugarRequiredToRestorePhosphocreatine < glucoseInBlood) {
			glucoseInBlood -= sugarRequiredToRestorePhosphocreatine;
			phosphocreatineInMuscles += phosphocreatineToRestore;
		} else {
			phosphocreatineInMuscles += glucoseInBlood * glucoseToEnergyRatio / phosphocreatineToEnergyRatio;
			glucoseInBlood = 0;
		}
		
		// Decrease lactate saturation
		lactateSaturation = Mathf.Max (0, lactateSaturation - Time.fixedDeltaTime / 3600 * bloodLossMultiplier);

		// Increase muscle glycogen stores from liver
		float muscleGlycogenIncrease = Mathf.Max (0.0165F - glycogenInMuscles / 10000F, 0.002667F) * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
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
				float liquidWaterBeingDigested = liquidWaterDigestionRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
				
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
				float foodWaterBeingDigested = currentFoodWaterDigestionRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;

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
				float sugarBeingDigested = currentGlucoseDigestionRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;

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
				float proteinBeingDigested = currentProteinDigestionRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
				
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
				float fatBeingDigested = currentFatDigestionRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
				
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
				float fibreBeingDigested = currentFibreDigestionRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
				
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
			float liquidWaterBeingReleased = liquidWaterDigestionRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
			
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
			float foodWaterBeingReleased = foodWaterReleaseRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
			
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
			float sugarBeingReleased = glucoseReleaseRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;

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
			float proteinBeingReleased = proteinReleaseRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
			
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
			float fatBeingReleased = fatReleaseRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
			
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
			float fibreBeingReleased = fibreReleaseRate * Time.fixedDeltaTime * timeCompression * bloodLossMultiplier;
			
			// if the last bit of fibre is being released, shift it all to the blood, otherwise shift the amount being released
			if (fibreBeingReleased > fibreInGut) {
				fibreInGut = 0;
			} else {
				fibreInGut -= fibreBeingReleased;
			}
		}



		foreach (var digestionPacket in digestionPackets) {
			// slow down gut digestion according to blood loss levels
			digestionPacket.liquidWaterReleaseTime += (1F - bloodLossMultiplier) * Time.fixedDeltaTime;
			digestionPacket.foodWaterReleaseTime += (1F - bloodLossMultiplier) * Time.fixedDeltaTime;
			digestionPacket.sugarReleaseTime += (1F - bloodLossMultiplier) * Time.fixedDeltaTime;
			digestionPacket.proteinReleaseTime += (1F - bloodLossMultiplier) * Time.fixedDeltaTime;
			digestionPacket.fatReleaseTime += (1F - bloodLossMultiplier) * Time.fixedDeltaTime;
			digestionPacket.fibreReleaseTime += (1F - bloodLossMultiplier) * Time.fixedDeltaTime;

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
					float muscleMassRequired = (energyRequired / proteinToEnergyRatio) * 2; // calculate body mass reduction due to starvation
					if (muscleMassRequired < muscleMass) {
						muscleMass -= muscleMassRequired;
					} else {
						energyRequired -= (muscleMass * proteinToEnergyRatio) / 2;
						muscleMass = 0;
						// pretty sure you're dead by now
						GetComponentInParent<Death>().Die();
					}
				}
			}
		}
	}

	public float DrawEnergy(float totalEnergyRequired) {
		//Debug.Log ("I have been asked for " + totalEnergyRequired + " kJ");
		float energyRequired = totalEnergyRequired; // used to track how much energy is still unaccounted for
		// work out how much energy we are taking from phosphocreatine
		float energyFromPhosphocreatine = Mathf.Min (energyRequired, maxPhosphocreatineUtilisation * Time.fixedDeltaTime);
		//Debug.Log ("I can take " + energyFromPhosphocreatine + " kJ from phosphocreatine");
		//Debug.Log ("The other value was " + (maxPhosphocreatineUtilisation * Time.fixedDeltaTime));
		
		// work out how much phosphocreatine mass this is
		float massFromPhosphocreatine = energyFromPhosphocreatine / phosphocreatineToEnergyRatio;
		
		// subtract from phosphocreatine stores and energy required
		if (massFromPhosphocreatine <= phosphocreatineInMuscles) {
			phosphocreatineInMuscles -= massFromPhosphocreatine; // we have enough glucose to meet the energy requirement
			energyRequired -= energyFromPhosphocreatine;
			//Debug.Log("I have enough phosphocreatine available, so now the remainder of the request is " + energyRequired);
		} else {
			// not enough energy in phosphocreatine stores, so deplete them fully and return reporting how much energy we were able to secure
			energyRequired -= phosphocreatineInMuscles * phosphocreatineToEnergyRatio;
			phosphocreatineInMuscles = 0;
			//Debug.Log("I do not have enough phosphocreatine available, so I will use what is available and now the remainder of the request is " + energyRequired);
		}

		// work out how much energy we are taking from glycogen
		float maximumAvailableEnergyFromGlycogen = maxGlycogenUtilisation * Time.fixedDeltaTime * (1 - 0.9F * lactateSaturation);
		float energyFromGlycogen = Mathf.Min (energyRequired, maximumAvailableEnergyFromGlycogen);
		lactateSaturation += 1F / 900F * Time.fixedDeltaTime * energyFromGlycogen / maximumAvailableEnergyFromGlycogen;
		//Debug.Log ("I can take " + energyFromGlycogen + " kJ from glycogen");

		// work out how much glycogen mass this is
		float massFromGlycogen = energyFromGlycogen / glucoseToEnergyRatio;

		// subtract from glycogen stores and energy required
		if (massFromGlycogen <= glycogenInMuscles) {
			glycogenInMuscles -= massFromGlycogen; // we have enough glycogen to meet the energy requirement
			energyRequired -= energyFromGlycogen;
			//Debug.Log("I have enough glycogen available, so now the remainder of the request is " + energyRequired);
		} else {
			// not enough energy in glycogen stores, so deplete them fully and draw further energy from fat
			energyRequired -= glycogenInMuscles * glucoseToEnergyRatio;
			glycogenInMuscles = 0;
			//Debug.Log("I do not have enough glycogen available, so I will use what is available and now the remainder of the request is " + energyRequired);
		}

		// work out how much energy we are taking from fat
		float energyFromFat = Mathf.Min (energyRequired, maxFatUtilisation * Time.fixedDeltaTime);
		//Debug.Log ("I can take " + energyFromFat + " kJ from fat");

		// work out how much fat mass this is
		float massFromFat = energyFromFat / fatToEnergyRatio;

		// subtract from fat stores and energy required
		if (massFromFat <= fatInBlood) {
			fatInBlood -= massFromFat; // we have enough fat to meet the energy requirement
			energyRequired -= energyFromFat;
			//Debug.Log("I have enough fat available, so now the remainder of the request is " + energyRequired);
		} else {
			// not enough energy in fat stores, so deplete them fully and draw further energy from glucose
			energyRequired -= fatInBlood * fatToEnergyRatio;
			fatInBlood = 0;
			//Debug.Log("I do not have enough fat available, so I will use what is available and now the remainder of the request is " + energyRequired);
		}

		// work out how much energy we are taking from glucose
		float energyFromGlucose = Mathf.Min (energyRequired, maxGlucoseUtilisation * Time.fixedDeltaTime);
		//Debug.Log ("I can take " + energyFromGlucose + " kJ from glucose");

		// work out how much glucose mass this is
		float massFromGlucose = energyFromGlucose / glucoseToEnergyRatio;

		// subtract from glucose stores and energy required
		if (massFromGlucose <= glucoseInBlood) {
			glucoseInBlood -= massFromGlucose; // we have enough glucose to meet the energy requirement
			energyRequired -= energyFromGlucose;
			//Debug.Log("I have enough glucose available, so now the remainder of the request is " + energyRequired);
		} else {
			// not enough energy in glucose stores, so deplete them fully and draw further energy from phosphocreatine
			energyRequired -= glucoseInBlood * glucoseToEnergyRatio;
			glucoseInBlood = 0;
			//Debug.Log("I do not have enough glucose available, so I will use what is available and now the remainder of the request is " + energyRequired);
		}

		if (energyRequired == totalEnergyRequired) {
			//Debug.Log("Using muscle mass");
			float energyFromMuscleMass = Mathf.Min (energyRequired, maxMuscleUtilisation * Time.fixedDeltaTime);
			//Debug.Log("Energy from muscle mass = " + energyFromMuscleMass);
			float massFromMuscle = (energyRequired / proteinToEnergyRatio) * 2;
			muscleMass -= massFromMuscle;
			energyRequired -= energyFromMuscleMass;
		}

		if (energyRequired < 0.001) {
			energyRequired = 0;
		}
		return energyRequired; // return the amount of energy we couldn't provide
	}
}
