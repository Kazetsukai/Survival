using UnityEngine;
using System.Collections;

/// <summary>
/// Converts a GameObject to a new GameObject with RigidBody and CarryableItem script.
/// </summary>
public class ConvertToItemInteraction : MonoBehaviour, IInteraction 
{
	public void Interact()
	{
		//Spawn copy of this object in same place with same rotation
		this.gameObject.AddComponent<Rigidbody> ();
		this.gameObject.AddComponent<CarryableItem> ();
		this.name = this.name + "_asItem";
		Destroy (this);				//Remove this script
	}
}
