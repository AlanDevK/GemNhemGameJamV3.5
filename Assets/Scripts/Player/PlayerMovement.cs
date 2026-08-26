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

    [Header("Combat & Aiming Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f; // Time delay between shots

    [Header("Input References")]
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private InputActionReference dashActionReference;
    [SerializeField] private InputActionReference fireActionReference;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 movementInput;

    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownTimer;
    private float fireTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        rb.freezeRotation = true; 
    }

    private void OnEnable()
    {
        moveActionReference.action.Enable();
        dashActionReference.action.Enable();
        if (fireActionReference != null) fireActionReference.action.Enable();
        
        dashActionReference.action.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        dashActionReference.action.performed -= OnDashPerformed;

        moveActionReference.action.Disable();
        dashActionReference.action.Disable();
        if (fireActionReference != null) fireActionReference.action.Disable();
    }

    private void Update()
    {
        movementInput = moveActionReference.action.ReadValue<Vector2>();
        RotateTowardsMouse();

        // Cooldowns
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (fireTimer > 0) fireTimer -= Time.deltaTime;

        // Continuous shooting when holding the fire button
        if (fireActionReference != null && fireActionReference.action.IsPressed() && fireTimer <= 0)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;
            if (dashTimeLeft <= 0) isDashing = false;
            return;
        }

        rb.linearVelocity = movementInput * moveSpeed;
    }

    // Rotate player to face the mouse cursor
    private void RotateTowardsMouse()
    {
        if (mainCamera == null || Mouse.current == null) return;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 direction = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
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

        Vector2 dashDir = Vector2.up;
        if (mainCamera != null && Mouse.current != null)
        {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            dashDir = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;
        }
        
        rb.linearVelocity = dashDir * dashSpeed;
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}