using UnityEngine;
using System.Collections;

public class character_test_anim_behaviour : MonoBehaviour 
{
	public AudioClip footstepSound;
	public AudioClip stumbleSound;

	CustomCharacterController cc;
    Rigidbody rb;
		
	Animator anim;
	public float walkAnimSpeedThreshold = 0.01f; //When player speed becomes larger than this value, walking animation will start (removes noise)
	public float animSpeedFactorWalking = 0.8f;
	public float animSpeedFactorRunning = 0.3f;
	public float runThresholdSpeed = 2.5f;
	public float framesBetweenStepsWalking = 10;
	public float framesBetweenStepsRunning = 8;
	FootTargetBehaviour lf;
	FootTargetBehaviour rf;

	Vector3 previousPosition;

	void Start () 
	{
		anim = GetComponent<Animator> ();
		cc = GetComponentInParent<CustomCharacterController> ();
        rb = GetComponentInParent<Rigidbody> ();
		lf = cc.leftFoot.GetComponent<FootTargetBehaviour> ();
		rf = cc.rightFoot.GetComponent<FootTargetBehaviour> ();
		lf.stepUpTolerance = cc.stepUpTolerance;
		lf.stepDownTolerance = cc.stepDownTolerance;
		rf.stepUpTolerance = cc.stepUpTolerance;
		rf.stepDownTolerance = cc.stepDownTolerance;
	}

	float PlayerSpeed()
	{
		return rb.velocity.magnitude;
	}

	/// <summary>
	/// Determines player speed relative to forward motion. Positive values indicate that the player is moving forward (from their first-person perspective), whereas negative values indicate backwards movement.
	/// </summary>
	/// <returns>Forward-directional speed as signed float.</returns>
	float PlayerForwardSpeed()
	{
		//Use direction variable to determine sign for speed
		float direction = Vector3.Dot(transform.forward, transform.position - previousPosition);

		if (direction < 0) 
		{
			direction = -1f;
		} 
		else 
		{
			direction = 1f;
		}

		return Vector3.Magnitude(new Vector3 (rb.velocity.x, 0, rb.velocity.z)) * direction;
	}

	bool PlayerIsMovingForward()
	{
		if (PlayerForwardSpeed () > walkAnimSpeedThreshold)
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}

	bool PlayerIsMovingBackward()
	{
		if (PlayerForwardSpeed () < -walkAnimSpeedThreshold)
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}

	bool PlayerIsMoving()
	{
		if (PlayerForwardSpeed () < -walkAnimSpeedThreshold)
		{
			return true;
		} 
		if (PlayerForwardSpeed () > walkAnimSpeedThreshold)
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}

	void Update () 
	{
		if (PlayerIsMoving())
		{
			//Play walk animation when player speed is slow
			if ((PlayerForwardSpeed() < runThresholdSpeed) && PlayerIsMovingForward())
			{
				AnimSetWalking();
			} 
			//Player reverse walk animation if player is moving backwards
			if (PlayerIsMovingBackward())
			{
				AnimSetWalkingBackwards();
			}
			//Play run animation when player speed is faster
			if ((PlayerForwardSpeed() >= runThresholdSpeed) && PlayerIsMovingForward())
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

		//Update previousPosition so that forward speed can be determined
		previousPosition = transform.position;
	}

	void AnimSetIdle()
	{
		anim.SetBool ("walking", false);
		anim.SetBool ("walkingBackwards", false);
		anim.SetBool ("running", false);
		anim.speed = 1.0f;
		lf.currentPlayingAnimation = "idle";
		rf.currentPlayingAnimation = "idle";
	}

	void AnimSetWalking()
	{
		anim.SetBool ("walking", true);
		anim.SetBool ("walkingBackwards", false);
		anim.SetBool ("running", false);
		anim.speed = Mathf.Min(PlayerSpeed (), 5F) * animSpeedFactorWalking / (rb.velocity.y != 0 ? (0.001F * Mathf.Pow(cc.RelativeSlopeAngleDeg(), 2) + 1) : 1);
		lf.currentPlayingAnimation = "walking";
		rf.currentPlayingAnimation = "walking";
	}

	void AnimSetWalkingBackwards()
	{
		anim.SetBool ("walking", false);
		anim.SetBool ("walkingBackwards", true);
		anim.SetBool ("running", false);
		anim.speed = Mathf.Min(PlayerSpeed (), 5F) * animSpeedFactorWalking / (rb.velocity.y != 0 ? (0.001F * Mathf.Pow(cc.RelativeSlopeAngleDeg(), 2) + 1) : 1) * 0.4f; //* 0.4f just to slow down anim. Not sure why walkingBackwards animation plays so fast?
		lf.currentPlayingAnimation = "walkingBackwards";
		rf.currentPlayingAnimation = "walkingBackwards";
	}

	void AnimSetRunning()
	{
		anim.SetBool ("walking", false);
		anim.SetBool ("walkingBackwards", false);
		anim.SetBool ("running", true);
		anim.speed = Mathf.Min(PlayerSpeed(), 5F) * animSpeedFactorRunning / (rb.velocity.y != 0 ? (0.0005F * Mathf.Pow(cc.RelativeSlopeAngleDeg(), 2) + 1) : 1);
		lf.currentPlayingAnimation = "running";
		rf.currentPlayingAnimation = "running";
	}

	public void Event_LeftFootLift() {
		lf.DetermineTarget ();
	}

	public void Event_RightFootLift() {
		rf.DetermineTarget ();
	}

	public void Event_LeftFootLand() {
		lf.HandleStep ();
	}
	
	public void Event_RightFootLand() {
		rf.HandleStep ();
	}

	public void Event_LeftFootBack() {
		lf.MoveToBack ();
	}

	public void Event_RightFootBack() {
		rf.MoveToBack ();
	}


	public void PlayFootstep() {}

	public void NewEvent() {}
}