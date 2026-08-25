using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;

    void Update()
    {
        // Fly along the y-axis
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}