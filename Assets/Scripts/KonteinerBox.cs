using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KonteinerBox : MonoBehaviour, IInteractable
{
    public enum State {
            Ground,
            PickedUp
        }


        [SerializeField] private Objekttyp objectType;
        [SerializeField] private TextMeshPro amountTextMesh;


        private State state = State.Ground;
        private BoxCollider boxCollider;
        private int amount = 15;


        private void Awake() {
            boxCollider = GetComponent<BoxCollider>();
            amountTextMesh.text = amount.ToString();
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary() {
            return new Dictionary<IInteractable.InteractAction, string>{
                { IInteractable.InteractAction.PickUpBox, "Pick up" }
            };
        }

        public Objekttyp GetObjectType() {
            return objectType;
        }

        public Transform GetTransform() {
            return transform;
        }

        public bool CanDoInteractAction(IInteractable.InteractAction interactAction) {
            return
                interactAction == IInteractable.InteractAction.PickUpBox ||
                interactAction == IInteractable.InteractAction.DropBox;
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform) {
            if (!VcerkaSimulaotrScript.Instance.IsCarryingContainerBox()) {
                // Not yet carrying a box, can carry
                VcerkaSimulaotrScript.Instance.SetCarryingContainerBox(this);
            }
        }

        public bool CanAddAmount(Objekttyp objectType) {
            if (GetObjectType() == objectType) {
                return true;
            } else {
                return false;
            }
        }

        public void AddAmount() {
            amount++;
            amountTextMesh.text = amount.ToString();
        }

        public bool CanRemoveAmount() {
            return amount > 0;
        }

        public void RemoveAmount() {
            amount--;
            amountTextMesh.text = amount.ToString();
        }

        public void SetState(State state) {
            this.state = state;

            switch (state) {
                case State.Ground:
                    boxCollider.enabled = true;
                    break;
                case State.PickedUp:
                    boxCollider.enabled = false;
                    break;
            }
        }

    }

