using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HackingUI : MonoBehaviour
{
    [Header("UI Slots")]
    [SerializeField] RectTransform sequenceContainer;
    [SerializeField] Image arrowSlotPrefab;
    [SerializeField] Image timerFillBar;

    [Header("Sprites")]
    [SerializeField] Sprite upArrow;
    [SerializeField] Sprite downArrow;
    [SerializeField] Sprite leftArrow;
    [SerializeField] Sprite rightArrow;

    [Header("Colors")]
    [SerializeField] Color pendingColor = Color.white;
    [SerializeField] Color completeColor = Color.yellow;
    [SerializeField] Color failColor = Color.red;

    readonly List<Image> activeArrowSlots = new List<Image>(8);

    public void DisplaySequence(IReadOnlyList<HackingInputDirection> sequence, int round, int maxRounds){
        while (activeArrowSlots.Count < sequence.Count){
            Image newSlot = Instantiate(arrowSlotPrefab, sequenceContainer);
            activeArrowSlots.Add(newSlot);
        }
        for (int i = 0; i<activeArrowSlots.Count; i++){
            if (i < sequence.Count){
                activeArrowSlots[i].gameObject.SetActive(true);
                activeArrowSlots[i].sprite = GetArrowSprite(sequence[i]);
                activeArrowSlots[i].color = pendingColor;
            } else activeArrowSlots[i].gameObject.SetActive(false);
        }
    }

    public void MarkStepCompleted(int stepIndex){
        if (stepIndex >= 0 && stepIndex < activeArrowSlots.Count) activeArrowSlots[stepIndex].color = completeColor;
    }

    public void UpdateTimer(float normalizedTime){
        if (timerFillBar != null) timerFillBar.fillAmount = Mathf.Clamp01(normalizedTime);
    }

    public void ShowFailureFeedback(){
        foreach (Image arrow in activeArrowSlots){
            if (arrow.gameObject.activeSelf) arrow.color = failColor;
        }
    }

    public void ShowSuccessFeedback(){
        foreach (Image arrow in activeArrowSlots){
            if (arrow.gameObject.activeSelf) arrow.color = Color.green;
        }
    }

    Sprite GetArrowSprite(HackingInputDirection direction) => direction switch{
        HackingInputDirection.Up => upArrow,
        HackingInputDirection.Down => downArrow,
        HackingInputDirection.Left => leftArrow,
        HackingInputDirection.Right => rightArrow,
        _ => null
    };
}
