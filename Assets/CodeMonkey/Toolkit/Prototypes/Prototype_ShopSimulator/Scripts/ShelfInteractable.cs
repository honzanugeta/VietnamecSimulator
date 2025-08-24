using CodeMonkey.Toolkit.TBlockerUI;
using CodeMonkey.Toolkit.TInputWindow;
using CodeMonkey.Toolkit.TTextPopup;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo
{

    public class ShelfInteractable : MonoBehaviour, IInteractable
    {


        [SerializeField] private ShelfHeight shelfHeight;

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary()
        {
            return new Dictionary<IInteractable.InteractAction, string>{
            { IInteractable.InteractAction.Stock, "Naskladnit" }, // Translated
            { IInteractable.InteractAction.Unstock, "Vyskladnit" }, // Translated
            { IInteractable.InteractAction.ChangePrice, "Zmìnit cenu" }, // Translated
            };
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public bool CanDoInteractAction(IInteractable.InteractAction interactAction)
        {
            return
                interactAction == IInteractable.InteractAction.Stock ||
                interactAction == IInteractable.InteractAction.Unstock ||
                interactAction == IInteractable.InteractAction.ChangePrice;
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform)
        {
            switch (interactAction)
            {
                case IInteractable.InteractAction.Stock:
                    if (PlayerShopSimulator.Instance.IsCarryingContainerBox())
                    {
                        // Player is carrying something, try to stock it
                        ContainerBox containerBox = PlayerShopSimulator.Instance.GetCarryingContainerBox();
                        if (containerBox.CanRemoveAmount())
                        {
                            // Container box has amount
                            if (shelfHeight.TryAddObjectType(containerBox.GetObjectType()))
                            {
                                // Stocked!
                                containerBox.RemoveAmount();
                            }
                            else
                            {
                                // Failed to stock! Probably different object type already there
                            }
                        }
                    }
                    break;
                case IInteractable.InteractAction.Unstock:
                    if (PlayerShopSimulator.Instance.IsCarryingContainerBox())
                    {
                        // Player is carrying something, try to unstock it
                        ContainerBox containerBox = PlayerShopSimulator.Instance.GetCarryingContainerBox();
                        if (containerBox.CanAddAmount(shelfHeight.GetObjectType()))
                        {
                            // Container box can store more amount of this type
                            if (shelfHeight.TryRemoveObjectType(containerBox.GetObjectType()))
                            {
                                // Unstocked!
                                containerBox.AddAmount();
                            }
                            else
                            {
                                // Failed to unstock!
                            }
                        }
                    }
                    break;
                case IInteractable.InteractAction.ChangePrice:

                    if (!shelfHeight.IsEmpty())
                    {

                        if (StoreManager.Instance.CanChangePrice() == false)
                        {
                            // Cannot change price, store is open or there are customers
                            // Show message to player
                            Debug.Log("Cannot change price, store is open or there are customers");

                            TextPopupUI.Create(
                                   new Vector2(Screen.width * 0.5f, Screen.height * 0.5f),
                                   "Nelze zmìnit cenu, dokud je obchod otevøený nebo jsou uvnitø zákazníci.",
                                   4f,
                                   3f,
                                   false
                             );
                        }
                        else
                        {
                            PlayerShopSimulator.Instance.Freeze();
                            BlockerUI.Show();
                            InputWindowUI.Show(
                                "Nová cena pro " + shelfHeight.GetObjectType(),
                                PriceManager.Instance.GetPrice(shelfHeight.GetObjectType()).ToString("0.##"),
                                "0123456789.,", // povolte èárku i teèku
                                20,
                                () =>
                                {
                                    BlockerUI.Hide();
                                    PlayerShopSimulator.Instance.Unfreeze();
                                },
                                (string newPriceText) =>
                                {
                                    // Nahraïte èárku teèkou pro správné parsování
                                    string normalized = newPriceText.Replace(',', '.');
                                    if (float.TryParse(normalized, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float newPrice))
                                    {
                                        PriceManager.Instance.SetPrice(shelfHeight.GetObjectType(), newPrice);
                                    }
                                    BlockerUI.Hide();
                                    PlayerShopSimulator.Instance.Unfreeze();
                                });
                        }

                    }
                    else
                    {
                        // It's empty, cannot change price
                    }
                    break;
            }
        }
    }

}