using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseVecerka : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
  
    [SerializeField] private Animator mAnimator;

    private bool isOpen = false;

    void Start()
    {
        
        if (mAnimator == null)
        {
            Debug.LogError("Animator component not found on the object!");
        }
    }

    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Interact called on OpenCloseVecerka.");

        if (mAnimator != null)
        {
            if (!isOpen)
            {
                Debug.Log("Opening object.");
                mAnimator.SetTrigger("TOpen");
                isOpen = true;
            }
            else
            {
                Debug.Log("Closing object.");
                mAnimator.SetTrigger("TClose");
                isOpen = false;
            }
        }
        else
        {
            Debug.LogError("Animator is null in OpenCloseVecerka.");
        }

        return true;
    }
}