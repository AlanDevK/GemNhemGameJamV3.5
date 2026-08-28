using UnityEngine;
using UnityEngine.InputSystem;

public class UITiltEffect : MonoBehaviour
{
    [Header("Tilt Settings")]
    [Tooltip("Maximum angle the UI will rotate")]
    public float maxTiltAngle = 10f;
    [Tooltip("How fast the UI snaps to the mouse position")]
    public float tiltSpeed = 5f;

    [Header("Invert Axes")]
    public bool invertX = false;
    public bool invertY = true; // Usually true feels more natural for UI

    private RectTransform rectTransform;
    private Quaternion originalRotation;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalRotation = rectTransform.localRotation;
    }

    void Update()
    {
        if (this != null)
        {
            if (Mouse.current == null) return;

            // Get mouse position from the new Input System
            Vector2 mousePos = Mouse.current.position.ReadValue();

            // Normalize mouse position to range [-1, 1] based on screen center
            float xNorm = (mousePos.x / Screen.width) * 2f - 1f;
            float yNorm = (mousePos.y / Screen.height) * 2f - 1f;

            // Clamp values just in case the mouse goes out of the game window bounds
            xNorm = Mathf.Clamp(xNorm, -1f, 1f);
            yNorm = Mathf.Clamp(yNorm, -1f, 1f);

            // Calculate the target rotation
            // Mouse X controls the Y axis rotation, Mouse Y controls the X axis rotation
            float rotX = maxTiltAngle * yNorm * (invertY ? -1 : 1);
            float rotY = maxTiltAngle * xNorm * (invertX ? -1 : 1);

            Quaternion targetRotation = originalRotation * Quaternion.Euler(rotX, rotY, 0f);

            // Smoothly lerp the rotation so it doesn't snap instantly
            rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
    }
}