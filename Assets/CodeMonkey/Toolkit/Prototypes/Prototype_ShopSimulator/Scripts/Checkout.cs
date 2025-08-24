using CodeMonkey.Toolkit.TTextPopup;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo
{
    public class Checkout : MonoBehaviour
    {
        public static Checkout Instance { get; private set; }

        public event EventHandler<OnObjectScannedEventArgs> OnObjectScanned;
        public class OnObjectScannedEventArgs : EventArgs
        {
            public ObjectType objectType;
        }

        [SerializeField] private Transform customerPositionTranform;
        [SerializeField] private Transform itemPositionTransform;
        [SerializeField] private Wallet wallet;

        private Customer customer;

        // Active (spawned) item currently on the belt
        private Transform spawnedObjectTypeTransform;
        private ObjectType spawnedObjectType;

        // Pending items for the active customer
        private readonly Queue<ObjectType> pendingQueue = new Queue<ObjectType>();

        private void Awake()
        {
            Instance = this;
        }

        public Vector3 GetCustomerPosition()
        {
            return customerPositionTranform.position;
        }

        public bool IsFree()
        {
            return customer == null;
        }

        public void SetCustomer(Customer customer)
        {
            // Reset state for a new customer
            ClearActiveItem();
            pendingQueue.Clear();
            this.customer = customer;
        }

        public void AddObject(ObjectType objectType)
        {
            if (customer == null)
            {
                Debug.LogWarning("Checkout.AddObject called but no active customer.");
                return;
            }

            pendingQueue.Enqueue(objectType);

            // If nothing is currently spawned, spawn the next item now
            if (!HasObjectWaitingToScan())
            {
                TrySpawnNextFromQueue();
            }
        }

        public bool HasObjectWaitingToScan()
        {
            return spawnedObjectTypeTransform != null;
        }

        public void ScanObject()
        {
            if (!HasObjectWaitingToScan())
                return;

            // Price popup (pickup price *before* we clear spawnedObjectType)
            float price = PriceManager.Instance.GetPrice(spawnedObjectType);
            string priceString = GameAssetsShopSimulator.Instance.GetPriceString(price);
            TextPopupWorld.Create(itemPositionTransform.position + Vector3.up * .4f, "<color=#0f0>+" + priceString + "</color>", .03f, 2f);

            // Add money
            wallet.AddKorunyCeske(price);

            // Destroy the scanned object on the belt
            if (spawnedObjectTypeTransform != null)
                Destroy(spawnedObjectTypeTransform.gameObject);

            // Fire event for this specific object
            ObjectType scannedObjectType = spawnedObjectType;
            OnObjectScanned?.Invoke(this, new OnObjectScannedEventArgs { objectType = scannedObjectType });

            // Clear active slot
            spawnedObjectTypeTransform = null;
            spawnedObjectType = ObjectType.None;

            // Are there more items? If yes, spawn next. If not, finish the customer.
            if (pendingQueue.Count > 0)
            {
                TrySpawnNextFromQueue();
            }
            else
            {
                FinishCustomerAndFreeCheckout();
            }
        }


        private void TrySpawnNextFromQueue()
        {
            if (spawnedObjectTypeTransform != null)
                return; // Belt already has an item
            if (pendingQueue.Count == 0)
                return;

            spawnedObjectType = pendingQueue.Dequeue();
            Transform prefab = GameAssetsShopSimulator.Instance.GetObjectTypeBoxData(spawnedObjectType).shelfBoxPrefab;
            spawnedObjectTypeTransform = Instantiate(prefab, itemPositionTransform.position, itemPositionTransform.rotation);
        }

        private void FinishCustomerAndFreeCheckout()
        {
            if (customer != null)
            {
                // Tell the customer to leave only after all of their items are scanned
                customer.LeaveShop();
            }
            customer = null;
            ClearActiveItem();
            pendingQueue.Clear();
        }

        private void ClearActiveItem()
        {
            if (spawnedObjectTypeTransform != null)
            {
                Destroy(spawnedObjectTypeTransform.gameObject);
            }
            spawnedObjectTypeTransform = null;
            spawnedObjectType = ObjectType.None;
        }
    }
}
