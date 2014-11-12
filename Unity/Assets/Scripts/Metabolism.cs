using UnityEngine;
using System.Collections;

public class Metabolism : MonoBehaviour {

	public float waterInStomach = 200F;
	public float sugarInStomach = 90F;
	public float proteinInStomach = 120F;
	public float fatInStomach = 80F;
	public float fibreInStomach = 50F;
	public float waterInBody = 44000F;
	public float sugarInBlood = 90F;
	public float proteinInBlood = 120F;
	public float fatInBlood = 50F;

	float timeCompression = 15F;
	float restingWaterLoss = 1600F / (24F * 60F * 60F);
	float waterDigestionRate = 100F / 60F;
	float sugarDigestionRate = 50F / 60F;
	float proteinDigestionRate = 20F / 60F;
	float fatDigestionRate = 10F / 60F;
	float fibreDigestionRate = 30F / 50F;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		if (waterInBody > 0) {
			waterInBody -= Time.fixedDeltaTime * restingWaterLoss * timeCompression;
		}
		if (waterInStomach > 0) {
			waterInStomach -= Time.fixedDeltaTime * waterDigestionRate * timeCompression;
			waterInBody += Time.fixedDeltaTime * waterDigestionRate * timeCompression;
		}
		if (sugarInStomach > 0) {
			sugarInStomach -= Time.fixedDeltaTime * sugarDigestionRate * timeCompression;
			sugarInBlood += Time.fixedDeltaTime * sugarDigestionRate * timeCompression;
		}
		if (proteinInStomach > 0) {
			proteinInStomach -= Time.fixedDeltaTime * proteinDigestionRate * timeCompression;
			proteinInBlood += Time.fixedDeltaTime * proteinDigestionRate * timeCompression;
		}
		if (fatInStomach > 0) {
			fatInStomach -= Time.fixedDeltaTime * fatDigestionRate * timeCompression;
			fatInBlood += Time.fixedDeltaTime * fatDigestionRate * timeCompression;
		}
		if (fibreInStomach > 0) {
			fibreInStomach -= Time.fixedDeltaTime * fibreDigestionRate * timeCompression;
		}
	}
}
