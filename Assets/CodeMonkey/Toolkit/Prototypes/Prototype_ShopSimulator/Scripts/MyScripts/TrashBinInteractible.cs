using CodeMonkey.Toolkit.TTextPopup;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo
{
    public class TrashBinInteractible : MonoBehaviour, IInteractable
    {
        public bool CanDoInteractAction(IInteractable.InteractAction interactAction)
        {
            // Povolit akci TrashObject pouze když hráè nese krabici
            return interactAction == IInteractable.InteractAction.TrashObject &&
                   PlayerShopSimulator.Instance.IsCarryingContainerBox();
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary()
        {
            // Zobrazit možnost vyhození pouze když hráè nese krabici
            if (PlayerShopSimulator.Instance.IsCarryingContainerBox())
            {
                return new Dictionary<IInteractable.InteractAction, string>
                {
                    { IInteractable.InteractAction.TrashObject, "Vyhodit krabici" }
                };
            }
            return null;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform)
        {
            if (interactAction == IInteractable.InteractAction.TrashObject)
            {
                if (PlayerShopSimulator.Instance.IsCarryingContainerBox())
                {

                    ContainerBox carryingBox = PlayerShopSimulator.Instance.GetCarryingContainerBox();
                    if (carryingBox != null && carryingBox.GetAmmout() == 0)
                    {
                        // Znièit nesou krabici
                        PlayerShopSimulator.Instance.ClearCarryingContainerBox();

                        if (carryingBox != null)
                        {
                            Destroy(carryingBox.gameObject);
                        }
                    }
                    else
                    {
                        TextPopupUI.Create(
                            new Vector2(Screen.width * 0.5f, Screen.height * 0.5f),
                             "Nelze vyhodit, krabice není prázdná!",
                            4f,
                            3f,
                            false
                          );
                    }
                }
            }
        }
    }
}
