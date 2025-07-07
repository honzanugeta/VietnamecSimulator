using UnityEngine;
using CodeMonkey.Toolkit.TFunctionPeriodic;
using CodeMonkey.Toolkit.TRandomData;
using System;
public class Zakaznikmanazer : MonoBehaviour
{
    public static Zakaznikmanazer Instance { get; private set; }


    public event EventHandler OnCustomerSpawned;


    [SerializeField] private Transform customerPrefabTransform;
    [SerializeField] private Transform customerSpawnPositionTransform;
    [SerializeField] private Transform customerLeavePositionTransform;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        FunctionPeriodic.Create(() => {
            TrySpawnCustomer();
        }, .5f);
    }

    private void TrySpawnCustomer() {
        int maxCustomerCount = 3;
        if (Zakaznik.GetInstanceList().Count >= maxCustomerCount) {
            // Too many customers spawned
            return;
        }

        if (RandomData.TestChance(10, 100)) {
            Transform customerTransform = Instantiate(customerPrefabTransform, customerSpawnPositionTransform.position, Quaternion.identity);
            OnCustomerSpawned?.Invoke(this, EventArgs.Empty);
        }
    }

    public Vector3 GetCustomerLeavePosition() {
        return customerLeavePositionTransform.position;
    }

}


