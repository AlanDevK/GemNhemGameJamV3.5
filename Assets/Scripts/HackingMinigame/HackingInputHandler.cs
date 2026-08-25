using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class HackingInputHandler : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] int sequenceLength =  5;
    [SerializeField] int totalRounds = 3;
    [SerializeField] float timeLimitPerRound = 6f;

    [Header("Dependencies")]
    [SerializeField] HackingUI uiController;

    [Header("Enemy Spawner Target")]
    [SerializeField] Transform playerTransform;
    [SerializeField] GameObject enemyPrefab;

    public event Action OnMinigameSuccess;
    public event Action OnMinigameFailed;

    readonly List<HackingInputDirection> currentSequence = new List<HackingInputDirection>(8);
    int currentStepIndex;
    int currentRound;
    float remainingTime;
    bool isGameActive;

    void Update(){
        if (!isGameActive) return;
        remainingTime -= Time.deltaTime;
        uiController.UpdateTimer(remainingTime / timeLimitPerRound);
        if (remainingTime <= 0f){
            HandleFailure();
        }
    }

    void StartMinigame(){
        currentRound = 0;
        isGameActive = true;
        StartNextRound();
    }

    void StartNextRound(){
        currentRound++;
        if (currentRound > totalRounds){
            HandleSuccess();
            return;
        }
        currentStepIndex = 0;
        remainingTime = timeLimitPerRound;
        GenerateRandomSequence();
        uiController.DisplaySequence(currentSequence, currentRound, totalRounds);
    }

    void GenerateRandomSequence(){
        currentSequence.Clear();
        for (int i = 0; i<sequenceLength; i++){
            var randomDir = (HackingInputDirection)UnityEngine.Random.Range(0,4);
            currentSequence.Add(randomDir);
        }
    }

    public void OnNavigate(InputValue value){
        if (!isGameActive) return;
        Vector2 input = value.Get<Vector2>();
        if (input.sqrMagnitude < 0.25f) return;
        HackingInputDirection pressedDir;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)){
            pressedDir = input.x > 0 ? HackingInputDirection.Right : HackingInputDirection.Left;
        } else pressedDir = input.y > 0 ? HackingInputDirection.Up : HackingInputDirection.Down;
        ValidateInput(pressedDir);
    }

    void ValidateInput(HackingInputDirection pressedDir){
        if (currentSequence[currentStepIndex] == pressedDir){
            uiController.MarkStepCompleted(currentStepIndex);
            currentStepIndex++;
        } if (currentStepIndex >= currentSequence.Count) StartNextRound();
        else HandleFailure();
    }

    void HandleFailure(){
        isGameActive = false;
        uiController.ShowFailureFeedback();
        if (enemyPrefab != null && playerTransform != null){
            Instantiate(enemyPrefab, playerTransform.position, playerTransform.rotation);
        } OnMinigameFailed?.Invoke();
    }

    void HandleSuccess(){
        isGameActive = false;
        uiController.ShowSuccessFeedback();
        OnMinigameSuccess?.Invoke();
    }
}
