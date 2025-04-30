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
    private float currentTotal = 0f;

    private void Start()
    {
        PopulateItems();
    }

    private void PopulateItems()
    {
        foreach (var item in availableItems)
        {
            GameObject itemButtonGo = Instantiate(itemButtonPrefab, itemsPanel);
            Button button = itemButtonGo.GetComponent<Button>();
            Image image = itemButtonGo.transform.Find("Image").GetComponent<Image>(); // Assuming there's an Image child
            TextMeshProUGUI nameText = itemButtonGo.transform.Find("NameText").GetComponent<TextMeshProUGUI>(); // Assuming there's a Text child

            image.sprite = item.image;
            nameText.text = item.name;

            button.onClick.AddListener(() => AddItemToOrder(item));
        }
    }

    private void AddItemToOrder(Item item)
    {
        currentTotal += item.price;
        UpdateTotalText();
        Debug.Log("Added " + item.name + " to order. Total: " + currentTotal);
        // Implement visual feedback here (e.g., animation, sound)
    }

    private void UpdateTotalText()
    {
        totalText.text = "Total: CZK" + currentTotal.ToString("F2");
    }

    public void PlaceOrder()
    {
        Debug.Log("Order placed! Total amount: CZK" + currentTotal);
        // Implement order placement logic here (e.g., deduct currency, spawn items)
        currentTotal = 0f;
        UpdateTotalText();
    }
}

