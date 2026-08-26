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

    [SerializeField] Transform player;
    private Camera mainCamera;
    private float fireTimer;
    float strafeTimer;
    float strafeDirection = 1f;

    NavMeshAgent agent;
    bool isInCameraView;

    void Start()
    {
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

        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
        // Check if enemy is inside the camera view bounds (0 to 1)
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool currentlyVisible = viewportPos.x >= 0 && viewportPos.x <= 1 && 
                              viewportPos.y >= 0 && viewportPos.y <= 1 && 
                              viewportPos.z > 0;

        // Only chase and shoot when visible inside the camera
        if (currentlyVisible != isInCameraView)
        {
            isInCameraView = currentlyVisible;
            if (isInCameraView)
            {
                CombatZoneManager.Instance?.RegisterEnemy(this);
            } else {
                CombatZoneManager.Instance?.UnregisterEnemy(this);
            }

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
}