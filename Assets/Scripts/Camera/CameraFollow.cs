using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player; // Player transform
    float cameraDistance = 3.5f;
    [SerializeField] InputActionReference aimAction;

    Camera mainCam;
    void Awake()
    {
        mainCam = Camera.main;
    }
    void OnEnable()
    {
        if (aimAction!= null) aimAction.action.Enable();
    }
    void OnDisable()
    {
        if (aimAction != null) aimAction.action.Disable();
    }
    void FixedUpdate()
    {
        if (player == null || mainCam == null || aimAction == null) return;
        Vector3 mouseScreenPos = aimAction.action.ReadValue<Vector2>();
        Vector2 viewportPos = mainCam.ScreenToViewportPoint(mouseScreenPos);
        viewportPos = (viewportPos * 2f) - Vector2.one;

        float max = 0.9f;
        if (Mathf.Abs(viewportPos.x) > max || Mathf.Abs(viewportPos.y) > max)
        {
            viewportPos = viewportPos.normalized * max;
        }
        Vector3 mouseOffset = new Vector3(viewportPos.x, viewportPos.y, 0f) * cameraDistance;
        transform.position = player.position + mouseOffset;
    }
}