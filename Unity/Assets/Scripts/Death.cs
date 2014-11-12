using UnityEngine;
using System.Collections;

public class Death : MonoBehaviour {

	public GameObject Ragdoll;
	public GameObject MainCamera;
	public GameObject DeathCamera;
	public GameObject CharacterAvatarObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.KeypadEnter))
			Die ();
	}

	public void Die()
	{
		MainCamera.GetComponent<Camera> ().enabled = false;
		MainCamera.GetComponent<AudioListener> ().enabled = false;

		GetComponent<CapsuleCollider>().enabled = false;

		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		rigidbody.freezeRotation = true;
		
		CharacterAvatarObject.SetActive(false);
		Ragdoll.SetActive(true);
		
		Ragdoll.transform.position = CharacterAvatarObject.transform.position;
		
		// Set all bones to same position as animated character
		foreach (Transform child in CharacterAvatarObject.transform)
		{
			foreach (Transform ragChild in Ragdoll.transform)
			{
				if (ragChild.gameObject.name == child.gameObject.name)
				{
					ragChild.position = child.position;
					break;
				}
			}
		}

		DeathCamera.GetComponent<Camera> ().enabled = true;
		DeathCamera.GetComponent<AudioListener> ().enabled = true;
		
		foreach (var behaviour in GetComponents<MonoBehaviour>()) {
			behaviour.enabled = false;
		}
	}
}
