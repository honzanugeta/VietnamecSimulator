using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletOrderingsystem : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string name;
        public string description;
        public Sprite image;
        public float price;
    }

    public List<Item> availableItems = new List<Item>();
    public Transform itemsPanel; // Assign in inspector
    public GameObject itemButtonPrefab; // Assign in inspector
    public TextMeshProUGUI totalText; // Assign in inspector
    public TextMeshProUGUI totalCount; // Assign in inspector
    public Sprite beachVacationSprite; // assign in inspector
    public GameObject insufficientFundsPopup;


    private float currentTotal = 0f;
    private int itemCount = 0; // Počet zakoupených položek
    private const int maxItems = 10; // Limit nákupu

    private void Start()
    {
        PopulateItems();
        AddSpecialItem();

    }

    private void PopulateItems()
    {
        foreach (var item in availableItems)
        {
            GameObject itemButtonGo = Instantiate(itemButtonPrefab, itemsPanel);
            Button button = itemButtonGo.GetComponent<Button>();
            Image image = itemButtonGo.transform.Find("Image").GetComponent<Image>();
            TextMeshProUGUI nameText = itemButtonGo.transform.Find("NameText").GetComponent<TextMeshProUGUI>();

            image.sprite = item.image;
        
            // Clean slate - remove any components that might interfere
            AspectRatioFitter[] fitters = image.GetComponents<AspectRatioFitter>();
            for (int i = 0; i < fitters.Length; i++)
            {
                DestroyImmediate(fitters[i]);
            }
        
            ContentSizeFitter[] sizeFitters = image.GetComponents<ContentSizeFitter>();
            for (int i = 0; i < sizeFitters.Length; i++)
            {
                DestroyImmediate(sizeFitters[i]);
            }
        
            // Set image properties
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
        
            // Force exact positioning and size
            RectTransform imageRect = image.GetComponent<RectTransform>();
        
            // Set anchors to stretch within parent bounds
            imageRect.anchorMin = new Vector2(0.1f, 0.1f); // 10% margin from edges
            imageRect.anchorMax = new Vector2(0.9f, 0.9f); // 10% margin from edges
            imageRect.offsetMin = Vector2.zero;
            imageRect.offsetMax = Vector2.zero;
            imageRect.localScale = Vector3.one;
        
            nameText.text = item.name;
            button.onClick.AddListener(() => AddItemToOrder(item));
        }
    }

    private void AddSpecialItem()
    {
        GameObject itemButtonGo = Instantiate(itemButtonPrefab, itemsPanel);
        Button button = itemButtonGo.GetComponent<Button>();
        Image image = itemButtonGo.transform.Find("Image").GetComponent<Image>();
        TextMeshProUGUI nameText = itemButtonGo.transform.Find("NameText").GetComponent<TextMeshProUGUI>();

        image.sprite = beachVacationSprite; // assign via inspector
        image.preserveAspect = true;

        RectTransform imageRect = image.GetComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0.1f, 0.1f);
        imageRect.anchorMax = new Vector2(0.9f, 0.9f);
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;
        imageRect.localScale = Vector3.one;

        nameText.text = "Beach Vacation";

        Item beachVacation = new Item
        {
            name = "Beach Vacation",
            price = 9999.99f,
            image = beachVacationSprite
        };

        button.onClick.AddListener(() => AddItemToOrder(beachVacation));
    }

    public void StartOrder()
    {
        itemCount = 0; // Reset počtu položek
        totalCount.text = "Remaining Items: " + (maxItems - itemCount); // Nastavení počáteční hodnoty
    }

    private void AddItemToOrder(Item item)
    {
        if (itemCount >= maxItems)
        {
            Debug.LogWarning("Reached the maximum item limit!");
            return;
        }

        currentTotal += item.price;
        itemCount++;
        totalCount.text = "Remaining Items: " + (maxItems - itemCount); // Aktualizace zbývajícího počtu
        UpdateTotalText();
        Debug.Log("Added " + item.name + " to order. Total: " + currentTotal);
    }

    private void UpdateTotalText()
    {
        totalText.text = "Total: CZK" + currentTotal.ToString("F2");
    }

    public void PlaceOrder()
    {
        if (currentTotal <= 0)
        {
            Debug.LogWarning("No items in order!");
            return;
        }

        if (MoneyManager.Instance.Spend(currentTotal))
        {
            Debug.Log("Order placed! Total amount: CZK" + currentTotal);
            currentTotal = 0f;
            itemCount = 0;
            UpdateTotalText();
            totalCount.text = "Remaining Items: " + (maxItems - itemCount); // Reset text
        }
        else
        {
            Debug.LogWarning("Insufficient funds to place the order!");
            if (insufficientFundsPopup != null)
            {
                insufficientFundsPopup.SetActive(true);
            }
        }
    }
    public void CloseInsufficientFundsPopup()
    {
        if (insufficientFundsPopup != null)
            insufficientFundsPopup.SetActive(false);
    }


}