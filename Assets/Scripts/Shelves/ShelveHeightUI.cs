using TMPro;
using UnityEngine;


    public class ShelveHeightUI : MonoBehaviour {


        [SerializeField] private ShelfHeightScript shelfHeight;
        [SerializeField] private SpriteRenderer iconSpriteRenderer;
        [SerializeField] private TextMeshPro textMesh;


        private void Start() {
            shelfHeight.OnObjectTypeChanged += ShelfHeight_OnObjectTypeChanged;
            cenamaanzer.Instance.OnPriceChanged += PriceManager_OnPriceChanged;

            Hide();
        }

        private void PriceManager_OnPriceChanged(object sender, System.EventArgs e) {
            UpdateVisual();
        }

        private void ShelfHeight_OnObjectTypeChanged(object sender, System.EventArgs e) {
            UpdateVisual();
        }

        private void UpdateVisual() {
            Objekttyp objectType = shelfHeight.GetObjectType();
            if (objectType == Objekttyp.None) {
                Hide();
            } else {
                Show(objectType, cenamaanzer.Instance.GetPrice(objectType));
            }
        }

        public void Show(Objekttyp objectType, int price) {
            iconSpriteRenderer.enabled = true;
            VecerkaAsseTSimulaotr.ObjectTypeBoxData objectTypeBoxData = VecerkaAsseTSimulaotr.Instance.GetObjectTypeBoxData(objectType);
            iconSpriteRenderer.sprite = objectTypeBoxData.sprite;
            textMesh.text = VecerkaAsseTSimulaotr.Instance.GetPriceString(price);
        }

        public void Hide() {
            iconSpriteRenderer.enabled = false;
            textMesh.text = "-";
        }

    }


