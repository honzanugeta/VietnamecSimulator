using UnityEngine;

public class shelvebox : MonoBehaviour
{
    [SerializeField] private Objekttyp objectType;


    public Objekttyp GetObjectType() {
        return objectType;
    }

}
