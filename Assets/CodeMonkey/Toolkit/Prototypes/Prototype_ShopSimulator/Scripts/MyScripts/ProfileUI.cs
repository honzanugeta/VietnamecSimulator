using System;
using TMPro;
using UnityEngine;

public class ProfileUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI money;

    [Header("Other Scripts")]
    [SerializeField] private Wallet wallet;

    private void Awake()
    {
        wallet.OnKorunyCeskeChanged += UpdateMoneyDisplay;
    }
    private void UpdateMoneyDisplay(float newMoney)
    {
        money.text = newMoney.ToString();
    }
}
