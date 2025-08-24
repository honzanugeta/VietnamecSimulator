using CodeMonkey.Toolkit.ShopSimulatorDemo;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Computer : MonoBehaviour, IInteractable
{
    private bool isInteracting = false;

    public event Action<bool> OnInteract;

    public bool CanDoInteractAction(IInteractable.InteractAction interactAction)
    {
        return interactAction == IInteractable.InteractAction.InteractWithComputer;
    }

    public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary()
    {
        var interactTextDict = new Dictionary<IInteractable.InteractAction, string>();
        interactTextDict[IInteractable.InteractAction.InteractWithComputer] = "Použít poèítaè";
        return interactTextDict;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform)
    {
        isInteracting = !isInteracting;
        OnInteract?.Invoke(isInteracting);
    }


}
