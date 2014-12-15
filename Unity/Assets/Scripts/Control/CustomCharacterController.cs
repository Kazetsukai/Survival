using UnityEngine;
using System.Collections;

public class CustomCharacterController : MonoBehaviour {
	
	public float slowWalkSpeed = 0.75F;
	public float fastWalkSpeed = 1.5F;
	public float slowRunSpeed = 3F;
	public float fastRunSpeed = 4.5F;
	public float sprintSpeed = 6F;

	public float slowWalkEnergy = 0.114F;
	public float fastWalkEnergy = 0.4F;
	public float slowRunEnergy = 1F;
	public float fastRunEnergy = 3F;
	public float sprintEnergy = 7.6F;

	public float acceleration = 10F;
	public float stepDownTolerance = 0.5F;
	public float stepUpTolerance = 0.5F;
	public float maximumSlope = 50F;
	public float stumbleSpeedFactor = 2F/3F;
	public float stumbleSpeedExponent = 4F;

	Metabolism metabolism;

	public GameObject leftFoot;
	public GameObject rightFoot;
	FootTargetBehaviour lf;
	FootTargetBehaviour rf;

	[HideInInspector]
	public bool walkingSlow = false;
	[HideInInspector]
	public bool walkingFast = false;
	[HideInInspector]
	public bool runningSlow = false;
	[HideInInspector]
	public bool runningFast = false;
	[HideInInspector]
	public bool sprinting = false;

	float altitude = 0F;
	float desiredSpeed = 0F;
	float energyRequired = 0F;
	float objectiveSlopeAngle = 0F;
	float relativeSlopeAngle = 0F;
	Rigidbody rb;
	bool isGrounded = false;
	bool wasGrounded = true;
	int terrainCollisionCount = 0;
	RaycastHit hit;
	CapsuleCollider cc;
	Vector3 _movementVector = Vector3.zero;
	int stumbleSteps = 0;
	Camera cam;

	// Use this for initialization
	void Start () {
		rb = GetComponentInParent<Rigidbody> ();
		cc = GetComponent<CapsuleCollider> ();
		lf = leftFoot.GetComponent<FootTargetBehaviour> ();
		rf = rightFoot.GetComponent<FootTargetBehaviour> ();
		metabolism = GetComponentInParent<Metabolism> ();
	}
	
	// Update is called once per frame
	void Update () {
		walkingSlow = false;
		walkingFast = false;
		runningSlow = false;
		runningFast = false;
		sprinting = false;
	

		if (stumbleSteps < 1) { // can't control direction when stumbling
			if (Input.GetAxis("Run") > 0) {
				_movementVector = transform.forward; // no strafing when running
				if (Input.GetAxis("Fast") > 0) {
					sprinting = true; // pushing run and fast, sprinting
					desiredSpeed = sprintSpeed;
					energyRequired = sprintEnergy;

					//Debug.Log("Sprinting");
				} else if (Input.GetAxis("Slow") > 0) {
					runningSlow = true; // pushing run and slow, running slow
					desiredSpeed = slowRunSpeed;
					energyRequired = slowRunEnergy;
					//Debug.Log("Running slow");
				} else {
					runningFast = true; // pushing run alone, running fast
					desiredSpeed = fastRunSpeed;
					energyRequired = fastRunEnergy;
					//Debug.Log("Running fast");
				}
			} else if (Input.GetAxis("Vertical") > 0) {
				if (Input.GetAxis("Fast") > 0) {
					_movementVector = transform.forward; // no strafing when running
					runningSlow = true; // pushing forward and fast, running slow
					desiredSpeed = slowRunSpeed;
					energyRequired = slowRunEnergy;
					//Debug.Log("Running slow");
				} else if (Input.GetAxis("Slow") > 0) {
					_movementVector = transform.forward + transform.right * Input.GetAxis ("Horizontal"); // not running, strafing
					walkingSlow = true; // pushing forward and slow, walking slow
					desiredSpeed = slowWalkSpeed;
					energyRequired = slowWalkEnergy;
					//Debug.Log("Walking slow");
				} else {
					_movementVector = transform.forward + transform.right * Input.GetAxis ("Horizontal"); // not running, strafing
					walkingFast = true; // pushing forward alone, walking fast
					desiredSpeed = fastWalkSpeed;
					energyRequired = fastWalkEnergy;
					//Debug.Log("Walking fast");
				}
			} else {
				_movementVector = transform.forward * Input.GetAxis ("Vertical") + transform.right * Input.GetAxis ("Horizontal");
				if (_movementVector != Vector3.zero) {
					if (Input.GetAxis("Slow") > 0) {
						walkingSlow = true;
						desiredSpeed = slowWalkSpeed;
						energyRequired = slowWalkEnergy;
						//Debug.Log("Walking slow");
					} else {
						walkingFast = true;
						desiredSpeed = fastWalkSpeed;
						energyRequired = fastWalkEnergy;
						//Debug.Log("Walking fast");
					}
				}
			}
		}
	}



	void OnGUI() {
	}

	void FixedUpdate() {

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
			// turn off gravity to prevent collision drag unless stumbling
			if (stumbleSteps < 1) {
				rb.useGravity = false;
			} else {
				rb.useGravity = true;
			}

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
					relativeSlopeSpeedMultiplier = 1 - Mathf.Min(Mathf.Pow ((_movementVector.y < 0 ? -RelativeSlopeAngleDeg() : RelativeSlopeAngleDeg()) / maximumSlope, 2), 0.999F);

				}
				// scale the vector to the speed as determined by the player inputs and the relativeSlopeSpeedMultiplier
				_movementVector = _movementVector.normalized;
				float maxSpeed = sprintSpeed * metabolism.muscleMass / metabolism.muscleMassMax; // might make this hyperbolic at some stage
				if (maxSpeed < desiredSpeed) {
					//Debug.Log ("Can't move as fast as desired. desiredSpeed= " + desiredSpeed + " maxSpeed= " + maxSpeed);
					if (maxSpeed < fastRunSpeed) {
						if (maxSpeed < slowRunSpeed) {
							if (maxSpeed < fastWalkSpeed) {
								if (maxSpeed < slowWalkSpeed) {
									//Debug.Log ("I have to go slower than walking slow");
									energyRequired = slowWalkEnergy * maxSpeed / slowWalkSpeed;
									desiredSpeed = slowWalkSpeed * maxSpeed / slowWalkSpeed;
								} else {
									//Debug.Log ("I can still walk slow");
									energyRequired = fastWalkEnergy * maxSpeed / fastWalkSpeed;
									desiredSpeed = fastWalkSpeed * maxSpeed / fastWalkSpeed;
								}
							} else {
								//Debug.Log ("I can still run slow");
								energyRequired = slowRunEnergy * maxSpeed / slowRunSpeed;
								desiredSpeed = slowRunSpeed * maxSpeed / slowRunSpeed;
							}
						} else {
							//Debug.Log ("I can still run fast");
							energyRequired = fastRunEnergy * maxSpeed / fastRunSpeed;
							desiredSpeed = fastRunSpeed * maxSpeed / fastRunSpeed;
						}
					} else {
						//Debug.Log ("I can still sprint slowly");
						energyRequired = sprintEnergy * maxSpeed / sprintSpeed;
						desiredSpeed = sprintSpeed * maxSpeed / sprintSpeed;
					}
				}

				float unavailableEnergy = metabolism.DrawEnergy(energyRequired * Time.fixedDeltaTime);
				//Debug.Log (unavailableEnergy);
				if (unavailableEnergy > 0) {
					float availableEnergy = energyRequired * Time.fixedDeltaTime - unavailableEnergy;
					if (desiredSpeed > fastRunSpeed && availableEnergy > fastRunEnergy * Time.fixedDeltaTime) {
						float excess = availableEnergy - fastRunEnergy * Time.fixedDeltaTime; // how much left we can still go faster by
						_movementVector *= (fastRunSpeed + (excess / (sprintEnergy - fastRunEnergy)) * (sprintSpeed - fastRunSpeed));
					} else if (desiredSpeed > slowRunSpeed && availableEnergy > slowRunEnergy * Time.fixedDeltaTime) {
						float excess = availableEnergy - slowRunEnergy * Time.fixedDeltaTime; // how much left we can still go faster by
						_movementVector *= (slowRunSpeed + (excess / (fastRunEnergy - slowRunEnergy)) * (fastRunSpeed - slowRunSpeed));
					} else if (desiredSpeed > fastWalkSpeed && availableEnergy > fastWalkEnergy * Time.fixedDeltaTime) {
						float excess = availableEnergy - fastWalkEnergy * Time.fixedDeltaTime; // how much left we can still go faster by
						_movementVector *= (fastWalkSpeed + (excess / (slowRunEnergy - fastWalkEnergy)) * (slowRunSpeed - fastWalkSpeed));
					} else if (desiredSpeed > slowWalkSpeed && availableEnergy > slowWalkEnergy * Time.fixedDeltaTime) {
						float excess = availableEnergy - slowWalkEnergy * Time.fixedDeltaTime; // how much left we can still go faster by
						_movementVector *= (slowWalkSpeed + (excess / (fastWalkEnergy - slowWalkEnergy)) * (fastWalkSpeed - slowWalkSpeed));
					} else {
						_movementVector *= slowWalkSpeed * availableEnergy / (slowWalkEnergy * Time.fixedDeltaTime);
					}
				} else {
					_movementVector *= desiredSpeed;
				}
			}

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
					rb.useGravity = true;
				}
			}
			

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
			terrainCollisionCount += 1;
		}
	}
	
	void OnCollisionExit(Collision collision) {
		if (collision.gameObject.layer == 8) {
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

	public void Stumble() {
		if (stumbleSteps < 1) {
			stumbleSteps = 3;
			Vector3 stumbleDirection = transform.right * Random.Range(-slowWalkSpeed, slowWalkSpeed);
			rb.velocity = rb.velocity + stumbleDirection;
			_movementVector = _movementVector + stumbleDirection;
		} else {
			Fall();
		}
	}

	public void Fall() {
		GetComponentInParent<Death> ().Die();
	}

	public void StableStep () {
		if (rb.velocity.magnitude < 1) {
			stumbleSteps = 0;
		} else {
			stumbleSteps--;
		}
	}

	public bool IsStumbling() {
		return stumbleSteps > 0;
	}
}


