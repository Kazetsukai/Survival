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
	public float maximumSlope = 50F;
	public bool isStumbling = false;
	float altitude = 0;
	float objectiveSlopeAngle = 0F;
	float relativeSlopeAngle = 0F;
	Rigidbody rb;
	bool isGrounded = false;
	bool wasGrounded = true;
	int terrainCollisionCount = 0;
	RaycastHit hit;
	CapsuleCollider cc;
	Vector3 _movementVector = Vector3.zero;

	// Use this for initialization
	void Start () {
		rb = GetComponentInParent<Rigidbody> ();
		cc = GetComponent<CapsuleCollider> ();
	}
	
	// Update is called once per frame
	void Update () {
		_movementVector = transform.forward * (int)Input.GetAxis ("Vertical") + transform.right * (int)Input.GetAxis ("Horizontal");
		Debug.Log (_movementVector);
	}

	void OnGUI() {
		if (isStumbling) {
			GUI.Box (new Rect (Screen.width /2 - 35, Screen.height /2 - 10, 70, 20), "STUMBLE");
		}
	}

	void FixedUpdate() {
		//Debug.Log ("Velocity normal");
		//Debug.Log (terrainCollisionCount);
		// work out the bottom of the player
		Vector3 bottomOfCapsule = transform.position - Vector3.up * cc.height / 2;

		// raycast down to find the terrain below
		int layerMask = 1 << 8; // terrain layer
		Physics.Raycast(transform.position, Vector3.down, out hit, 1000, layerMask);

		// now that we have the normal of the terrain, work out the point on the capsule collider that is closest to the terrain
		Vector3 newRayCastOrigin = bottomOfCapsule + Vector3.up * cc.radius - hit.normal * cc.radius + Vector3.up * cc.radius; // a small amount up to ensure the raycast doesn't start below the terrain

		// raycast from this new point down to find the altitude
		if (Physics.Raycast (newRayCastOrigin, Vector3.down, out hit, 1000, layerMask)) {
			altitude = Mathf.Max (0, hit.distance - cc.radius); // correcting for the offset added earlier
		} else {
			altitude = 10000;
		}

		// remember the player's previous grounded state
		wasGrounded = isGrounded;

		// work out the new grounded state
		isGrounded = IsGrounded ();

		// if the player has just left the ground but only by a small amount, i.e. has gone over a crease
		if (!isGrounded && wasGrounded && altitude < stepDownTolerance) {
			// move the player down to the ground
			transform.position -= Vector3.up * altitude * 1.1F;

			// adjust the velocity direction to keep the player moving along the surface of the terrain
			Vector3 velocityRightVector = Vector3.Cross(hit.normal, rb.velocity);
			Vector3 newVelocityVector = -Vector3.Cross(hit.normal, velocityRightVector);
			
			// adjust the velocity magnitude back to what it was
			rb.velocity = newVelocityVector.normalized * rb.velocity.magnitude;
			//Debug.Log("I am messing with the player's velocity");
			
			// player is now still touching the ground

			isGrounded = true;
			//rb.useGravity = true;
		}

		// work out the actual angle of the sloped terrain
		objectiveSlopeAngle = Vector3.Angle (hit.normal, transform.up) * Mathf.Deg2Rad;

		if (isGrounded) {
			// turn off gravity to prevent collision drag
			rb.useGravity = false;

			// generate new desired direction based on player input
			float relativeSlopeSpeedMultiplier = 1F;

			if (_movementVector != Vector3.zero) {

				// if the player is standing on a slope, we need to re-orient the desired direction
				if (objectiveSlopeAngle != 0) {
					// manipulate the movementVector until it is pointing up or down the slope
					_movementVector = Vector3.Cross(Vector3.Cross (hit.normal, _movementVector), hit.normal);

					// calculate the relativeSlopeAngle
					Vector3 lateralMovementVector =  _movementVector;
					lateralMovementVector.y = 0F;
					relativeSlopeAngle = Vector3.Angle(lateralMovementVector, _movementVector) * Mathf.Deg2Rad;

					// use the relativeSlopeAngle to work out relativeSlopeSpeedMultiplier
					relativeSlopeSpeedMultiplier = 1 - Mathf.Min(Mathf.Pow ((_movementVector.y < 0 ? -RelativeSlopeAngleDeg() : RelativeSlopeAngleDeg()) / maximumSlope, 2), 1);

				}
				// scale the vector to the speed as determined by the player inputs and the relativeSlopeSpeedMultiplier
				_movementVector = _movementVector.normalized * jogSpeed;
				if (Input.GetAxis ("Vertical") > 0) {
					if (Input.GetAxis ("Walk") > 0) {
						_movementVector = _movementVector * walkSpeedFactor;
					}
					if (Input.GetAxis ("Sprint") > 0) {
						_movementVector = _movementVector * sprintSpeedFactor;
					}
				} else {
					_movementVector = _movementVector * walkSpeedFactor;
				}
				if (_movementVector.y > 0) {
					_movementVector = _movementVector * relativeSlopeSpeedMultiplier;
				} else {
					_movementVector = _movementVector / relativeSlopeSpeedMultiplier;
				}
			}
			
			// now we have the movementVector pointing in the right direction and at the right magnitude
			// next is to work out the accelerationVector required to move the player in this new direction
			Vector3 accelerationVector = (_movementVector - rb.velocity);

			// now we scale the accelerationVector to the acceleration value
			accelerationVector = Vector3.ClampMagnitude(accelerationVector, acceleration * Time.fixedDeltaTime);

			// and apply the accelerationVector
			rb.AddForce(accelerationVector, ForceMode.VelocityChange);
			
			// we can't let people walk up slopes that are tpp steep just by walking sideways
			if (ObjectiveSlopeAngleDeg() > maximumSlope) {
				accelerationVector.y = Mathf.Min(0, accelerationVector.y);
				if (rb.velocity.y > 0) {
					Vector3 newVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
					rb.velocity = newVelocity;
					//Debug.Log("I am stopping the player from walking up a cliff");
				}
			}
			
			//Debug.DrawRay(transform.position, rb.velocity, Color.green);
			//Debug.DrawRay(transform.position, movementVector, Color.red);
			//Debug.DrawRay(transform.position + rb.velocity, accelerationVector, Color.blue);

		} else {
			// player is off the ground, so apply gravity
			rb.useGravity = true;
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

	public float ObjectiveSlopeAngleRad() {
		return objectiveSlopeAngle;
	}

	public float ObjectiveSlopeAngleDeg() {
		return objectiveSlopeAngle * Mathf.Rad2Deg;
	}

	public float RelativeSlopeAngleRad() {
		return relativeSlopeAngle;
	}

	public float RelativeSlopeAngleDeg() {
		return relativeSlopeAngle * Mathf.Rad2Deg;
	}
}


