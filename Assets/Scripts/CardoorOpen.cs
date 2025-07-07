using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    public class CardoorOpen : MonoBehaviour, IInteractable
    {
        private bool isOpen;
        private Transform doorTransform;
        private float doorRotateDuration = 1.0f; // Doba rotace
        private float doorMoveDuration = 1.0f; // Doba posunu
        private Vector3 closedPosition;
        private Vector3 openPosition;

        private void Awake() {
            doorTransform = transform;
            closedPosition = doorTransform.localPosition;
            openPosition = closedPosition + new Vector3(0, 0, -1f); // Posun dovnitř (nastavte podle potřeby)
        }

        public void ToggleDoor() {
            isOpen = !isOpen;
            StartCoroutine(OpenCloseDoorCoroutine(isOpen));
            Debug.Log(isOpen ? "Opening" : "Closing");
        }

       private IEnumerator OpenCloseDoorCoroutine(bool open) {
            float time = 0;
            
            if (open) {
                // OPENING ANIMATION
                Vector3 currentRotation = doorTransform.localEulerAngles;
                Vector3 currentPosition = doorTransform.localPosition;
                Vector3 endRotation = new Vector3(90, currentRotation.y, currentRotation.z);
                Vector3 firstPosition = new Vector3(currentPosition.x, 2.97f, -9);
                Vector3 finalPosition = new Vector3(currentPosition.x, 2.97f, -7);

                // Rotace na 90° X osu a nastavení pozice Y=3, Z=-9
                while (time < doorRotateDuration) {
                    time += Time.deltaTime;
                    doorTransform.localEulerAngles = Vector3.Lerp(currentRotation, endRotation, time / doorRotateDuration);
                    doorTransform.localPosition = Vector3.Lerp(currentPosition, firstPosition, time / doorRotateDuration);
                    yield return null;
                }
                doorTransform.localEulerAngles = endRotation;
                doorTransform.localPosition = firstPosition;

                // Zpoždění 1 sekundu
                yield return new WaitForSeconds(1.0f);

                // Finální změna Z pozice na -6
                time = 0;
                while (time < doorRotateDuration) {
                    time += Time.deltaTime;
                    doorTransform.localPosition = Vector3.Lerp(firstPosition, finalPosition, time / doorRotateDuration);
                    yield return null;
                }
                doorTransform.localPosition = finalPosition;
            } else {
                // CLOSING ANIMATION - návrat na pozici 0,0,0 a rotaci 0,0,0
                Vector3 currentRotation = doorTransform.localEulerAngles;
                Vector3 currentPosition = doorTransform.localPosition;
                Vector3 zeroRotation = Vector3.zero; // Přesně nulová rotace 0,0,0
                Vector3 zeroPosition = Vector3.zero; // Přesně nulová pozice 0,0,0
                
                // Návrat zpět na nulovou rotaci a nulovou pozici
                while (time < doorRotateDuration * 2) {
                    time += Time.deltaTime;
                    float normalizedTime = time / (doorRotateDuration * 2);
                    doorTransform.localEulerAngles = Vector3.Lerp(currentRotation, zeroRotation, normalizedTime);
                    doorTransform.localPosition = Vector3.Lerp(currentPosition, zeroPosition, normalizedTime);
                    yield return null;
                }
                doorTransform.localEulerAngles = zeroRotation; // Explicitně nastavíme rotaci 0,0,0
                doorTransform.localPosition = zeroPosition; // Explicitně nastavíme pozici 0,0,0
            }
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
