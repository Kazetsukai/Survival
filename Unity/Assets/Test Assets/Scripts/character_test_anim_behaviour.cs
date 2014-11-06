using UnityEngine;
using System.Collections;

public class character_test_anim_behaviour : MonoBehaviour 
{
	public AudioClip footstepSound;
	public AudioClip stumbleSound;

	public float stumbleChanceOneIn;
	public float angleMultiplier;
	CustomCharacterController cc;
		
	Animator anim;
	public float animSpeedFactorWalking = 0.5f;
	public float animSpeedFactorRunning = 0.3f;
	public float speedThresholdRun = 2.5f;

	void Start () 
	{
		anim = GetComponent<Animator> ();
		cc = GetComponentInParent<CustomCharacterController> ();

	}

	float PlayerSpeed()
	{
		return cc.rigidbody.velocity.magnitude;
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
}