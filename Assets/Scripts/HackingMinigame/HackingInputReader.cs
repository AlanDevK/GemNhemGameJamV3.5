using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HackingInputReader : MonoBehaviour
{
    [Header("Input Configuration")]
    [Tooltip("2D Vector Composite action (e.g., WASD or Arrow Keys)")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float deadzone = 0.5f;

    public event Action<HackingDirection> OnDirectionInput;

    private void OnEnable()
    {
        if (moveAction == null || moveAction.action == null) return;
        moveAction.action.performed += HandleInput;
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction == null || moveAction.action == null) return;
        moveAction.action.performed -= HandleInput;
        moveAction.action.Disable();
    }

    private void HandleInput(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        if (input.sqrMagnitude < deadzone * deadzone) return;

        HackingDirection direction = Mathf.Abs(input.x) > Mathf.Abs(input.y)
            ? (input.x > 0 ? HackingDirection.Right : HackingDirection.Left)
            : (input.y > 0 ? HackingDirection.Up : HackingDirection.Down);

        OnDirectionInput?.Invoke(direction);
    }
}