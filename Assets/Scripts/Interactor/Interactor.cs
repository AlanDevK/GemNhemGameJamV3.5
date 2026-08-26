using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string objectName = "realComtutar";
    [SerializeField] HackingMinigameManager hackingManager;
    [TextArea] public string interactionMessage = "Đã tương tác thành công với thiết bị này!";

    public virtual void Interact()
    {
        Debug.Log("Đang tương tác với: " + objectName + " - " + interactionMessage);
        if (!hackingManager._isActive){
            hackingManager.StartMinigame();
        }
    }
}