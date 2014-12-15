using UnityEngine;
using System.Collections;

public class WinScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.name == "Player Controller")
		{
			other.gameObject.SendMessage("Die");
			GameObject.Find ("ragdoll").rigidbody.AddForce (Vector3.up * 500, ForceMode.VelocityChange);
		}
	}
}
