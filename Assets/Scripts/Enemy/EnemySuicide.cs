using UnityEngine;

public class SuicideEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Explosion")]
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private GameObject explosionEffectPrefab;

    private Transform player;
    private Camera mainCamera;

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

        // Check if inside camera view
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool isInCameraView = viewportPos.x >= 0 && viewportPos.x <= 1 && 
                               viewportPos.y >= 0 && viewportPos.y <= 1 && 
                               viewportPos.z > 0;

        if (isInCameraView)
        {
            MoveAndAimTowardsPlayer();
        }
    }

    private void MoveAndAimTowardsPlayer()
    {
        // Move towards player
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Rotate towards player
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        // Spawn explosion effect
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
        }

        // Deal area damage
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // hit.GetComponent<PlayerHealth>()?.TakeDamage(explosionDamage);
            }
        }

        // Destroy self
        Destroy(gameObject);
    }

    // Visualize explosion radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}