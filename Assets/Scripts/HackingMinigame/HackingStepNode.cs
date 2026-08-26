using UnityEngine;
using UnityEngine.UI;

public class HackingStepNode : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Color pendingColor = Color.white;
    [SerializeField] private Color completedColor = Color.yellow;
    [SerializeField] private Color failedColor = Color.red;

    public void Setup(HackingDirection dir, Sprite arrowSprite)
    {
        iconImage.sprite = arrowSprite;
        iconImage.color = pendingColor;

        // Rotate arrow based on direction
        float rotationZ = dir switch
        {
            HackingDirection.Up => 0f,
            HackingDirection.Right => -90f,
            HackingDirection.Down => 180f,
            HackingDirection.Left => 90f,
            _ => 0f
        };
        iconImage.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
    }

    public void SetCompleted() => iconImage.color = completedColor;
    public void SetFailed() => iconImage.color = failedColor;
}