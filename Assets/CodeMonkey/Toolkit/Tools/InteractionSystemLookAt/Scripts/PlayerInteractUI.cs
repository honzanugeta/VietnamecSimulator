using UnityEngine;
using TMPro;

namespace CodeMonkey.Toolkit.TInteractionSystemLookAt {

    public class PlayerInteractUI : MonoBehaviour {


        [SerializeField] private GameObject containerGameObject;
        [SerializeField] private PlayerInteractLookAt playerInteract;
        [SerializeField] private TextMeshProUGUI interactTextMeshProUGUI;


        private void Update() {
            if (playerInteract.GetInteractableObject() != null) {
                Show(playerInteract.GetInteractableObject());
            } else {
                Hide();
            }
        }

        private void Show(IInteractable interactable) {
            containerGameObject.SetActive(true);
            interactTextMeshProUGUI.text = interactable.GetInteractTextDictionary()[IInteractable.InteractAction.Primary];
        }

        private void Hide() {
            containerGameObject.SetActive(false);
        }

    }

}