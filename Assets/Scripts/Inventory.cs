using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int inventorySize = 20; // Define maximum inventory size
    private List<Item> items = new List<Item>();

    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChangedCallback;

    public bool Add(Item item)
    {
        if (items.Count >= inventorySize)
        {
            Debug.Log($"Added {item.itemName} to inventory");
            Debug.Log("Inventory full");
            return false;
        }

        items.Add(item);
        onInventoryChangedCallback?.Invoke();
        Debug.Log($"{item.itemName} added to inventory");
        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        onInventoryChangedCallback?.Invoke();
        Debug.Log($"{item.itemName} removed from inventory");
    }

    public List<Item> GetItems()
    {
        return items;
    }
}