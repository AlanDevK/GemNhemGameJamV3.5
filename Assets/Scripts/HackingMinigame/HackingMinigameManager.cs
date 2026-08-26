using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HackingMinigameManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private HackingInputReader inputReader;
    [SerializeField] private HackingUIController uiController;

    [Header("Round Configuration")]
    [SerializeField] private int totalRounds = 3;
    [SerializeField] private int baseSequenceLength = 3;
    [SerializeField] private float timeLimitPerRound = 5.0f;

    [Header("Penalty / Failure")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;

    public event Action OnMinigameSuccess;
    public event Action OnMinigameFailed;

    private readonly List<HackingDirection> _currentSequence = new();
    private int _currentRound;
    private int _currentStepIndex;
    public float _remainingTime;
    public bool _isActive;

    private void OnEnable(){
        inputReader.OnDirectionInput += HandleDirectionInput;
    }
    private void OnDisable(){
        inputReader.OnDirectionInput -= HandleDirectionInput;
    }

    private void Update()
    {
        if (!_isActive) return;

        _remainingTime -= Time.deltaTime;
        // uiController.UpdateTimer(_remainingTime / timeLimitPerRound);

        if (_remainingTime <= 0f)
        {
            FailMinigame();
        }
    }

    [ContextMenu("Start Minigame")]
    public void StartMinigame()
    {
        uiController.timer.gameObject.SetActive(true);
        uiController.panelRoot.SetActive(true);
        uiController.nodesContainer.gameObject.SetActive(true);
        _currentRound = 0;
        _isActive = true;
        uiController.ShowUI(true);
        AdvanceToNextRound();
    }

    private void AdvanceToNextRound()
    {
        _currentRound++;
        if (_currentRound > totalRounds)
        {
            CompleteMinigame();
            return;
        }

        _currentStepIndex = 0;
        _remainingTime = timeLimitPerRound;

        GenerateSequence(baseSequenceLength + (_currentRound - 1));
        uiController.RenderSequence(_currentSequence);
    }

    private void GenerateSequence(int length)
    {
        _currentSequence.Clear();
        for (int i = 0; i < length; i++)
        {
            var randomDir = (HackingDirection)UnityEngine.Random.Range(0, 4);
            _currentSequence.Add(randomDir);
        }
    }

    private void HandleDirectionInput(HackingDirection inputDir)
    {
        if (!_isActive || _currentSequence.Count == 0) return;

        if (inputDir == _currentSequence[_currentStepIndex])
        {
            uiController.MarkStepCompleted(_currentStepIndex);
            _currentStepIndex++;

            if (_currentStepIndex >= _currentSequence.Count)
            {
                AdvanceToNextRound();
            }
        }
        else
        {
            uiController.MarkStepFailed(_currentStepIndex);
            FailMinigame();
        }
    }

    private void FailMinigame()
    {
        _isActive = false;
        uiController.ShowUI(false);

        if (enemyPrefab != null && playerTransform != null)
        {
            Instantiate(enemyPrefab, playerTransform.position, playerTransform.rotation);
        }

        OnMinigameFailed?.Invoke();
    }

    private void CompleteMinigame()
    {
        _isActive = false;
        uiController.ShowUI(false);
        OnMinigameSuccess?.Invoke();
    }
}