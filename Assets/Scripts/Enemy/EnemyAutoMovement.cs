using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Combat")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 2f;

    private Transform player;
    private Camera mainCamera;
    private float fireTimer;

    void Start()
    {
        mainCamera = Camera.main;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || mainCamera == null) return;

        // Check if enemy is inside the camera view bounds (0 to 1)
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool isInCameraView = viewportPos.x >= 0 && viewportPos.x <= 1 && 
                              viewportPos.y >= 0 && viewportPos.y <= 1 && 
                              viewportPos.z > 0;

        // Only chase and shoot when visible inside the camera
        if (isInCameraView)
        {
            MoveAndAimTowardsPlayer();

            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                Shoot();
                fireTimer = 0f;
            }
        }
    }

    private void MoveAndAimTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Shoot()
    {
        if (enemyBulletPrefab != null && firePoint != null)
        {
            Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}