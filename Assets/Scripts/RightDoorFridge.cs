using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TInteractionSystemLookAt {

    public class RightDoorFridge : MonoBehaviour, IInteractable {

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
            float startYRotation = doorTransform.localEulerAngles.y;
            float endYRotation = open ? -90f : 0f; // Adjust the end rotation as needed

            // Ensure the shortest path is taken
            if (startYRotation > 180f) startYRotation -= 360f;

            while (time < doorRotateDuration) {
                time += Time.deltaTime;
                float yRotation = Mathf.Lerp(startYRotation, endYRotation, time / doorRotateDuration);
                doorTransform.localEulerAngles = new Vector3(doorTransform.localEulerAngles.x, yRotation, doorTransform.localEulerAngles.z);
                yield return null;
            }
            doorTransform.localEulerAngles = new Vector3(doorTransform.localEulerAngles.x, endYRotation, doorTransform.localEulerAngles.z);
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

}