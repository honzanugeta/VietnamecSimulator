using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    public class OpenCloseFront : MonoBehaviour, IInteractable {

        private bool isOpen;
        private Transform doorTransform;
        private float doorRotateDuration = 1.0f; // Duration of the rotation

        private void Awake() {
            doorTransform = transform;
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
            float time = 0;
            Vector3 startRotation = doorTransform.localEulerAngles;
            Vector3 endRotation = open ? new Vector3(0, 90, 0) : new Vector3(0, 0, 0); // Osa Y nastavena na 90 při otevření

            while (time < doorRotateDuration) {
                time += Time.deltaTime;
                doorTransform.localEulerAngles = Vector3.Lerp(startRotation, endRotation, time / doorRotateDuration);
                yield return null;
            }
            doorTransform.localEulerAngles = endRotation;
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

