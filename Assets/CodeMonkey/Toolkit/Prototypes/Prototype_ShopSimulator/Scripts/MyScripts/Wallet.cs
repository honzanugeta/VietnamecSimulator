using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
   private float korunyCeske;

    public event Action<float> OnKorunyCeskeChanged;

    private void Start()
    {
        AddKorunyCeske(367.0f); // Add initial amount to the wallet
    }

    public void AddKorunyCeske(float amount)
    {
        korunyCeske += amount;
        OnKorunyCeskeChanged?.Invoke(korunyCeske);
    }

    public void RemoveKorunyCeske(float amount)
    {
        if (korunyCeske >= amount)
        {
            korunyCeske -= amount;
            OnKorunyCeskeChanged?.Invoke(korunyCeske);
        }
        else
        {
            Debug.LogWarning("Not enough koruny to remove: " + amount);
        }
    }

    public float KolikMamKorunCeske()
    {
        return korunyCeske;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
        {
            RemoveKorunyCeske(500);
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
        {
            AddKorunyCeske(1000);
        }
    }
}
