using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent; // The parent object of all the slots
    private Inventory inventory;
    private InventorySlot[] slots;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            inventory.onInventoryChangedCallback += UpdateUI;
        }
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    void UpdateUI()
    {
        List<Item> items = inventory.GetItems();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].AddItem(items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}