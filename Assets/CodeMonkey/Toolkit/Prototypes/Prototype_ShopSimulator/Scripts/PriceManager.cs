using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class PriceManager : MonoBehaviour {


        public static PriceManager Instance { get; private set; }


        public event EventHandler OnPriceChanged;


        private Dictionary<ObjectType, float> objectTypePriceDictionary;


        private void Awake() {
            Instance = this;

            objectTypePriceDictionary = new Dictionary<ObjectType, float>();

            // Automatické naplnìní dictionary pro všechny ObjectType
            foreach (var boxData in GameAssetsShopSimulator.Instance.GetAllObjectTypeBoxDatas()) {
                objectTypePriceDictionary[boxData.objectType] = boxData.sellPrice;
            }
        }

        public float GetPrice(ObjectType objectType) {
            if (objectTypePriceDictionary.TryGetValue(objectType, out float price)) {
                return price;
            } else {
                Debug.LogWarning($"Cena pro ObjectType '{objectType}' (key: {(int)objectType}) nebyla nalezena!");
                return 0f; // nebo jiná defaultní hodnota
            }
        }

        public void SetPrice(ObjectType objectType, float price) {
            objectTypePriceDictionary[objectType] = price;

            OnPriceChanged?.Invoke(this, EventArgs.Empty);
        }


    }
}