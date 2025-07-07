using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    public class OpenCloseVecerka : MonoBehaviour, IInteractable {

        private bool isOpen;
        private Transform doorTransform;
        private float doorRotateDuration = 1.0f;

        private void Awake() {
            doorTransform = transform; // Should be DoorHinge with correct pivot
        }

        public void ToggleDoor() {
            isOpen = !isOpen;
            RotateDoor(isOpen);
            Debug.Log(isOpen ? "Opening" : "Closing");
        }

        private void RotateDoor(bool open) {
            StartCoroutine(RotateDoorCoroutine(open));
        }

        private IEnumerator RotateDoorCoroutine(bool open) {
            float time = 0f;

            // Get Y rotation only and work relative to local space
            float startY = doorTransform.localEulerAngles.y;
            float endY = open ? -90f : 0f;

            // Fix for 360 to -90 wraparound issues
            if (startY > 180) startY -= 360;

            while (time < doorRotateDuration) {
                time += Time.deltaTime;
                float currentY = Mathf.Lerp(startY, endY, time / doorRotateDuration);
                Vector3 currentRotation = new Vector3(0f, currentY, 0f);
                doorTransform.localEulerAngles = currentRotation;
                yield return null;
            }

            doorTransform.localEulerAngles = new Vector3(0f, endY, 0f);
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform) {
            ToggleDoor();
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary() {
            return new Dictionary<IInteractable.InteractAction, string> {
                { IInteractable.InteractAction.Primary, "Open/Close Door" }
            };
        }

        public bool CanDoInteractAction(IInteractable.InteractAction interactAction) {
            return interactAction == IInteractable.InteractAction.Primary;
        }

        public Transform GetTransform() {
            return transform;
        }

    }

