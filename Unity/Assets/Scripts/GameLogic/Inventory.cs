using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Control;

public class Inventory : MonoBehaviour 
{
    public MonoBehaviour[] ScriptsToPause = new MonoBehaviour[0];
    public GameObject InventoryScreen;

    List<CarryableItem> _inventory = new List<CarryableItem>();
    private bool _displayInventory;

    public bool CarryItem(CarryableItem item)
    {
        _inventory.Add(item);

        Debug.Log("Carried " + item.name);

        item.transform.parent = InventoryScreen.transform;

        return true;
    }

    void Update()
    {
    }

    public void DropFirstItem()
    {
        if (_inventory.Any())
        {
            var item = _inventory.First();

            item.transform.position = Player.CurrentPlayer.transform.position + Player.CurrentPlayer.transform.forward;
            item.gameObject.SetActive(true);

            if (item.rigidbody != null)
                item.rigidbody.AddForce(Player.CurrentPlayer.transform.forward, ForceMode.VelocityChange);

            _inventory.Remove(item);
        }
    }

    public void ToggleInventoryDisplay()
    {
        if (InventoryScreen != null)
        {
            if (!_displayInventory)
            {
                Debug.Log("Showing inventory");
                SetInventoryState(true);
            }
            else
            {
                Debug.Log("Hiding inventory");
                SetInventoryState(false);
            }
        }
        else
            Debug.LogWarning("No inventory screen object set!");
    }

    private void SetInventoryState(bool state)
    {
        _displayInventory = state;

        InventoryScreen.SetActive(state);

        foreach (var s in ScriptsToPause.Cast<IPausable>())
            s.SetPause(state);
    }
}
