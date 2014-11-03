using UnityEngine;
using System.Collections;

public class CustomPlayerMovementController : MonoBehaviour {

	public float walkSpeed = 3F;
	public float jogSpeed = 6F;
	public float sprintSpeed = 10F;
	public float backwardSpeed = 2F;
	public float acceleration = 10F;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponentInParent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		//float forwardSpeed = Input.GetAxis ("Vertical") * (1 - Input.GetAxis("Walk") * 0.5) * (1 + Input.GetAxis("Sprint"));
		//float sideSpeed = Input.GetAxis ("Horizontal");
		Vector3 movementVector = Vector3.forward * Input.GetAxis ("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
		movementVector.Normalize ();
		movementVector = movementVector * acceleration;
		rb.AddRelativeForce (movementVector);
	}
}
