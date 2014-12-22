using UnityEngine;
using System.Collections;

public class PickupInteraction : MonoBehaviour, IInteraction {

    public void Interact()
    {
        Destroy(gameObject);
    }
}
