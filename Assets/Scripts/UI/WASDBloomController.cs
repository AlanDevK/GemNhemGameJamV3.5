using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WASDBloomController : MonoBehaviour
{
    [Header("Input Setup")]
    public InputActionReference moveAction;
    [SerializeField] InputActionReference dashAction;
    [SerializeField] InputActionReference shootAction;

    [Header("Sprite References")]
    public SpriteRenderer wSprite;
    public SpriteRenderer aSprite;
    public SpriteRenderer sSprite;
    public SpriteRenderer dSprite;
    [SerializeField] SpriteRenderer spaceSprite;
    [SerializeField] SpriteRenderer leftMouseSprite;

    [Header("Bloom Settings")]
    [ColorUsage(true, true)] public Color normalColor = Color.white;
    [ColorUsage(true, true)] public Color bloomColor = new Color(2f, 2f, 2f, 1f);
    [Tooltip("How fast the color fades in and out. Higher is faster.")]
    public float fadeSpeed = 15f;

    // Track the target state for each direction
    private bool wPressed, aPressed, sPressed, dPressed;

    private void Start()
    {
        // Ensure sprites start at normal color immediately
        if(wSprite) wSprite.color = normalColor;
        if(aSprite) aSprite.color = normalColor;
        if(sSprite) sSprite.color = normalColor;
        if(dSprite) dSprite.color = normalColor;
        if(spaceSprite) spaceSprite.color = normalColor;
        if(leftMouseSprite) leftMouseSprite.color = normalColor;
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.performed += OnMoveInput;
            moveAction.action.canceled += OnMoveInput;
        }
        dashAction.action.Enable();
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.performed -= OnMoveInput;
            moveAction.action.canceled -= OnMoveInput;
        }
        dashAction.action.Disable();
        shootAction.action.Disable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 inputVal = context.ReadValue<Vector2>();

        // Update target states based on the Vector2 axes
        wPressed = inputVal.y > 0.1f;
        sPressed = inputVal.y < -0.1f;
        dPressed = inputVal.x > 0.1f;
        aPressed = inputVal.x < -0.1f;
    }
    private void Update()
    {
        // Continuously smoothly transition colors every frame
        SmoothTransition(wSprite, wPressed);
        SmoothTransition(aSprite, aPressed);
        SmoothTransition(sSprite, sPressed);
        SmoothTransition(dSprite, dPressed);
        SmoothTransition(spaceSprite, dashAction.action.IsPressed());
        SmoothTransition(leftMouseSprite, shootAction.action.IsPressed());
    }

    private void SmoothTransition(SpriteRenderer sprite, bool isPressed)
    {
        if (sprite != null)
        {
            Color targetColor = isPressed ? bloomColor : normalColor;
            // Color.Lerp smoothly blends the current color towards the target color
            sprite.color = Color.Lerp(sprite.color, targetColor, Time.deltaTime * fadeSpeed);
        }
    }
}