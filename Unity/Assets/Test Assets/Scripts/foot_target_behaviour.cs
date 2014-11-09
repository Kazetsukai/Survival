using UnityEngine;
using System.Collections;

public class foot_target_behaviour : MonoBehaviour {

	public float distanceInFrontWhenWalking = 0.2F;
	public float distanceInFrontWhenRunning = 0.1F;
	public string animationEventName;
	public string currentPlayingAnimation;
	public float stepDownTolerance = 0.1F;
	public float stepUpTolerance = 0.35F;
	public GameObject player;
	float objectiveSlopeAngle;
	RaycastHit hit;
	CustomCharacterController cc;
	Camera cam;
	AudioSource sound;
	public AudioClip footstepSound;
	public AudioClip stumbleSound;
	Vector3 vectorToGround = Vector3.zero;
	Animator anim;
	Terrain footstepTerrain;
	bool isGrounded;

	// Use this for initialization
	void Start () {
		cc = player.GetComponent<CustomCharacterController> ();
		sound = GetComponent<AudioSource> ();
		vectorToGround = Vector3.down * cc.GetComponentInParent<CapsuleCollider> ().height / 2F + Vector3.up * cc.stepUpTolerance;
		anim = cc.GetComponentInChildren<Animator> ();
		cam = cc.GetComponentInChildren<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = cc.transform.rotation;
		// vector that is orthogonal to hit.normal and vector3.up
		// rotate around this
		transform.RotateAround(transform.position, Vector3.Cross(Vector3.up, hit.normal), Vector3.Angle(hit.normal, Vector3.up));
	}

	void SettleToGround() {
		int layerMask = 1 << 8;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, layerMask)) {
			if (hit.distance < (stepDownTolerance + stepUpTolerance) * 2) {
				isGrounded = true;
				transform.position += Vector3.down * hit.distance;
				objectiveSlopeAngle = Vector3.Angle (hit.normal, Vector3.up) * Mathf.Deg2Rad;
				footstepTerrain = hit.collider.GetComponentInParent<Terrain>();
			} else {
				isGrounded = false;
			}
		}
	}

	public void HandleStep() {
		if (cc.IsGrounded()) {
			sound.pitch = 1.0f + Random.Range (-0.1f, 0.1f);
			sound.volume = 1.0f + Random.Range (-0.2f, 0.0f);
			sound.PlayOneShot (footstepSound);
			float speedStumbleMultiplier = 1 + Mathf.Pow ((cc.rigidbody.velocity.magnitude / (cc.jogSpeed * cc.sprintSpeedFactor)), 5);
			float angleStumbleChance = 1000;//Mathf.Max (1, footstepTerrain.SlopeStumbleConstant() - footstepTerrain.SlopeStumbleCoefficient() * Mathf.Pow (ObjectiveSlopeAngleDeg (), footstepTerrain.SlopeStumbleExponent()));
			float lookingAtFeetMultiplier = Mathf.Min (Mathf.Max (5.5F - 0.05F * Vector3.Angle (Vector3.down, cam.transform.forward), 0.25F), 4F);
			if (Random.Range (0.0f, 1.0f) < (speedStumbleMultiplier / (angleStumbleChance * lookingAtFeetMultiplier))) {
				sound.pitch = 1.0f;
				sound.volume = 1.0f;
				sound.PlayOneShot (stumbleSound);
				cc.Stumble();
			} else {
				sound.pitch = 1.0f + Random.Range (-0.1f, 0.1f);
				sound.volume = 1.0f + Random.Range (-0.2f, 0.0f);
				sound.PlayOneShot (footstepSound);
				cc.StableStep();
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
	public float ObjectiveSlopeAngleRad() {
		return objectiveSlopeAngle;
	}
	
	public float ObjectiveSlopeAngleDeg() {
		return objectiveSlopeAngle * Mathf.Rad2Deg;
	}

	public void DetermineTarget() {
		transform.position = cc.rigidbody.transform.position + vectorToGround + cc.rigidbody.transform.forward * (currentPlayingAnimation == "running" ? distanceInFrontWhenRunning : distanceInFrontWhenWalking) + cc.rigidbody.velocity * TimeToLanding() / anim.speed;
		SettleToGround ();

	}

	public bool IsGrounded() {
		return isGrounded;
	}
}
