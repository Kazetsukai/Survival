using UnityEngine;
using System.Collections;

public class Terrain : MonoBehaviour {

	float slopeStumbleConstant = 800F;
	float slopeStumbleCoefficient = 0.045F;
	float slopeStumbleExponent = 3F;
	public string terrainType;

	// Use this for initialization
	void Start () {
		switch(terrainType) {
		case "Gravel":
			slopeStumbleConstant = 800F;
			slopeStumbleCoefficient = 0.045F;
			slopeStumbleExponent = 3F;
			break;
		case "Grass":
			slopeStumbleConstant = 900F;
			slopeStumbleCoefficient = 0.007F;
			slopeStumbleExponent = 3F;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float SlopeStumbleConstant() {
		return slopeStumbleConstant;
	}

	public float SlopeStumbleCoefficient() {
		return slopeStumbleCoefficient;
	}

	public float SlopeStumbleExponent() {
		return slopeStumbleExponent;
	}

}
