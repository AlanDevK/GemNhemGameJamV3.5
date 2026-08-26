using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HackingUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelRoot;
    public Transform nodesContainer; // Has HorizontalLayoutGroup
    [SerializeField] private HackingStepNode stepNodePrefab;
    public TextMeshProUGUI timer;
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] HackingMinigameManager gameManager;

    private readonly List<HackingStepNode> _activeNodes = new();

    void Update(){
        if (gameManager != null){
            if (gameManager._remainingTime > 0){
                timer.text = gameManager._remainingTime.ToString("0.00");
            }
        }
    }

    public void ShowUI(bool isVisible){
        panelRoot.SetActive(isVisible);
        timer.gameObject.SetActive(isVisible);
        nodesContainer.gameObject.SetActive(isVisible);
    }
    // public void UpdateTimer(float normalizedTime)
    // {
    //     if (timerSlider != null)
    //         timerSlider.value = Mathf.Clamp01(normalizedTime);
    // }

    public void RenderSequence(IReadOnlyList<HackingDirection> sequence)
    {
        ClearNodes();
        for (int i = 0; i < sequence.Count; i++)
        {
            HackingStepNode node = Instantiate(stepNodePrefab, nodesContainer);
            node.Setup(sequence[i], arrowSprite);
            _activeNodes.Add(node);
        }
    }

    public void MarkStepCompleted(int index)
    {
        if (index >= 0 && index < _activeNodes.Count)
            _activeNodes[index].SetCompleted();
    }

    public void MarkStepFailed(int index)
    {
        if (index >= 0 && index < _activeNodes.Count)
            _activeNodes[index].SetFailed();
    }

    private void ClearNodes()
    {
        foreach (var node in _activeNodes)
        {
            if (node != null) Destroy(node.gameObject);
        }
        _activeNodes.Clear();
    }
}