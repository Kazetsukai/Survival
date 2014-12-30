using UnityEngine;
using System.Collections;

public class UpperbodyLeanBehaviour : MonoBehaviour 
{
	/// <summary>
	/// This object must have a rigidbody component.
	/// </summary>
	public GameObject playerRbObject;

	/// <summary>
	/// The movement direction as defined by the player input axes, NOT the player velocity.
	/// </summary>
	float inputDirection;

	/// <summary>
	/// The direction of the leanTarget object to the leanGoal (which is set by inputDirection). 
	/// The leanTarget object transitions from its current position to the goalLocalPosition always.
	/// </summary>
	float goalDirection;

	/// <summary>
	/// The amount by which the player will lean. Larger values create a greater lean.
	/// Different directions of lean have different amounts of lean.
	/// </summary>
	public float leanAmountForward = 0.2f;
	public float leanAmountBackward = 0.15f;
	public float leanAmountSideways = 0.175f;

	/// <summary>
	/// The speed at which the leanTarget will move to the goalLocalPosition.
	/// </summary>
	public float leanSpeedFactor = 0.04f;

	/// <summary>
	/// The starting position of the leanTarget object. When there is no input from player, the leanTarget will transition back to this position.
	/// </summary>
	Vector3 startLocalPosition; 

	/// <summary>
	/// The position of the leanTarget goal. The leanTarget object will transition to this position when the player is moving.
	/// </summary>
	Vector3 goalLocalPosition;

	//DEBUGGING graphics:
	public GameObject goal;

	void Start () 
	{
		//Record the starting position
		startLocalPosition = transform.localPosition;
	}

	void Update () 
	{
		//Get player input
		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");
		inputDirection = Mathf.Atan2(inputY, inputX);

		//Set goal position based on inputDirection
		if (inputY > 0) 		//Moving forwards
		{
			goalLocalPosition = new Vector3 (leanAmountSideways * Mathf.Cos (inputDirection), transform.localPosition.y, leanAmountForward * Mathf.Sin (inputDirection));
		}
		else if (inputY < 0) 	//Moving backwards
		{
			goalLocalPosition = new Vector3 (leanAmountSideways * Mathf.Cos (inputDirection), transform.localPosition.y, leanAmountBackward * Mathf.Sin (inputDirection));
		}
		else 					//Strafing only
		{
			goalLocalPosition = new Vector3 (leanAmountSideways * Mathf.Cos (inputDirection), transform.localPosition.y, leanAmountForward * Mathf.Sin (inputDirection));
		}
		//If there is no player input, return lean to upright position
		if ((inputX == 0) && (inputY == 0)) 
		{
			goalLocalPosition = startLocalPosition;
		}

		//Transition leanTarget to goal location
		transform.localPosition += (goalLocalPosition - transform.localPosition) * leanSpeedFactor;

		//Debugging
		goal.transform.localPosition = goalLocalPosition;			
	}
}
