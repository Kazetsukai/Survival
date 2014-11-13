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
		
		Ragdoll.SetActive(true);
		
		// Set all bones to same position as animated character
		CopyPosition(CharacterAvatarObject.transform, Ragdoll.transform);
		
		CharacterAvatarObject.SetActive(false);
		
		DeathCamera.GetComponent<Camera> ().enabled = true;
		DeathCamera.GetComponent<AudioListener> ().enabled = true;
		
		foreach (var behaviour in GetComponents<MonoBehaviour>()) {
			behaviour.enabled = false;
		}
	}
	
	public void CopyPosition(Transform fromTransform, Transform toTransform)
	{
		toTransform.position = fromTransform.position;
		toTransform.rotation = fromTransform.rotation;
		
		foreach (Transform child in fromTransform)
		{
			foreach (Transform ragChild in toTransform)
			{
				if (ragChild.gameObject.name == child.gameObject.name)
				{
					CopyPosition (child, ragChild);
				}
			}
		}
	}
}
