﻿using UnityEngine;
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
		//Disable item physics
		if (item.collider != null)
			item.collider.enabled = false;
		
		if (item.rigidbody != null)
		{
			item.rigidbody.isKinematic = true;
		}

		//Add item to inventory
        _inventory.Add(item);
        var index = _inventory.Count;

        Debug.Log("Carried " + item.name);

		//Set item parent to be the inventory screen
        item.transform.parent = InventoryScreen.transform;

		//Move item to inventory screen layer
        ConvertToLayer(item.gameObject, InventoryScreen.layer);

		//Set position, rotation, scale of newly-added item in the inventory screen
        var uiScale = 130;
        var x = ((index % 6) - 3) * uiScale;
        var y = (1 - (index / 6)) * uiScale;

        item.transform.localScale = Vector3.one * 50;
        item.transform.localRotation = Quaternion.identity;
        item.InventoryPosition = new Vector2(x, y);

        return true;
    }

    private void ConvertToLayer(GameObject gameObject, int newLayer)
    {
        gameObject.layer = newLayer;
        
        var count = gameObject.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            ConvertToLayer(gameObject.transform.GetChild(i).gameObject, newLayer);
        }
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

        foreach (var s in ScriptsToPause) 
		{
			if (s is IPausable)
				(s as IPausable).SetPause (state);
		}
    }
}
