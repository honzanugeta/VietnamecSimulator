using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInteractSingleUIscript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI textMesh;



    public void Setup(IInteractable.InteractAction interactAction, string text) {
        iconImage.sprite = VecerkaAsseTSimulaotr.Instance.GetIconSprite(interactAction);
        textMesh.text = text;
    }
}
