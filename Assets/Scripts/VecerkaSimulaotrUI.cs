using UnityEngine;
using CodeMonkey.Toolkit.TTextPopup;


public class VecerkaSimulaotrUI : MonoBehaviour
{
    private void Start() {
        Checkoutscript.Instance.OnObjectScanned += Checkout_OnObjectScanned;
        Zakaznikmanazer.Instance.OnCustomerSpawned += CustomerManager_OnCustomerSpawned;
    }

    private void CustomerManager_OnCustomerSpawned(object sender, System.EventArgs e) {
        TextPopupUI.Create(new Vector2(0, 400), "New Customer entered!", 2f, 2f);
    }

    private void Checkout_OnObjectScanned(object sender, Checkoutscript.OnObjectScannedEventArgs e) {
        string priceString = VecerkaAsseTSimulaotr.Instance.GetPriceString(cenamaanzer.Instance.GetPrice(e.ObjectType));
        TextPopupUI.Create(new Vector2(0, 400), "Sold " + e.ObjectType + " <color=#0f0>+" + priceString + "</color>", 2f, 2f);
    }
}
