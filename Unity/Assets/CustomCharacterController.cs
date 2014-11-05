using UnityEngine;
using System.Collections;

public class CustomCharacterController : MonoBehaviour {
	
	public float walkSpeedFactor = 0.7F;
	public float jogSpeed = 4F;
	public float sprintSpeedFactor = 1.3F;
	public float acceleration = 10F;
	public float stepDownTolerance = 0.5F;
	public float stumbleOneInBase = 900F;
	public float stumbleDegAngleMultiplier = 20F;
	float height = 0.92F;
	float altitude = 0;
	float slopeAngle = 0F;
	public float slopeSpeedFactor = 64F;
	public float slopeAngleFactor = 45F;
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
		Physics.Raycast(transform.position, Vector3.down, out hit, 15, layerMask);
		altitude = Mathf.Max(hit.distance - height, 0);
		//Debug.Log (altitude);
		
		wasGrounded = isGrounded;
		isGrounded = IsGrounded ();
		Debug.Log (isGrounded);


		slopeAngle = Vector3.Angle (hit.normal, transform.up) * Mathf.Deg2Rad;
		float slopeSpeedModifier = 1F - Mathf.Pow (slopeAngle / slopeAngleFactor, 2);
		if (isGrounded) {
			rb.useGravity = false;
			Vector3 movementVector = transform.forward * (int)Input.GetAxis ("Vertical") + transform.right * (int)Input.GetAxis ("Horizontal");
			Vector3 targetVector = -rb.velocity * 3;
			if (movementVector != Vector3.zero) {
				targetVector = movementVector.normalized * jogSpeed * (Input.GetAxis ("Vertical") > 0 ? (Input.GetAxis ("Walk") == 1F ? walkSpeedFactor : 1 * Input.GetAxis ("Sprint") == 1 ? sprintSpeedFactor : 1) : walkSpeedFactor) * slopeSpeedModifier;
			}
			Vector3 currentVelocity = rigidbody.velocity;
			currentVelocity.y = 0F;
			Vector3 accelerationVector = (targetVector - currentVelocity);
			accelerationVector = Vector3.ClampMagnitude (accelerationVector, acceleration);// * (1 + slopeAngle / slopeAngleFactor);
			
			if (slopeAngle != 0) {
				float theta = Vector3.Angle (hit.normal, Vector3.Cross (accelerationVector, Vector3.Cross (hit.normal, accelerationVector)));
				theta *= Mathf.Deg2Rad;
				
				Vector3 v = accelerationVector;
				Vector3 k = -transform.right;
				
				Vector3 vRot = v * Mathf.Cos (theta) + (Vector3.Cross (k, v)) * Mathf.Sin (theta) + k * (Vector3.Dot (k, v)) * (1 - Mathf.Cos (theta));
				rb.AddForce (vRot);
			} else {
				rb.AddForce(accelerationVector);
			}
			steppingDown = false;
			//Debug.DrawRay (transform.position, targetVector, Color.red);
			//Debug.DrawRay (transform.position, rb.velocity, Color.green);
			//Debug.DrawRay (transform.position + rb.velocity, targetVector - rb.velocity, Color.blue);

		} else if ((wasGrounded && altitude < stepDownTolerance) || steppingDown && altitude < stepDownTolerance) {
			//rb.useGravity = true;
			//transform.Translate(Vector3.down * altitude);
			//rb.velocity += Vector3.down * altitude;
			transform.position -= Vector3.up * altitude;
			Vector3 newVelocityVector = Vector3.Cross(hit.normal, transform.right);
			float newVelocityMagnitude = Vector3.Dot(newVelocityVector, rb.velocity);
			//Debug.DrawRay (transform.position, rb.velocity, Color.green);

			rb.velocity = newVelocityVector * newVelocityMagnitude;
			//Debug.DrawRay (transform.position, rb.velocity, Color.red);

			steppingDown = true;
		} else {
			rb.useGravity = true;
			steppingDown = false;
		}
	}
	
	public bool IsGrounded() {
		return terrainCollisionCount > 0 || steppingDown;
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

	public float SlopeAngleRad() {
		return slopeAngle;
	}

	public float SlopeAngleDeg() {
		return slopeAngle * Mathf.Rad2Deg;
	}

}


