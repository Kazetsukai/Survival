using UnityEngine;
using System.Collections;

public class CarryableItem : MonoBehaviour, IInteraction {

    public Inventory PlayerInventory
    {
        get { return Player.CurrentPlayer.GetComponent<Inventory>(); }
    }

    public void Interact()
    {
        PlayerInventory.CarryItem(this);

        if (collider != null)
            collider.enabled = false;

        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
        }

        transform.localScale = Vector3.one * 10;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }


}
