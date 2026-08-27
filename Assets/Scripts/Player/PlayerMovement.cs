using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] int dashDamage = 15;
    [SerializeField] float dashHitboxRadius = 1f;

    [Header("Layers")]
    [SerializeField] LayerMask unphasableLayer;
    [SerializeField] LayerMask enemiesLayer;
    [SerializeField] int dashingLayerIndex = 8;
    int originalLayerIndex;

    [Header("Combat & Aiming Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f; // Time delay between shots

    [Header("Screen Shake")]
    [SerializeField] float recoilForce = 0.05f;
    [SerializeField] float dashImpactForce = 1f;
    CinemachineImpulseSource impulseSource;
    [SerializeField] float hitStopDuration = 0.08f;
    bool isHitStopping = false;

    [Header("Knock Back / Damage")]
    bool isKnockedBack = false;
    public bool isInvincible = false;
    Color flashColor = Color.red;
    float invincibleDuration;
    SpriteRenderer sr;
    Color originalColor;
    float flashSpeed = 0.05f;
    float flashTimer = 0f;

    [Header("Input References")]
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private InputActionReference dashActionReference;
    [SerializeField] private InputActionReference fireActionReference;
    [SerializeField] InputActionReference slowMovementReference;

    [SerializeField] int playerHealth = 100;
    [SerializeField] int healBarAmount = 100;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 movementInput;

    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownTimer;
    private float fireTimer;
    bool canShoot = true;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        rb.freezeRotation = true; 
        originalLayerIndex = gameObject.layer;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        originalColor = sr.color;
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
        if (fireActionReference != null && fireActionReference.action.IsPressed() && fireTimer <= 0 && canShoot)
        {
            Shoot();
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(-transform.up * recoilForce);
            }
            fireTimer = fireRate;
        }
    }

    private void FixedUpdate()
    {
        if (!isKnockedBack && !isDashing)
        {
            rb.linearVelocity = movementInput * moveSpeed;
        }
        if (isInvincible)
        {
            flashTimer += Time.deltaTime;
        }
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
            StartCoroutine(Dash());
        }
    } 

    IEnumerator Dash()
    {
        isDashing = true;
        canShoot = false;
        dashCooldownTimer = dashCooldown;
        gameObject.layer = dashingLayerIndex;
        Vector2 dashDir = Vector2.up;
        if (mainCamera != null && Mouse.current != null)
        {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            dashDir = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;
        }
        
        float timer = 0;
        List<Collider2D> hitEnemies = new List<Collider2D>();
        while (timer < dashDuration)
        {
            rb.linearVelocity = dashDir * dashSpeed;
            timer += Time.deltaTime;
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, dashHitboxRadius);
            bool hitSomethingThisFrame = false;
            foreach (Collider2D hit in hitObjects)
            {
                if (hit.CompareTag("Enemy") && !hitEnemies.Contains(hit)){
                    hitEnemies.Add(hit);
                    EnemyAI enemy = hit.GetComponent<EnemyAI>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(dashDamage);
                        if (impulseSource != null)
                        {
                            impulseSource.GenerateImpulse(dashDir * dashImpactForce);
                        }
                        hitSomethingThisFrame = true;
                    }
                }
            }
            if (hitSomethingThisFrame && !isHitStopping)
            {
                StartCoroutine(HitStopRoutine());
                if (impulseSource != null)
                {
                    impulseSource.GenerateImpulse(dashDir * dashImpactForce);
                }
            }
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        gameObject.layer = originalLayerIndex;
        isDashing = false;
        canShoot = true;
    }

    IEnumerator HitStopRoutine()
    {
        isHitStopping = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
        isHitStopping = false;
    }
    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EnemyBullets") && !isInvincible)
        {
            TakeDamage(10);
            float force = 25f;
            float stunDuration = 0.2f;
            Knockback(other.transform.position, force, stunDuration);
        }
    }

    public void Knockback(Vector3 bullet, float force, float stunTime)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        Vector2 dir = (transform.position - bullet).normalized;
        if (dir == Vector2.zero) dir = Vector2.up;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        StartCoroutine(Invincible());
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    IEnumerator Invincible()
    {
        flashTimer = 0f;
        isInvincible = true;
        while (flashTimer <= invincibleDuration)
        {
            sr.color = flashColor;
            yield return new WaitForSeconds(flashSpeed);
            sr.color = originalColor;
        }
        sr.color = originalColor;
        isInvincible = false;
    }
    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        Debug.Log($"The player's hit! I only got {playerHealth} left");
        if (playerHealth <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Game Over!");
        }
    }
}