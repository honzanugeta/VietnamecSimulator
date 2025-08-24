using UnityEngine;

public class DeliveryBox : MonoBehaviour
{
    private void Start()
    {
        // Add rigedboy with gravity and lock rotation
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }

    // On colision remove rb
    private void OnCollisionEnter(Collision collision)
    {
        // Destroy only this component DeliveryBox from gameObject
        Debug.Log("DeliveryBox collided");
        Rigidbody rb = GetComponent<Rigidbody>();
        Destroy(rb);

        Destroy(this);

    }
}
