using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string objectName = "realComtutar";
    [TextArea] public string interactionMessage = "Đã tương tác thành công với thiết bị này!";

    // Hàm này sẽ được gọi khi Player đứng gần nhất và bấm phím F
    public virtual void Interact()
    {
        Debug.Log("Đang tương tác với: " + objectName + " - " + interactionMessage);
        // TODO: Viết logic mở UI, hack máy, hoặc chạy sự kiện của bạn ở đây
    }
}