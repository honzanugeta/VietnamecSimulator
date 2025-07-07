using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    private const string MoneyKey = "PlayerMoney";

    public float CurrentMoney { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadMoney(); // Load on start
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool Spend(float amount)
    {
        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            SaveMoney(); // Save after spending
            return true;
        }
        return false;
    }

    public void Add(float amount)
    {
        CurrentMoney += amount;
        SaveMoney(); // Save after adding
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetFloat(MoneyKey, CurrentMoney);
        PlayerPrefs.Save();
    }

    private void LoadMoney()
    {
        CurrentMoney = PlayerPrefs.GetFloat(MoneyKey, 1000f); // default starting money
    }

    public void ResetMoney(float amount = 1000f)
    {
        CurrentMoney = amount;
        SaveMoney();
    }
}