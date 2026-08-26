using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] HackingMinigameManager hackingManager;
    public InteractionUI interactionButton;
    public void Interact()
    {
        if (!hackingManager._isActive){
            hackingManager.StartMinigame();
        }
        
    }
}