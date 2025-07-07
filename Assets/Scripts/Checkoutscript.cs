using CodeMonkey.Toolkit.TTextPopup;
using System;
using UnityEngine;

public class Checkoutscript : MonoBehaviour
{
    public static Checkoutscript Instance { get; private set; }


    public event EventHandler<OnObjectScannedEventArgs> OnObjectScanned;
    public class OnObjectScannedEventArgs : EventArgs {
        public Objekttyp ObjectType;
    }


    [SerializeField] private Transform customerPositionTranform;
    [SerializeField] private Transform itemPositionTransform;


    private Zakaznik customer;
    private Transform spawnedObjectTypeTransform;
    private Objekttyp spawnedObjectType;


    private void Awake() {
        Instance = this;
    }


    public Vector3 GetCustomerPosition() {
        return customerPositionTranform.position;
    }

    public bool IsFree() {
        return customer == null;
    }

    public void SetCustomer(Zakaznik customer) {
        this.customer = customer;
    }

    public void AddObject(Objekttyp objectType) {
        spawnedObjectType = objectType;
        spawnedObjectTypeTransform = Instantiate(VecerkaAsseTSimulaotr.Instance.GetObjectTypeBoxData(objectType).boxPrefab, itemPositionTransform.position, itemPositionTransform.rotation);
    }

    public bool HasObjectWaitingToScan() {
        return spawnedObjectTypeTransform != null;
    }

    public void ScanObject() {
        string priceString = VecerkaAsseTSimulaotr.Instance.GetPriceString(cenamaanzer.Instance.GetPrice(spawnedObjectType));
        TextPopupWorld.Create(itemPositionTransform.position + Vector3.up * .4f, "<color=#0f0>+" + priceString + "</color>", .03f, 2f);
        Destroy(spawnedObjectTypeTransform.gameObject);
        Objekttyp scannedObjectType = spawnedObjectType;
        spawnedObjectType = Objekttyp.None;
        customer.LeaveShop();

        OnObjectScanned?.Invoke(this, new OnObjectScannedEventArgs {
            ObjectType = scannedObjectType,
        });
    }

}

