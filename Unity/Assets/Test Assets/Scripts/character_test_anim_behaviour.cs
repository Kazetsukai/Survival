using UnityEngine;
using System.Collections;

public class character_test_anim_behaviour : MonoBehaviour 
{
	public AudioClip footstepSound;
	public AudioClip stumbleSound;

	CustomCharacterController cc;
    Rigidbody rb;
		
	Animator anim;
	public float animSpeedFactorWalking = 0.5f;
	public float animSpeedFactorRunning = 0.3f;
	public float speedThresholdRun = 2.5f;
	public float framesBetweenStepsWalking = 10;
	public float framesBetweenStepsRunning = 8;
	foot_target_behaviour lf;
	foot_target_behaviour rf;

	void Start () 
	{
		anim = GetComponent<Animator> ();
		cc = GetComponentInParent<CustomCharacterController> ();
        rb = GetComponentInParent<Rigidbody> ();
		lf = cc.leftFoot.GetComponent<foot_target_behaviour> ();
		rf = cc.rightFoot.GetComponent<foot_target_behaviour> ();
	}

	float PlayerSpeed()
	{
		return rb.velocity.magnitude;
	}

	void Update () 
	{
        //Rough code to change animation state for debug legs. Makes them run if the player presses forward button.
		//float jump = Input.GetAxis ("Jump");

		//Debug.Log ("Player speed: " + PlayerSpeed().ToString ());
		//anim.bodyRotation.SetLookRotation (cc.rigidbody.velocity, Vector3.up);


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
		anim.speed = PlayerSpeed () * animSpeedFactorWalking;
	}

	void AnimSetRunning()
	{
		anim.SetBool ("walking", false);
		anim.SetBool ("running", true);
		anim.speed = PlayerSpeed () * animSpeedFactorRunning;
	}

	public void PlayFootstep()
	{
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
		}
	}

	public void Event_LeftFootLift() {
		//cc.leftFoot.transform.position = rb.transform.position + rb.velocity / 7 + Vector3.down * cc.GetComponentInParent<CapsuleCollider>().height / 2.2F - rb.transform.right;
        //Debug.Log("Lifting left foot");
	}

	public void Event_RightFootLift() {
		cc.rightFoot.transform.position = rb.transform.position + rb.transform.forward * rf.distanceInFrontWhenLandingRunning;
        //Debug.Log("Lifting right foot");
	}

	public void Event_LeftFootLand() {
		HandleFootStep ();
	}
	
	public void Event_RightFootLand() {
		HandleFootStep ();
	}

	public void NewEvent() {
	}
}