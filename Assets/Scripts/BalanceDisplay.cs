using TMPro;
using UnityEngine;

public class BalanceDisplay : MonoBehaviour
{
    public TextMeshProUGUI balanceText;

    private void Update()
    {
        if (balanceText != null && MoneyManager.Instance != null)
        {
            balanceText.text = "CZK " + MoneyManager.Instance.CurrentMoney.ToString("F2");
        }
    }
}