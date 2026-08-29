using System.Collections;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] float attackStoppingDistance = 4f;
    [SerializeField] float retreatDistance = 2f;

    [Header ("Strafing")]
    [SerializeField] float strafeChangeInterval = 2f;
    [SerializeField] float navMeshSampleRadius = 2f;

    [Header("Combat")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float fireInterval = 2f;

    [Header("Visibility")]
    [SerializeField] float exitZoneBufferTime = 0.6f;

    [Header("Stats")]
    [SerializeField] int health;

    [Header("Damage")]
    SpriteRenderer sr;
    Color flashColor = Color.red;
    [SerializeField] float flashDuration = 0.1f;
    Color originalColor;

    [SerializeField] Transform player;
    PlayerMovement playerMovement;
    RepairUI repair;
    private Camera mainCamera;
    private float fireTimer;
    float strafeTimer;
    float strafeDirection = 1f;
    float exitTimer;

    NavMeshAgent agent;
    bool isInCameraView;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        playerMovement = FindObjectOfType<PlayerMovement>();
        repair = FindObjectOfType<RepairUI>();
        agent = GetComponent<NavMeshAgent>();
        if (agent!= null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = moveSpeed;
            agent.stoppingDistance = 0.2f;
        }
        mainCamera = Camera.main;
        
        strafeDirection = Random.value > 0.5f ? 1f : -1f;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || mainCamera == null) return;

        Vector3 vp = mainCamera.WorldToViewportPoint(transform.position);
        bool insideEnterBounds = vp.z > 0 && vp.x >= 0.05f && vp.x <= 0.95f && vp.y >= 0.05f && vp.y <= 0.95f;
        bool outsideExitBounds = vp.z <= 0 || vp.x < -0.15f ||  vp.x > 1.15f || vp.y < -0.15f || vp.y > 1.15f;
        if (!isInCameraView && insideEnterBounds)
        {
            isInCameraView = true;
            exitTimer = 0f;
            CombatZoneManager.Instance?.RegisterEnemy(this);
        } else if (isInCameraView && outsideExitBounds)
        {
            exitTimer += Time.deltaTime;
            if (exitTimer >= exitZoneBufferTime)
            {
                isInCameraView = false;
                exitTimer = 0f;
                CombatZoneManager.Instance?.UnregisterEnemy(this);
            }
        } else
        {
            exitTimer = 0f;
        }
        HandleMovement();
        if (isInCameraView)
        {
            AimTowardsPlayer();

            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                Shoot();
                fireTimer = 0f;
            }
        }
    }

    void HandleMovement()
    {
        if (agent == null  ||  !agent.isOnNavMesh) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position); 

        strafeTimer += Time.deltaTime;
        if (strafeTimer >= strafeChangeInterval)
        {
            if (Random.value > 0.4f) strafeDirection *= -1f;
            strafeTimer = 0f;
        }
        Vector3 targetDestination;
        if (distanceToPlayer < retreatDistance)
        {
            Vector2 retreatDir = -toPlayer;
            Vector2 desiredPos = (Vector2)transform.position + retreatDir * (retreatDistance - distanceToPlayer + 1.5f);
            targetDestination = GetValidNavMeshPoint(desiredPos);
        } else if (distanceToPlayer <= attackStoppingDistance)
        {
            Vector2 perpendicularDir = new Vector2(-toPlayer.y, toPlayer.x) * strafeDirection;
            Vector2 desiredPos = (Vector2)transform.position + (perpendicularDir * 2f);
            targetDestination = GetValidNavMeshPoint(desiredPos);
        }
        else
        {
            targetDestination = player.position;
        }
        agent.SetDestination(targetDestination);
    }

    Vector3 GetValidNavMeshPoint(Vector3 targetPos)
    {
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
        {
            return hit.position;
        } return transform.position;
    }
    private void AimTowardsPlayer()
    {
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Shoot()
    {
        if (enemyBulletPrefab == null || firePoints == null) return;
        foreach (Transform firePoint in firePoints)
        {
            if (firePoint != null) Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void OnDisable()
    {
        if (CombatZoneManager.Instance != null) CombatZoneManager.Instance.UnregisterEnemy(this);
    }

    void OnDestroy()
    {
        if (CombatZoneManager.Instance != null) CombatZoneManager.Instance.UnregisterEnemy(this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullets"))
        {
            TakeDamage(10);
        }
    }

    IEnumerator FlashDamage()
    {
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }
    public void TakeDamage(int damage)
    {
        StartCoroutine(FlashDamage());
        health -= damage;
        Debug.Log($"I'm hit! I only got {health} left");
        if (health <= 0)
        {  
            gameObject.SetActive(false);
            if (Mathf.Abs(playerMovement.playerHeal - playerMovement.playerMaxHeal)>=5)
            {
                playerMovement.playerHeal+=5;
                repair.SetHeal(playerMovement.playerHeal);
            }
        }
    }
}