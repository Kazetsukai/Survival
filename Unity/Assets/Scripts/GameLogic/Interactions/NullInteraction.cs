using UnityEngine;
using System.Collections;

public class NullInteraction : MonoBehaviour, IInteraction {

	public void Interact ()
	{
        Debug.Log("Interacted with " + name);
	}

}
