using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionReference interactAction; 

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
        FindClosestInteractable();
    }

    private void FindClosestInteractable()
    {
        if (objectsInRange.Count == 0)
        {
            closestObject = null;
            return;
        }

        float minDistance = float.MaxValue;
        InteractableObject nearest = null;

        foreach (var obj in objectsInRange)
        {
            if (obj == null) continue;
            
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }

        if (closestObject != nearest)
        {
            closestObject = nearest;
            if (closestObject != null)
            {
                Debug.Log("Đang đứng gần nhất: " + closestObject.objectName + " - Nhấn 'F' để tương tác.");
            }
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        InteractableObject interactable = collision.GetComponent<InteractableObject>();
        if (interactable != null && !objectsInRange.Contains(interactable))
        {
            objectsInRange.Add(interactable);
        }
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        InteractableObject interactable = collision.GetComponent<InteractableObject>();
        if (interactable != null && objectsInRange.Contains(interactable))
        {
            objectsInRange.Remove(interactable);
            if (closestObject == interactable)
            {
                closestObject = null;
            }
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