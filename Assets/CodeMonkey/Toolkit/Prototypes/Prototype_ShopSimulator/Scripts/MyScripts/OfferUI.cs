using CodeMonkey.Toolkit.ShopSimulatorDemo;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI offerNameText;
    [SerializeField] private TextMeshProUGUI offerPriceText;
    [SerializeField] private TextMeshProUGUI offerSellText;
    [SerializeField] private TextMeshProUGUI offerAmmountText;

    [SerializeField] private Button offerButton;

    [SerializeField] private Image offerImage;

    private Shop shop;

    public void SetOfferDetails(string offerName, string offerPrice, string sellDescription, string ammountText, Sprite offerImageSprite, Color buttonColor, ObjectType objectType, Shop shopReference)
    {
        offerNameText.text = offerName;
        offerPriceText.text = offerPrice;
        offerSellText.text = sellDescription;
        offerAmmountText.text = ammountText;
        offerImage.sprite = offerImageSprite;
        offerButton.GetComponent<Image>().color = buttonColor;

        offerButton.onClick.RemoveAllListeners();
        offerButton.onClick.AddListener(() => OnOfferButtonClicked(objectType));

        shop = shopReference;
    }

    private void OnOfferButtonClicked(ObjectType objectType)
    {
        shop.BuyOffer(objectType);
    }
}
