using UnityEngine;
using System.Collections;

public class character_test_anim_behaviour : MonoBehaviour 
{
	public AudioClip footstepSound;
	public AudioClip stumbleSound;

	CustomCharacterController cc;
    Rigidbody rb;
		
	Animator anim;
	public float animSpeedFactorWalking = 0.8f;
	public float animSpeedFactorRunning = 0.3f;
	public float speedThresholdRun = 2.5f;
	public float framesBetweenStepsWalking = 10;
	public float framesBetweenStepsRunning = 8;
	foot_target_behaviour lf;
	foot_target_behaviour rf;

	Vector3 vectorToGround = Vector3.zero;

	void Start () 
	{
		anim = GetComponent<Animator> ();
		cc = GetComponentInParent<CustomCharacterController> ();
        rb = GetComponentInParent<Rigidbody> ();
		lf = cc.leftFoot.GetComponent<foot_target_behaviour> ();
		rf = cc.rightFoot.GetComponent<foot_target_behaviour> ();
		lf.stepUpTolerance = cc.stepUpTolerance;
		lf.stepDownTolerance = cc.stepDownTolerance;
		rf.stepUpTolerance = cc.stepUpTolerance;
		rf.stepDownTolerance = cc.stepDownTolerance;
		vectorToGround = Vector3.down * cc.GetComponentInParent<CapsuleCollider> ().height / 2F + Vector3.up * cc.stepUpTolerance;
	}

	float PlayerSpeed()
	{
		return rb.velocity.magnitude;
	}

	void Update () 
	{


		if (PlayerSpeed() > 0.1f)	//rather than != 0 (just to prevent noise triggering animations)
		{
			//Play walk animation when player speed is slow
			if (PlayerSpeed() < speedThresholdRun) 
			{
				AnimSetWalking();
			} 
			//Play run animation when player speed is faster
			else 
			{
				AnimSetRunning();
			} 
		}
		//Play idle animation when not moving
		else 
		{
			AnimSetIdle();
		}

		//Prevent run animations while falling (maybe even play falling animation??)
		if (!cc.IsGrounded ()) 
		{
			//AnimSetIdle();
		}
	
		//if (jump != 0) 
		//{
		//	anim.SetBool("jump", true);
		//}
		//else
		//{
		//	anim.SetBool("jump", false);
		//}
	}

	void AnimSetIdle()
	{
		anim.SetBool ("walking", false);
		anim.SetBool ("running", false);
		anim.speed = 1.0f;
	}

	void AnimSetWalking()
	{
		anim.SetBool ("walking", true);
		anim.SetBool ("running", false);
		anim.speed = Mathf.Min(PlayerSpeed (), 5F) * animSpeedFactorWalking / (rb.velocity.y > 0 ? (0.0005F * Mathf.Pow(cc.RelativeSlopeAngleDeg(), 2) + 1) : 1);
		lf.currentPlayingAnimation = "walking";
		rf.currentPlayingAnimation = "walking";
	}

	void AnimSetRunning()
	{
		anim.SetBool ("walking", false);
		anim.SetBool ("running", true);
		anim.speed = Mathf.Min(PlayerSpeed(), 5F) * animSpeedFactorRunning;
		lf.currentPlayingAnimation = "running";
		rf.currentPlayingAnimation = "running";
	}

	public void PlayFootstep()
	{
		// deprecated. only left in here to stop runtime complaints

		/*if (cc.IsGrounded()) {
			var audioSource = GetComponentInChildren<AudioSource> ();

			if (Random.Range (0.0f, 1.0f) < 1 / cc.stumbleOneInBase) {
				audioSource.pitch = 1.0f;
				audioSource.volume = 1.0f;
				audioSource.PlayOneShot (stumbleSound);
				cc.isStumbling = true;
			} else {
				audioSource.pitch = 1.0f + Random.Range (-0.1f, 0.1f);
				audioSource.volume = 1.0f + Random.Range (-0.2f, 0.0f);
				audioSource.PlayOneShot (footstepSound);
				cc.isStumbling = false;
			}
		}*/
	}

	void HandleFootStep() {
		if (cc.IsGrounded()) {
			var audioSource = GetComponentInChildren<AudioSource> ();
			float speedStumbleMultiplier = 1 + Mathf.Pow((PlayerSpeed() / (cc.jogSpeed * cc.sprintSpeedFactor)), 5);
            float angleStumbleChance = Mathf.Max (1, cc.slopeStumbleConstant - cc.slopeStumbleCoefficient * Mathf.Pow(cc.ObjectiveSlopeAngleDeg(), cc.slopeStumbleExponent)  );
			Debug.Log(angleStumbleChance);
			if (Random.Range (0.0f, 1.0f) < (speedStumbleMultiplier / angleStumbleChance)) {
				audioSource.pitch = 1.0f;
				audioSource.volume = 1.0f;
				audioSource.PlayOneShot (stumbleSound);
				cc.isStumbling = true;
			} else {
				audioSource.pitch = 1.0f + Random.Range (-0.1f, 0.1f);
				audioSource.volume = 1.0f + Random.Range (-0.2f, 0.0f);
				audioSource.PlayOneShot (footstepSound);
				cc.isStumbling = false;
			}
		}
	}

	public void Event_LeftFootLift() {
		lf.transform.position = rb.transform.position + vectorToGround + rb.transform.forward * lf.distanceInFrontWhenRunning + rb.velocity * lf.TimeToLanding() / anim.speed;
		lf.transform.rotation = cc.transform.rotation;
		lf.SettleToGround ();
	}

	public void Event_RightFootLift() {
		rf.transform.position = rb.transform.position + vectorToGround + rb.transform.forward * rf.distanceInFrontWhenRunning + rb.velocity * rf.TimeToLanding() / anim.speed;
		rf.transform.rotation = cc.transform.rotation;
		rf.SettleToGround ();
	}

	public void Event_LeftFootLand() {
		HandleFootStep ();
	}
	
	public void Event_RightFootLand() {
		HandleFootStep ();
	}

	public void NewEvent() {
		// deprecated. only left in here to stop runtime complaints
	}
}