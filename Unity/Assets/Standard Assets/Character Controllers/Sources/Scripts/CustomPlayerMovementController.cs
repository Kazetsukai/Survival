using UnityEngine;
using System.Collections;

public class CustomPlayerMovementController : MonoBehaviour {

	public float walkSpeedFactor = 0.5F;
	public float jogSpeed = 30F;
	public float sprintSpeedFactor = 1.5F;
	//public float backwardSpeedFactor = 0.3F;
	public float acceleration = 100F;
	Rigidbody rb;
	bool isGrounded = true;
	RaycastHit hit;

	// Use this for initialization
	void Start () {
		rb = GetComponentInParent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate() {
		isGrounded = GroundedTest ();
		if (isGrounded) {
			Vector3 movementVector = transform.forward * (int)Input.GetAxis ("Vertical") + transform.right * (int)Input.GetAxis ("Horizontal");
			Vector3 targetVector = movementVector.normalized * jogSpeed * (Input.GetAxis ("Vertical") > 0 ? (Input.GetAxis ("Walk") == 1F ? walkSpeedFactor : 1 * Input.GetAxis ("Sprint") == 1 ? sprintSpeedFactor : 1) : walkSpeedFactor);
			Vector3 currentVelocity = rigidbody.velocity;
			currentVelocity.y = 0F;
			Vector3 accelerationVector = (targetVector - currentVelocity);
			accelerationVector = Vector3.ClampMagnitude (accelerationVector, acceleration);

			float theta = Vector3.Angle(hit.normal, Vector3.Cross (accelerationVector, Vector3.Cross(hit.normal, accelerationVector)));
			theta *= Mathf.Deg2Rad;

			Vector3 v = accelerationVector;
			Vector3 k = -transform.right;

			Vector3 vRot = v * Mathf.Cos(theta) + (Vector3.Cross(k, v)) * Mathf.Sin(theta) + k * (Vector3.Dot(k, v)) * (1 - Mathf.Cos(theta));
			rb.AddForce (vRot);

			//Debug.DrawRay (transform.position, accelerationVector, Color.red, 0, true);
			//Debug.DrawRay (transform.position, currentVelocity, Color.green, 0, true);
			//Debug.DrawRay (hit.point, hit.normal, Color.blue, 0, true);
			//Debug.DrawRay (hit.point, Vector3.Cross(hit.normal, accelerationVector), Color.cyan, 0, true);
			//Debug.DrawRay (hit.point, Vector3.Cross (accelerationVector, Vector3.Cross(hit.normal, accelerationVector)), Color.magenta, 0, true);
			//Debug.Log(Vector3.Angle(hit.normal, Vector3.Cross (accelerationVector, Vector3.Cross(hit.normal, accelerationVector))));
			//Debug.DrawRay(hit.point, vRot, Color.red, 0, true);

		}
	}

	public bool IsGrounded() {
		return isGrounded;
	}

	private bool GroundedTest() {
		int layerMask = 1 << 8;
		return Physics.Raycast (transform.position, Vector3.down, out hit, 2, layerMask);
	}
}
