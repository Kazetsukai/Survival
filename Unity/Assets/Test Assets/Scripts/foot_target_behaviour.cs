using UnityEngine;
using System.Collections;

public class foot_target_behaviour : MonoBehaviour {

	public float distanceInFrontWhenWalking = 0.2F;
	public float distanceInFrontWhenRunning = 0.1F;
	public AnimationEvent landingEvent;
	public float timeToLanding = 0.5F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float TimeToLanding () {
		return timeToLanding;
	}
}
