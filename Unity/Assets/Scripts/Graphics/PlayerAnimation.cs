using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour 
{
	public AudioClip footstepSound;
	public AudioClip stumbleSound;

	CustomCharacterController cc;
    Rigidbody rb;		
	Animator anim;

	public float animSpeedFactorWalking = 0.8f;	 
	public float animSpeedFactorRunning = 0.3f;

	public float animSpeedThresholdWalking = 0.01f; //When player speed becomes larger than this value, walking animation will start (removes noise)
	public float animSpeedThresholdRunning = 2.3f;

	public float framesBetweenStepsWalking = 10;
	public float framesBetweenStepsRunning = 8;

	FootTargetBehaviour lf;
	FootTargetBehaviour rf;

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

	void Update () 
	{
		//Update the forward and strafing velocity parameters in the blend tree used for base movement animations
		float velocityForward = Vector3.Dot (transform.forward, rb.velocity);
		float velocityStrafe = Vector3.Dot (transform.right, rb.velocity);

		anim.SetFloat ("velForward", velocityForward);
		anim.SetFloat ("velStrafe",  velocityStrafe);

		//Modify animation speed for moving up and down slopes slopes
		//Walking forwards/backwards animation
		if ((velocityForward >= animSpeedThresholdWalking) || (velocityForward <= -animSpeedThresholdWalking))
		{
			anim.speed = Mathf.Min(PlayerSpeed (), 5F) * animSpeedFactorWalking / (rb.velocity.y != 0 ? (0.001F * Mathf.Pow(cc.RelativeSlopeAngleDeg(), 2) + 1) : 1);
			lf.currentPlayingAnimation = "walking";
			rf.currentPlayingAnimation = "walking";
		}

		//Running animation
		if (velocityForward >= animSpeedThresholdRunning) 
		{
			anim.speed = Mathf.Min (PlayerSpeed (), 5F) * animSpeedFactorRunning / (rb.velocity.y != 0 ? (0.0005F * Mathf.Pow (cc.RelativeSlopeAngleDeg (), 2) + 1) : 1);
			lf.currentPlayingAnimation = "running";
			rf.currentPlayingAnimation = "running";
		} 

		//Idle animation
		else if ((velocityForward < animSpeedThresholdWalking) && (velocityForward > -animSpeedThresholdWalking)) 
		{
			lf.currentPlayingAnimation = "idle";
			rf.currentPlayingAnimation = "idle";
		}
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