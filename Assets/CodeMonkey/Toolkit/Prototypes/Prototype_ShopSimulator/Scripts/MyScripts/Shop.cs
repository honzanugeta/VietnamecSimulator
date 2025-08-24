using CodeMonkey.Toolkit.ShopSimulatorDemo;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CodeMonkey.Toolkit.ShopSimulatorDemo.GameAssetsShopSimulator;

public class Shop : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform offersGrid;
    [SerializeField] private OfferUI offerUIPrefab;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private Wallet wallet;

    [Header("Store Controls")]
    [SerializeField] private Button closeOpenButton;
    [SerializeField] private TextMeshProUGUI closeOpenText;
    [SerializeField] private StoreManager storeManager;

    [Header("Dovolena")]
    [SerializeField] private Button dovolenaButton;
    [SerializeField] private GameObject dovolenaGO;
    private int dovolenaPrice = 29500;

    [Header("On Computer UI")]
    [SerializeField] private Image onComputercloseOpen;
    [SerializeField] private TextMeshProUGUI onComputercloseOpenText;

    private ObjectTypeBoxData[] allBoxData;
    private readonly Dictionary<ObjectType, OfferUI> offerUIByType = new();
    private bool isInitialized = false;

    private void Start()
    {
        allBoxData = GameAssetsShopSimulator.Instance.GetAllObjectTypeBoxDatas();

        InstantiateOffers();

        InitStoreToggleButton();

        // Registrace event handleru až po inicializaci
        wallet.OnKorunyCeskeChanged += Wallet_OnKorunyCeskeChanged;
        isInitialized = true;

        UpdateOfferButtons();
        CheckDovolenaCanBuy();

        dovolenaButton.onClick.AddListener(() =>
        {
            CheckDovolenaCanBuy();

            if (wallet.KolikMamKorunCeske() >= dovolenaPrice)
            {
                wallet.RemoveKorunyCeske(dovolenaPrice);

                dovolenaGO.SetActive(true);
            }
        });
    }

    private void OnDestroy()
    {
        if (wallet != null)
        {
            wallet.OnKorunyCeskeChanged -= Wallet_OnKorunyCeskeChanged;
        }
    }

    private void Wallet_OnKorunyCeskeChanged(float koruny)
    {
        // Kontrola, zda je objekt inicializován
        if (isInitialized)
        {
            UpdateOfferButtons();
        }

        CheckDovolenaCanBuy();
    }

    private void CheckDovolenaCanBuy()
    {
        dovolenaButton.gameObject.GetComponent<Image>().color = wallet.KolikMamKorunCeske() >= dovolenaPrice ? Color.green : Color.red;
    }

    #region Store Toggle
    private void InitStoreToggleButton()
    {
        UpdateStoreToggleUI();

        closeOpenButton.onClick.AddListener(() =>
        {
            storeManager.SetIsStoreOpen(!storeManager.IsStoreOpen());
            UpdateStoreToggleUI();
        });
    }

    private void UpdateStoreToggleUI()
    {
        bool isOpen = storeManager.IsStoreOpen();
        string zavrenoText = "Zavzeno";
        string otevrenoText = "Otevzeno";

        closeOpenText.text = isOpen ? otevrenoText : zavrenoText;
        closeOpenButton.GetComponent<Image>().color = isOpen ? Color.green : Color.red;

        onComputercloseOpenText.text = isOpen ? otevrenoText : zavrenoText;
        onComputercloseOpen.color = isOpen ? Color.green : Color.red;
    }
    #endregion

    #region Offers
    private void InstantiateOffers()
    {
        float playerMoney = wallet.KolikMamKorunCeske();

        foreach (var data in allBoxData)
        {
            var ui = Instantiate(offerUIPrefab, offersGrid);
            offerUIByType[data.objectType] = ui;
            ApplyOfferToUI(ui, data, playerMoney);
        }
    }

    private void UpdateOfferButtons()
    {
        // Dodateèná null kontrola pro bezpeènost
        if (allBoxData == null)
        {
            Debug.LogWarning("allBoxData is null - Shop not initialized yet");
            return;
        }

        float playerMoney = wallet.KolikMamKorunCeske();

        foreach (var data in allBoxData)
        {
            if (offerUIByType.TryGetValue(data.objectType, out var ui))
            {
                ApplyOfferToUI(ui, data, playerMoney);
            }
        }
    }

    private static float TotalBuyPrice(ObjectTypeBoxData d) => d.buyPrice * d.productsCount;

    private void ApplyOfferToUI(OfferUI ui, ObjectTypeBoxData data, float playerMoney)
    {
        float totalBuy = TotalBuyPrice(data);
        Color buttonColor = playerMoney >= totalBuy ? Color.green : Color.red;

        ui.SetOfferDetails(
            data.objectName,
            $"{totalBuy:F2} Kè",
            $"Prodat pøibližnì za - {data.sellPrice:F2} Kè",
            $"{data.productsCount} ks",
            data.sprite,
            buttonColor,
            data.objectType,
            this
        );
    }
    #endregion

    #region Buying
    internal void BuyOffer(ObjectType objectType)
    {
        var selectedData = Array.Find(allBoxData, d => d.objectType == objectType);
        if (selectedData == null)
        {
            Debug.LogWarning($"Objekt {objectType} nenalezen!");
            return;
        }

        float totalBuyPrice = TotalBuyPrice(selectedData);
        float playerMoney = wallet.KolikMamKorunCeske();

        if (playerMoney < totalBuyPrice)
        {
            Debug.Log("Nedostatek penìz!");
            return;
        }

        // Zaplatit
        wallet.RemoveKorunyCeske(totalBuyPrice);

        // Spawn boxu
        Transform boxPrefab = selectedData.boxPrefab;
        Transform spawnedBox = Instantiate(boxPrefab, spawnTransform.position, Quaternion.identity);
        spawnedBox.gameObject.AddComponent<DeliveryBox>();
        spawnedBox.GetComponent<ContainerBox>().SetAmmount(selectedData.productsCount);

        Debug.Log($"Koupeno: {selectedData.objectName} {selectedData.productsCount} ks za {totalBuyPrice:F2} Kè");

        UpdateOfferButtons();
    }
    #endregion
}
