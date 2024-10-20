using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float sensX = 100f;
    public float sensY = 100f;

    [Header("References")]
    public Transform orientation;

    private float xRotation;
    private float yRotation;

    private const float MIN_X_ROTATION = -90f;
    private const float MAX_X_ROTATION = 90f;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMouseInput();
        ApplyRotation();
    }

    private void HandleMouseInput()
    {
        float deltaTime = Time.deltaTime; // Cache Time.deltaTime for slight optimization
        float mouseX = Input.GetAxisRaw("Mouse X") * deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * deltaTime * sensY;

        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation - mouseY, MIN_X_ROTATION, MAX_X_ROTATION);
    }

    private void ApplyRotation()
    {
        // Apply rotation to the camera and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}