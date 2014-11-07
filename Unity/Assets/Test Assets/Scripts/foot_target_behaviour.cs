using UnityEngine;
using System.Collections;

public class foot_target_behaviour : MonoBehaviour {

	public float distanceInFrontWhenWalking = 0.2F;
	public float distanceInFrontWhenRunning = 0.1F;
	public string animationEventName;
	public string currentPlayingAnimation;
	public float stepDownTolerance = 0.1F;
	public float stepUpTolerance = 0.35F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SettleToGround() {
		int layerMask = 1 << 8;
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, layerMask)) {
			//Debug.Log("Found the ground");
			if (hit.distance < (stepDownTolerance + stepUpTolerance) * 2) {
				transform.position += Vector3.down * hit.distance;

				// vector that is orthogonal to hit.normal and vector3.up
				// rotate around this
				transform.RotateAround(transform.position, Vector3.Cross(Vector3.up, hit.normal), Vector3.Angle(hit.normal, Vector3.up));

			}
		}
	}

	public float TimeToLanding () {
		//float currentAnimationTime = 0;
		//float timeToMyEvent = 1;
		//float timeToLoop = 2;

		//if (timeToMyEvent > currentAnimationTime) {
		//	return timeToMyEvent - currentAnimationTime;
		//} else {
		//	return timeToLoop - currentAnimationTime + timeToMyEvent;
		//}
		if (currentPlayingAnimation == "walking") {
			return 0.6F;
		} else if (currentPlayingAnimation == "running") {
			return 0.25F;
		}
		return 1F;
	}
}
