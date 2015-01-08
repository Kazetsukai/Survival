using UnityEngine;
using System.Collections;

public class FollowBody : MonoBehaviour {

	public GameObject Target;
	public Vector3 cameraOffset = new Vector3 (2, 2, 2);

	void Update () 
	{
		//Move the camera gently (like spring motion)
		Vector3 cameraGoal = Target.transform.position + cameraOffset;
		Vector3 distanceToGoal = cameraGoal - transform.position;
		transform.position += distanceToGoal * Time.deltaTime;

		transform.LookAt(Target.transform.position);
	}
}
