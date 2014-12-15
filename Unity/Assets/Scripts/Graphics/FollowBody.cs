using UnityEngine;
using System.Collections;

public class FollowBody : MonoBehaviour {

	public GameObject Target;

	
	// Update is called once per frame
	void Update () {
		transform.LookAt(Target.transform.position);
	}
}
