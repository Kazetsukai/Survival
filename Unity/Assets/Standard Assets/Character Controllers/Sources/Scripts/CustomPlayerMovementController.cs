using UnityEngine;
using System.Collections;

public class CustomPlayerMovementController : MonoBehaviour {

	public float walkSpeedFactor = 0.5F;
	public float jogSpeed = 30F;
	public float sprintSpeedFactor = 1.5F;
	//public float backwardSpeedFactor = 0.3F;
	public float acceleration = 100F;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponentInParent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate() {
		Vector3 movementVector = transform.forward * (int)Input.GetAxis ("Vertical") + transform.right * (int)Input.GetAxis ("Horizontal");
		Vector3 targetVector = movementVector.normalized * jogSpeed * (Input.GetAxis ("Vertical") > 0 ? (Input.GetAxis ("Walk") == 1F ? walkSpeedFactor : 1 * Input.GetAxis("Sprint") == 1 ? sprintSpeedFactor : 1) : walkSpeedFactor);
		Vector3 currentVelocity = rigidbody.velocity;
		currentVelocity.y = 0F;
		Vector3 accelerationVector = (targetVector - currentVelocity);
		accelerationVector = Vector3.ClampMagnitude (accelerationVector, acceleration);
		rb.AddForce (accelerationVector);
	}
}
