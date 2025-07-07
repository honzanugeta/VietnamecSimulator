using System;
using System.Collections.Generic;
using UnityEngine;

public class VecerkaAsseTSimulaotr : MonoBehaviour
{
    private static VecerkaAsseTSimulaotr instance;


        public static VecerkaAsseTSimulaotr Instance {
            get {
                if (instance == null) instance = Resources.Load<VecerkaAsseTSimulaotr>(nameof(VecerkaAsseTSimulaotr));
                return instance;
            }
            private set {
                instance = value;
            }
        }



        private void Awake() {
            Instance = this;
        }


        public Sprite codeMonkeySprite;
        public Transform codeMonkeySpritePrefab;
        // Add your own fields here
        public List<ObjectTypeBoxData> objectTypeBoxDataList;
        public List<InteractActionSprite> interactActionSpriteList;



        [Serializable]
        public class ObjectTypeBoxData {
            public Objekttyp objectType;
            public Transform boxPrefab;
            public Sprite sprite;
        }

        [Serializable]
        public class InteractActionSprite {
            public IInteractable.InteractAction interactAction;
            public Sprite sprite;
        }


        public ObjectTypeBoxData GetObjectTypeBoxData(Objekttyp objectType) {
            foreach (ObjectTypeBoxData objectTypeBoxData in objectTypeBoxDataList) {
                if (objectTypeBoxData.objectType == objectType) {
                    return objectTypeBoxData;
                }
            }
            return null;
        }

        public Sprite GetIconSprite(IInteractable.InteractAction interactAction) {
            foreach (InteractActionSprite interactActionSprite in interactActionSpriteList) {
                if (interactActionSprite.interactAction == interactAction) {
                    return interactActionSprite.sprite;
                }
            }
            return null;
        }

        public string GetPriceString(int price) {
            int dollars = Mathf.FloorToInt(price / 100f);
            int cents = (int)(price - (dollars * 100f));
            return "$" + dollars + "." + cents;
        }

    }


