using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    private bool readyToJump = true;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    private Rigidbody rb;

    // Cache commonly used values to avoid calling them repeatedly
    private Transform _transform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        _transform = transform; // Cache the transform
    }

    private void Update()
    {
        // Ground check using SphereCast for more consistent results
        grounded = Physics.Raycast(_transform.position, Vector3.down, playerHeight * 0.6f, whatIsGround);

        HandleInput();
        HandleDrag();

        // Debugging: Check if we are grounded
        Debug.Log("Grounded: " + grounded);
    }

    private void FixedUpdate()
    {
        MovePlayer();
        SpeedControl();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump logic
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded) // Use GetKeyDown for single jump trigger
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;

        // Add force based on grounded or airborne
        float currentMoveSpeed = grounded ? moveSpeed : moveSpeed * airMultiplier;
        rb.AddForce(moveDirection * currentMoveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit the velocity if it exceeds moveSpeed
        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // Debugging: Check if Jump is called
        Debug.Log("Jump triggered");

        // Reset the vertical velocity before applying the jump force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void HandleDrag()
    {
        // Adjust drag based on whether the player is grounded
        rb.drag = grounded ? groundDrag : 0f;
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
