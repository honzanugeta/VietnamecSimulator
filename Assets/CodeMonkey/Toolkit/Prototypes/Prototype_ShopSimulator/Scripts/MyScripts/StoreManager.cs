using CodeMonkey.Toolkit.ShopSimulatorDemo;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    private bool isStoreOpen = false;

    [SerializeField] private CustomerManager customerManager;


    public static StoreManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SetIsStoreOpen(bool isOpen)
    {
        isStoreOpen = isOpen;
    }

    public bool IsStoreOpen()
    {
        return isStoreOpen;
    }

    public bool CanChangePrice()
    {
        // To Change Price, the store must be closed and in store must be 0 customers
        return !isStoreOpen && CustomerManager.GetCustomerCount() == 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
