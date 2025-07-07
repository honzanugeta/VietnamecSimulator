using System.Collections.Generic;
using UnityEngine;



    public interface IInteractable {

        public enum InteractAction {
            None,
            Primary,
            Stock,
            Unstock,
            PickUpBox,
            DropBox,
            ChangePrice,
            ScanObject,
        }

        public bool CanDoInteractAction(InteractAction interactAction);

        public void Interact(InteractAction interactAction, Transform interactorTransform);

        public Dictionary<InteractAction, string> GetInteractTextDictionary();

        public Transform GetTransform();

    }

