using System;
using UnityEngine;



    public class ShelfHeightScript : MonoBehaviour
    {


        public event EventHandler OnObjectTypeChanged;

        
        public enum HeightType {
            Top,
            Middle,
            SecondToLast,
            Bottom,
        }
        
        [SerializeField] protected HeightType heightType;
        [SerializeField] protected Transform gridOriginTransform;
        [SerializeField] protected ShelfScript shelf;

        [SerializeField] private Objekttyp objekttyp; 

        public bool IsEmpty() {
            return objekttyp == Objekttyp.None;
        }

        public new Objekttyp GetObjectType() {
            return objekttyp;
        }

        public void SetObjectType(Objekttyp newobjectType) {
            this.objekttyp = newobjectType;

            OnObjectTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        public Transform GetGridOriginTransform() {
            return gridOriginTransform;
        }

        public HeightType GetHeightType() {
            return heightType;
        }

        public bool TryAddObjectType(Objekttyp objectType) {
            if (!IsEmpty() && GetObjectType() != objectType) {
                // This ShelfHeight already has an objectType and it doesn't match this one!
                return false;
            }
            if (IsEmpty()) {
                SetObjectType(objectType);
            }
            return shelf.TryAddObjectType(this, objectType);
        }

        public bool TryRemoveObjectType(Objekttyp objectType) {
            if (shelf.IsFullyEmpty(this)) {
                // Shelf Empty, cannot remove anything
                return false;
            }
            if (GetObjectType() != objectType) {
                // Different object type, cannot remove
                return false;
            }

            bool ret = shelf.TryRemoveObjectType(this);

            if (shelf.IsFullyEmpty(this)) {
                SetObjectType(Objekttyp.None);
            }

            return ret;
        }


    }

