using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionReference interactAction;
    
    // We only track the object we are currently standing next to
    private InteractableObject currentInteractable;

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractPerformed;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TryGetComponent already gives you the reference, no need to call GetComponent again!
        if (collision.TryGetComponent(out InteractableObject interactableObject))
        {
            currentInteractable = interactableObject;
            currentInteractable.interactionButton.Show();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InteractableObject interactableObject))
        {
            interactableObject.interactionButton.Hide();
            
            // Clear the reference when we walk away
            if (currentInteractable == interactableObject)
            {
                currentInteractable = null;
            }
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        // Now this will actually work when the button is pressed once
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
}