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
    }


}
