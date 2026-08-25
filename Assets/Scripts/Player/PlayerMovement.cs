using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Input References")]
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private InputActionReference dashActionReference;
    [SerializeField] private InputActionReference abilityActionReference;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 lastValidDirection = Vector2.right; // Default looking direction

    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Freeze rotation for pure top-down perspective to prevent tipping over
        rb.freezeRotation = true; 
    }

    private void OnEnable()
    {
        // Enable actions when the script becomes active
        moveActionReference.action.Enable();
        dashActionReference.action.Enable();
        
        // Subscribe to the dash action event
        dashActionReference.action.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        dashActionReference.action.performed -= OnDashPerformed;
        moveActionReference.action.Disable();
        dashActionReference.action.Disable();
    }

    private void Update()
    {
        // Read Vector2 value from the Input System (WASD / Joystick)
        movementInput = moveActionReference.action.ReadValue<Vector2>();

        // Store the last valid movement direction for idle dashing
        if (movementInput != Vector2.zero)
        {
            lastValidDirection = movementInput.normalized;
        }

        // Countdown the dash cooldown timer
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // Handle dash state movement
        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
            return; // Skip regular movement to prioritize dash speed
        }

        // Regular movement using Rigidbody2D
        rb.linearVelocity = movementInput * moveSpeed;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        // Check if the player can dash (cooldown ready and not currently dashing)
        if (dashCooldownTimer <= 0 && !isDashing)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Determine dash direction: move input direction if moving, otherwise last valid direction
        Vector2 dashDir = movementInput != Vector2.zero ? movementInput.normalized : lastValidDirection;
        
        // Apply burst velocity in the dash direction
        rb.linearVelocity = dashDir * dashSpeed;
    }
}