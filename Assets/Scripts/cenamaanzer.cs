using System;
using System.Collections.Generic;
using UnityEngine;
public class cenamaanzer : MonoBehaviour
{
    public static cenamaanzer Instance { get; private set; }


    public event EventHandler OnPriceChanged;


    private Dictionary<Objekttyp, int> objectTypePriceDictionary;


    private void Awake() {
        Instance = this;

        objectTypePriceDictionary = new Dictionary<Objekttyp, int>();

        objectTypePriceDictionary[Objekttyp.monsterwhite] = 22;
        objectTypePriceDictionary[Objekttyp.monsteroriginal] = 22;
        objectTypePriceDictionary[Objekttyp.monsterblue] = 22;
        objectTypePriceDictionary[Objekttyp.monsterpink] = 22;
    }

    public int GetPrice(Objekttyp objectType) {
        return objectTypePriceDictionary[objectType];
    }

    public void SetPrice(Objekttyp objectType, int price) {
        objectTypePriceDictionary[objectType] = price;

        OnPriceChanged?.Invoke(this, EventArgs.Empty);
    }


}


