using UnityEngine;
using System.Collections;

public class DigestionHandler : MonoBehaviour {

	//public Metabolism;
	public float waterInStomach = 0F;
	public float sugarInStomach = 0F;
	public float proteinInStomach = 0F;
	public float fatInStomach = 0F;

	float timeCompression = 15F;

	// Use this for initialization
	void Start () {
		new WaitForSeconds (1);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
