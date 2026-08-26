using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
        Destroy(gameObject, 3f); // Auto destroy after 3 seconds
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Hit the player
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player took damage from enemy bullet!");
            // TODO: Call player health script to reduce HP here
            
            Destroy(gameObject); // Destroy bullet on hit
        }
    }
}