using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isStackable;
    
    
    public virtual void Use()
    {
        Debug.Log($"Using {itemName}");
        // Add custom behavior here for when the item is used
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
