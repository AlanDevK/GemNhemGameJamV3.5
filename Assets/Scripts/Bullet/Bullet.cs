using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Walls") || other.gameObject.CompareTag("Borders"))
        {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Enemy") && gameObject.CompareTag("Bullets"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Player") && gameObject.CompareTag("EnemyBullets"))
        {
            Destroy(gameObject);
        }
    }
}