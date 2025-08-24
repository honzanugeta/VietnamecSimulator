using System;
using System.Collections.Generic;
using CodeMonkey.Toolkit.TGridSystem;
using CodeMonkey.Toolkit.TRandomData;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo
{
    public class Shelf : MonoBehaviour
    {
        public static List<Shelf> instanceList = new List<Shelf>();
        public static List<Shelf> GetInstanceList() => instanceList;

        private Customer reservedBy;   // who is walking to this shelf
        private Customer shoppingBy;   // who is currently at this shelf


        public bool IsFree() => reservedBy == null && shoppingBy == null;

        public bool TryReserve(Customer c)
        {
            if (c == null)
                return false;
            if (reservedBy == null && shoppingBy == null)
            {
                reservedBy = c;
                return true;
            }
            return false;
        }

        public void CancelReservation(Customer c)
        {
            if (reservedBy == c)
                reservedBy = null;
        }
        public bool BeginShopping(Customer c)
        {
            if (c == null)
                return false;

            // Normal path: reserver arrives
            if (reservedBy == c && shoppingBy == null)
            {
                reservedBy = null;
                shoppingBy = c;
                return true;
            }

            // Edge: no reservation but still free (scene start, manual teleports, etc.)
            if (reservedBy == null && shoppingBy == null)
            {
                shoppingBy = c;
                return true;
            }

            return false;
        }

        public void EndShopping(Customer c)
        {
            if (shoppingBy == c)
                shoppingBy = null;
        }
        [SerializeField] private bool isThereACustomerShopping = false;

        public void SetIsThereACustomerShopping(bool isThereACustomerShopping)
        {
            this.isThereACustomerShopping = isThereACustomerShopping;


            if (isThereACustomerShopping)
            {
                // do nothing; only for inspector view
            }
            else
            {
                // Clear both (defensive)
                reservedBy = null;
                shoppingBy = null;
            }
        }

        public bool IsThereACustomerShopping()
        {

            return shoppingBy != null || isThereACustomerShopping;
        }


        public class GridObject
        {
            private GridSystem<GridObject> gridSystem;
            private GridPosition gridPosition;
            private Transform boxTransform;
            private Shelf shelf;
            private ShelfHeight shelfHeight;
            private ObjectType objectType;

            public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition, Shelf shelf, ShelfHeight shelfHeight)
            {
                this.gridSystem = gridSystem;
                this.gridPosition = gridPosition;
                this.shelf = shelf;
                this.shelfHeight = shelfHeight;
            }

            public bool IsEmpty()
            {
                return objectType == ObjectType.None;
            }

            public void AddObjectType(ObjectType objectType)
            {
                this.objectType = objectType;
                Transform prefab = GameAssetsShopSimulator.Instance.GetObjectTypeBoxData(objectType).shelfBoxPrefab;
                boxTransform = Instantiate(prefab, gridSystem.GetWorldPosition(gridPosition), shelf.transform.rotation);
            }

            public void RemoveObjectType()
            {
                if (boxTransform != null)
                {
                    Destroy(boxTransform.gameObject);
                    objectType = ObjectType.None;
                }
            }
        }

        [SerializeField] protected ShelfHeight[] shelfHeightArray;
        [SerializeField] private Transform gridDebugObject;
        [SerializeField] protected Transform interactFromPositionTransform;
        [SerializeField] protected Transform lookPositionWhenShoping;

        private GridSystem<GridObject>[] gridSystemArray;

        private void Awake()
        {
            instanceList.Add(this);

            float cellSize = 0.17f;
            gridSystemArray = new GridSystem<GridObject>[shelfHeightArray.Length];
            for (int i = 0; i < shelfHeightArray.Length; i++)
            {
                ShelfHeight shelfHeight = shelfHeightArray[i];

                GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(
                    10, 1, cellSize,
                    (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition, this, shelfHeight),
                    shelfHeight.GetGridOriginTransform().position
                );

                gridSystem.SetCustomGetWorldPositionFunc((GridPosition gridPosition) =>
                {
                    return shelfHeight.GetGridOriginTransform().position + (transform.right * gridPosition.x + transform.forward * gridPosition.z) * cellSize;
                });
                //gridSystem.CreateDebugObjects(gridDebugObject);

                gridSystemArray[i] = gridSystem;
            }
        }

        private void OnDestroy()
        {
            instanceList.Remove(this);
        }

        private GridSystem<GridObject> GetGridSystem(ShelfHeight shelfHeight)
        {
            for (int i = 0; i < shelfHeightArray.Length; i++)
            {
                if (shelfHeightArray[i] == shelfHeight)
                {
                    return gridSystemArray[i];
                }
            }
            Debug.LogError("Could not find GridSystem!");
            return null;
        }

        public bool IsFullyEmpty(ShelfHeight shelfHeight)
        {
            GridSystem<GridObject> gridSystem = GetGridSystem(shelfHeight);
            for (int x = 0; x < gridSystem.GetWidth(); x++)
            {
                for (int y = 0; y < gridSystem.GetHeight(); y++)
                {
                    GridObject gridObject = gridSystem.GetGridObject(new GridPosition(x, y));
                    if (!gridObject.IsEmpty())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public ObjectType TryGrabRandomItem()
        {
            List<ShelfHeight> shelfHeighNotEmptytList = new List<ShelfHeight>();
            foreach (ShelfHeight shelfHeight in shelfHeightArray)
            {
                if (!IsFullyEmpty(shelfHeight))
                {
                    // Not fully empty
                    shelfHeighNotEmptytList.Add(shelfHeight);
                }
            }

            if (shelfHeighNotEmptytList.Count > 0)
            {
                ShelfHeight shelfHeight = shelfHeighNotEmptytList.GetRandomElement();
                ObjectType objectType = shelfHeight.GetObjectType();
                shelfHeight.TryRemoveObjectType(objectType);
                return objectType;
            }

            return ObjectType.None;
        }

        private GridObject GetFirstEmptyGridObject(GridSystem<GridObject> gridSystem)
        {
            for (int x = 0; x < gridSystem.GetWidth(); x++)
            {
                for (int y = 0; y < gridSystem.GetHeight(); y++)
                {
                    GridObject gridObject = gridSystem.GetGridObject(new GridPosition(x, y));
                    if (gridObject.IsEmpty())
                    {
                        return gridObject;
                    }
                }
            }
            return null;
        }

        private GridObject GetRandomNotEmptyGridObject(GridSystem<GridObject> gridSystem)
        {
            List<GridObject> notEmptyGridObjectList = new List<GridObject>();
            for (int x = 0; x < gridSystem.GetWidth(); x++)
            {
                for (int y = 0; y < gridSystem.GetHeight(); y++)
                {
                    GridObject gridObject = gridSystem.GetGridObject(new GridPosition(x, y));
                    if (!gridObject.IsEmpty())
                    {
                        notEmptyGridObjectList.Add(gridObject);
                    }
                }
            }
            return notEmptyGridObjectList.GetRandomElement();
        }

        public bool TryAddObjectType(ShelfHeight shelfHeight, ObjectType objectType)
        {
            GridSystem<GridObject> gridSystem = GetGridSystem(shelfHeight);
            GridObject gridObject = GetFirstEmptyGridObject(gridSystem);
            if (gridObject != null)
            {
                gridObject.AddObjectType(objectType);
                return true;
            }
            else
            {
                // Could not add object, no empty spots
                return false;
            }
        }

        public bool TryRemoveObjectType(ShelfHeight shelfHeight)
        {
            GridSystem<GridObject> gridSystem = GetGridSystem(shelfHeight);
            GridObject gridObject = GetRandomNotEmptyGridObject(gridSystem);
            if (gridObject != null)
            {
                gridObject.RemoveObjectType();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Vector3 GetInteractFromPosition()
        {
            return interactFromPositionTransform.position;
        }

        public Vector3 GetLookingPositionWhenShoping()
        {
            return lookPositionWhenShoping.position;
        }

        internal ObjectType PeekRandomItem()
        {
            List<ShelfHeight> shelfHeighNotEmptytList = new List<ShelfHeight>();
            foreach (ShelfHeight shelfHeight in shelfHeightArray)
            {
                if (!IsFullyEmpty(shelfHeight))
                {
                    // Not fully empty
                    shelfHeighNotEmptytList.Add(shelfHeight);
                }
            }

            if (shelfHeighNotEmptytList.Count > 0)
            {
                ShelfHeight shelfHeight = shelfHeighNotEmptytList.GetRandomElement();
                return shelfHeight.GetObjectType();
            }

            return ObjectType.None;
        }
    }
}
