using UnityEngine;
using System.Collections;

public class Death : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Die()
	{
		GameObject.Find ("Main Camera").GetComponent<Camera> ().enabled = false;
		GameObject.Find ("Main Camera").GetComponent<AudioListener> ().enabled = false;

		GetComponent<BoxCollider>().enabled = true;

		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		rigidbody.freezeRotation = false;
		rigidbody.AddForceAtPosition (GetComponent<CharacterController> ().velocity, rigidbody.transform.position + Vector3.up * 10);
		GetComponent<CharacterController>().enabled = false;

		GameObject.Find ("DeathCamera").GetComponent<Camera> ().enabled = true;
		GameObject.Find ("Main Camera").GetComponent<AudioListener> ().enabled = true;
		
		foreach (var behaviour in GetComponents<MonoBehaviour>()) {
			behaviour.enabled = false;
		}
	}
}
