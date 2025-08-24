using CodeMonkey.Toolkit.TFunctionPeriodic;
using CodeMonkey.Toolkit.TRandomData;
using System;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class CustomerManager : MonoBehaviour {

        [SerializeField] private int maxCustomerCount = 3;


        public static CustomerManager Instance { get; private set; }


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
        
            if(!StoreManager.Instance.IsStoreOpen()) {
                // Store is closed, no customers can spawn
                return;
            }


            if (Customer.GetInstanceList().Count >= maxCustomerCount) {
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

        public static int GetCustomerCount() {
            return Customer.GetInstanceList().Count;
        }

    }

}