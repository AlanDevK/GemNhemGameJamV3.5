using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] float interactRange = 2f;
    [SerializeField] LayerMask interactLayer;

    private List<InteractableObject> objectsInRange = new List<InteractableObject>();
    private InteractableObject closestObject;

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

    private void Update()
    {
        if (interactAction.action.IsPressed())
        {
            Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (Collider2D collider in colliderArray)
            {
                if (collider.TryGetComponent(out InteractableObject interactableObject)) interactableObject.Interact();
            }
        }
    }

    public InteractableObject GetInteractableObject()
    {
        Collider2D interactableCollider = Physics2D.OverlapCircle(transform.position, interactRange,interactLayer);
        if (interactableCollider.TryGetComponent(out InteractableObject interactableObject)) return interactableObject;
        return null;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InteractableObject interactableObject))
        {
            collision.gameObject.GetComponent<InteractableObject>().interactionButton.Show();
        }
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InteractableObject interactableObject))
        {
            collision.gameObject.GetComponent<InteractableObject>().interactionButton.Hide();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (closestObject != null)
        {
            closestObject.Interact();
        }
    }
}