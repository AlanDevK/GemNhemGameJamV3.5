using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Player transform
    [SerializeField] private float smoothSpeed = 5f; // Follow smoothness
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Camera offset (Z should be negative in 2D)

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate target position with offset
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly interpolate towards the target position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}