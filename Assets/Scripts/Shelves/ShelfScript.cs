using System.Collections.Generic;
using CodeMonkey.Toolkit.TGridSystem;
using CodeMonkey.Toolkit.TRandomData;
using UnityEngine;



    public class ShelfScript : MonoBehaviour {


        public static List<ShelfScript> instanceList = new List<ShelfScript>();


        public static List<ShelfScript> GetInstanceList() {
            return instanceList;
        }
        
        [SerializeField] private ShelfHeightScript[] shelfHeightArray;
        [SerializeField] private Transform interactFromPositionTransform;

        public class GridObject {


            private GridSystem<GridObject> gridSystem;
            private GridPosition gridPosition;
            private Transform boxTransform;
            private ShelfScript shelf;
            private ShelfHeightScript shelfHeight;
            private Objekttyp objectType;


            public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition, ShelfScript shelf, ShelfHeightScript shelfHeight) {
                this.gridSystem = gridSystem;
                this.gridPosition = gridPosition;
                this.shelf = shelf;
                this.shelfHeight = shelfHeight;
            }

            public bool IsEmpty() {
                return objectType == Objekttyp.None;
            }

            public void AddObjectType(Objekttyp objectType) {
                this.objectType = objectType;
                Transform prefab = VecerkaAsseTSimulaotr.Instance.GetObjectTypeBoxData(objectType).boxPrefab;
                boxTransform = Instantiate(prefab, gridSystem.GetWorldPosition(gridPosition), shelf.transform.rotation);
            }

            public void RemoveObjectType() {
                if (boxTransform != null) {
                    Destroy(boxTransform.gameObject);
                    objectType = Objekttyp.None;
                }
            }

        }
        

        private GridSystem<GridObject>[] gridSystemArray;


        private void Awake() {
            instanceList.Add(this);

            float cellSize = 0.17f;
            gridSystemArray = new GridSystem<GridObject>[shelfHeightArray.Length];
            for (int i = 0; i < shelfHeightArray.Length; i++) {
                ShelfHeightScript shelfHeight = shelfHeightArray[i];

                GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(10, 1, cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition, this, shelfHeight), shelfHeight.GetGridOriginTransform().position);
                gridSystem.SetCustomGetWorldPositionFunc((GridPosition gridPosition) => {
                    return shelfHeight.GetGridOriginTransform().position + (transform.right * gridPosition.x + transform.forward * gridPosition.z) * cellSize;
                });
                //gridSystemTop.CreateDebugObjects(gridDebugObject);

                gridSystemArray[i] = gridSystem;
            }
        }

        private GridSystem<GridObject> GetGridSystem(ShelfHeightScript shelfHeight) {
            for (int i=0; i<shelfHeightArray.Length; i++) {
                if (shelfHeightArray[i] == shelfHeight) {
                    return gridSystemArray[i];
                }
            }
            Debug.LogError("Could not find GridSystem!");
            return null;
        }

        public bool IsFullyEmpty(ShelfHeightScript shelfHeight) {
            GridSystem<GridObject> gridSystem = GetGridSystem(shelfHeight);
            for (int x = 0; x < gridSystem.GetWidth(); x++) {
                for (int y = 0; y < gridSystem.GetHeight(); y++) {
                    GridObject gridObject = gridSystem.GetGridObject(new GridPosition(x, y));
                    if (!gridObject.IsEmpty()) {
                        return false;
                    }
                }
            }
            return true;
        }

        public Objekttyp TryGrabRandomItem() {
            List<ShelfHeightScript> shelfHeighNotEmptytList = new List<ShelfHeightScript>();
            foreach (ShelfHeightScript shelfHeight in shelfHeightArray) {
                if (!IsFullyEmpty(shelfHeight)) {
                    // Not fully empty
                    shelfHeighNotEmptytList.Add(shelfHeight);
                }
            }

            if (shelfHeighNotEmptytList.Count > 0) {
                ShelfHeightScript shelfHeight = shelfHeighNotEmptytList.GetRandomElement();
                Objekttyp objectType = shelfHeight.GetObjectType();
                shelfHeight.TryRemoveObjectType(objectType);
                return objectType;
            }

            return Objekttyp.None;
        }

        private GridObject GetFirstEmptyGridObject(GridSystem<GridObject> gridSystem) {
            for (int x = 0; x < gridSystem.GetWidth(); x++) {
                for (int y = 0; y < gridSystem.GetHeight(); y++) {
                    GridObject gridObject = gridSystem.GetGridObject(new GridPosition(x, y));
                    if (gridObject.IsEmpty()) {
                        return gridObject;
                    }
                }
            }
            return null;
        }

        private GridObject GetFirstNotEmptyGridObject(GridSystem<GridObject> gridSystem) {
            for (int x = 0; x < gridSystem.GetWidth(); x++) {
                for (int y = 0; y < gridSystem.GetHeight(); y++) {
                    GridObject gridObject = gridSystem.GetGridObject(new GridPosition(x, y));
                    if (!gridObject.IsEmpty()) {
                        return gridObject;
                    }
                }
            }
            return null;
        }

        private GridObject GetRandomNotEmptyGridObject(GridSystem<GridObject> gridSystem) {
            List<GridObject> notEmptyGridObjectList = new List<GridObject>();
            for (int x = 0; x < gridSystem.GetWidth(); x++) {
                for (int y = 0; y < gridSystem.GetHeight(); y++) {
                    GridObject gridObject = gridSystem.GetGridObject(new GridPosition(x, y));
                    if (!gridObject.IsEmpty()) {
                        notEmptyGridObjectList.Add(gridObject);
                    }
                }
            }
            return notEmptyGridObjectList.GetRandomElement();
        }

        public bool TryAddObjectType(ShelfHeightScript shelfHeight, Objekttyp objectType) {
            GridSystem<GridObject> gridSystem = GetGridSystem(shelfHeight);
            GridObject gridObject = GetFirstEmptyGridObject(gridSystem);
            if (gridObject != null) {
                gridObject.AddObjectType(objectType);
                return true;
            } else {
                // Could not add object, no empty spots
                return false;
            }
        }

        public bool TryRemoveObjectType(ShelfHeightScript shelfHeight) {
            GridSystem<GridObject> gridSystem = GetGridSystem(shelfHeight);
            GridObject gridObject = GetRandomNotEmptyGridObject(gridSystem);
            if (gridObject != null) {
                gridObject.RemoveObjectType();
                return true;
            } else {
                return false;
            }
        }

        public Vector3 GetInteractFromPosition() {
            return interactFromPositionTransform.position;
        }

    }

