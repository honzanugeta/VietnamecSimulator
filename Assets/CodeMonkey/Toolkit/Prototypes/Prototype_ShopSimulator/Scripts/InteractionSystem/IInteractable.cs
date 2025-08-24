using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public interface IInteractable {

        public enum InteractAction {
            None,
            Stock,
            Unstock,
            PickUpBox,
            DropBox,
            ChangePrice,
            ScanObject,
            InteractWithComputer,
            TrashObject,
            ToggleRadio
        }

        public bool CanDoInteractAction(InteractAction interactAction);

        public void Interact(InteractAction interactAction, Transform interactorTransform);

        public Dictionary<InteractAction, string> GetInteractTextDictionary();

        public Transform GetTransform();

    }

}