using UnityEngine;
using System.Collections;

public class CustomPlayerMovementController : MonoBehaviour {

	public float walkSpeedFactor = 0.5F;
	public float jogSpeed = 30F;
	public float sprintSpeedFactor = 1.5F;
	public float acceleration = 100F;
	public float stepDownTolerance = 0.5F;
	float height = 1.0F;
	float altitude = 0;
	Rigidbody rb;
	bool isGrounded = false;
	bool wasGrounded = true;
	bool steppingDown = false;
	int terrainCollisionCount = 0;
	RaycastHit hit;

	// Use this for initialization
	void Start () {
		rb = GetComponentInParent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate() {
		int layerMask = 1 << 8;
		Physics.Raycast(transform.position, Vector3.down, out hit, height, layerMask);
		altitude = Mathf.Max(hit.distance - height, 0);

		wasGrounded = isGrounded;
		isGrounded = IsGrounded ();

		if (isGrounded) {
			//Debug.Log("Grounded");
			Vector3 movementVector = transform.forward * (int)Input.GetAxis ("Vertical") + transform.right * (int)Input.GetAxis ("Horizontal");
			Vector3 targetVector = movementVector.normalized * jogSpeed * (Input.GetAxis ("Vertical") > 0 ? (Input.GetAxis ("Walk") == 1F ? walkSpeedFactor : 1 * Input.GetAxis ("Sprint") == 1 ? sprintSpeedFactor : 1) : walkSpeedFactor);
			Vector3 currentVelocity = rigidbody.velocity;
			currentVelocity.y = 0F;
			Vector3 accelerationVector = (targetVector - currentVelocity);
			accelerationVector = Vector3.ClampMagnitude (accelerationVector, acceleration);

			float theta = Vector3.Angle (hit.normal, Vector3.Cross (accelerationVector, Vector3.Cross (hit.normal, accelerationVector)));
			theta *= Mathf.Deg2Rad;

			Vector3 v = accelerationVector;
			Vector3 k = -transform.right;

			Vector3 vRot = v * Mathf.Cos (theta) + (Vector3.Cross (k, v)) * Mathf.Sin (theta) + k * (Vector3.Dot (k, v)) * (1 - Mathf.Cos (theta));
			rb.AddForce (vRot);
			steppingDown = false;

		} else if ((wasGrounded && altitude < stepDownTolerance) || steppingDown && altitude < stepDownTolerance) {
			//Debug.Log("Stepping down");
			transform.Translate(Vector3.down * altitude);
			rb.velocity += Vector3.down * altitude;
			steppingDown = true;
		} else {
			//Debug.Log ("Not stepping down");
			steppingDown = false;
		}
	}

	public bool IsGrounded() {
		return terrainCollisionCount > 0;
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.layer == 8) {
			//Debug.Log("Hit the ground");
			terrainCollisionCount += 1;
		}
	}

	void OnCollisionExit(Collision collision) {
		if (collision.gameObject.layer == 8) {
			//Debug.Log("Left the ground");
			terrainCollisionCount -= 1;
		}

	}
}


