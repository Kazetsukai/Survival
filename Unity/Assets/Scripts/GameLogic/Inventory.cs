using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour 
{
    List<CarryableItem> _inventory = new List<CarryableItem>();

    public bool CarryItem(CarryableItem item)
    {
        _inventory.Add(item);

        Debug.Log("Carried " + item.name);

        return true;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
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
    }
}
