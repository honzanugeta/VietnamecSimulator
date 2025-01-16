using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    public Item item;

    public string InteractionPrompt => $"Pick up {item.itemName}";

    public bool Interact(Interactor interactor)
    {
        Inventory inventory = interactor.GetComponent<Inventory>();
        if (inventory != null && inventory.Add(item))
        {
            Destroy(gameObject); // Destroy the item in the world
            return true;
        }

        Debug.Log("Couldn't add item to inventory");
        return false;
    }
}