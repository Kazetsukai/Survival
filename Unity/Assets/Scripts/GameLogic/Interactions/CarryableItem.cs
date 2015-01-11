using UnityEngine;
using System.Collections;

public class CarryableItem : MonoBehaviour, IInteraction {

    public bool AllowPickup = true;
    public Vector2 InventoryPosition { get; set; }

    public Inventory PlayerInventory
    {
        get { return Player.CurrentPlayer.GetComponent<Inventory>(); }
    }

    public void Interact()
    {
        if (AllowPickup)
            PlayerInventory.CarryItem(this);
    }


}
