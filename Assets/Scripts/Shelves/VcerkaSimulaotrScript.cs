using CodeMonkey.Toolkit.TFirstPersonController;
using UnityEngine;



    public class VcerkaSimulaotrScript : MonoBehaviour {


        public static VcerkaSimulaotrScript Instance { get; private set; }
        
        [SerializeField] private Transform carryingObjectParentTransform;

        
        private playerlookat playerInteractLookAt;
        private FirstPersonController firstPersonController;
        private KonteinerBox carryingContainerBox;


        private void Awake() {
            Instance = this;

            playerInteractLookAt = GetComponent<playerlookat>();
            firstPersonController = GetComponent<FirstPersonController>();
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                IInteractable interactable = playerInteractLookAt.GetInteractableObject();
                if (interactable != null) {
                    if (interactable.CanDoInteractAction(IInteractable.InteractAction.Stock)) {
                        interactable.Interact(IInteractable.InteractAction.Stock, transform);
                    }
                    if (interactable.CanDoInteractAction(IInteractable.InteractAction.ScanObject)) {
                        interactable.Interact(IInteractable.InteractAction.ScanObject, transform);
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                IInteractable interactable = playerInteractLookAt.GetInteractableObject();
                if (interactable != null) {
                    interactable.Interact(IInteractable.InteractAction.Unstock, transform);
                }
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                if (!IsCarryingContainerBox()) {
                    // Not carrying anything, pick up box?
                    IInteractable interactable = playerInteractLookAt.GetInteractableObject();
                    if (interactable != null) {
                        if (interactable.CanDoInteractAction(IInteractable.InteractAction.PickUpBox)) {
                            interactable.Interact(IInteractable.InteractAction.PickUpBox, transform);
                        }
                    }
                } else {
                    // Carrying something, drop it
                    ClearCarryingContainerBox();
                }
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                IInteractable interactable = playerInteractLookAt.GetInteractableObject();
                if (interactable != null) {
                    interactable.Interact(IInteractable.InteractAction.ChangePrice, transform);
                }
            }
        }

        public void Freeze() {
            enabled = false;
            playerInteractLookAt.enabled = false;
            firstPersonController.UnlockMouse();
            firstPersonController.Disable();
        }

        public void Unfreeze() {
            enabled = true;
            playerInteractLookAt.enabled = true;
            firstPersonController.LockMouse();
            firstPersonController.Enable();
        }

        public bool IsCarryingContainerBox() {
            return carryingContainerBox != null;
        }

        public KonteinerBox GetCarryingContainerBox() {
            return carryingContainerBox;
        }

        public void SetCarryingContainerBox(KonteinerBox containerBox) {
            carryingContainerBox = containerBox;
            carryingContainerBox.GetTransform().parent = carryingObjectParentTransform;
            carryingContainerBox.GetTransform().localPosition = Vector3.zero;
            carryingContainerBox.SetState(KonteinerBox.State.PickedUp);
        }

        public void ClearCarryingContainerBox() {
            if (carryingContainerBox == null) {
                // Not carrying anything
                return;
            }
            carryingContainerBox.SetState(KonteinerBox.State.Ground);
            carryingContainerBox.GetTransform().parent = null;
            carryingContainerBox.GetTransform().position = new Vector3(
                carryingContainerBox.GetTransform().position.x,
                0f,
                carryingContainerBox.GetTransform().position.z
            );
            carryingContainerBox = null;
        }

    }

