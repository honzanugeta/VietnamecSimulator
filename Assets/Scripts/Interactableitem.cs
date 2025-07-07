using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    public Item item;

    public bool CanDoInteractAction(IInteractable.InteractAction interactAction)
    {
        return interactAction == IInteractable.InteractAction.PickUpBox;
    }

    public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform)
    {
        if (interactAction != IInteractable.InteractAction.PickUpBox) return;

        Inventory inventory = interactorTransform.GetComponent<Inventory>();
        if (inventory != null && inventory.Add(item))
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Couldn't add item to inventory");
        }
    }

    public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary()
    {
        return new Dictionary<IInteractable.InteractAction, string>
        {
            { IInteractable.InteractAction.PickUpBox, $"Pick up {item.itemName}" }
        };
    }

    public Transform GetTransform()
    {
        return transform;
    }
}