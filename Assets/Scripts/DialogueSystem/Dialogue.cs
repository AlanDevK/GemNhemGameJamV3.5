using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using System.Collections.Generic;

[System.Serializable]
public struct Conversation
{
    public string conversationID;
    [TextArea(3,10)]
    public string[] lines;
}

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    [SerializeField] InputActionReference continueDialogueAction;
    public List<Conversation> sceneDialogues = new List<Conversation>();
    public float textSpeed;
    bool _isPlayingDialogue = false;
    [SerializeField] GameObject dialogueArea;
    public event Action OnDialogueComplete;
    bool isDialogueStarted = false;
    public bool isDialogueEnd = false;
    [SerializeField] GameObject continueCursor;

    int index;
    Coroutine typing;
    Coroutine cursor;
    string[] currentLines;

    void Start()
    {
        textComponent.text = string.Empty;
        if (continueCursor != null) continueCursor.SetActive(false);
    }

    void Update()
    {
        if (continueDialogueAction.action.WasPressedThisFrame() && isDialogueStarted)
        {
            if (_isPlayingDialogue)
            {
                if (typing != null) StopCoroutine(typing);
                textComponent.maxVisibleCharacters = textComponent.textInfo.characterCount;
                _isPlayingDialogue = false;
                StartBlinkingCursor();
            }
            else
            {
                NextLine();
            }
        }
    }

    public void PlayConversation(string id)
    {
        bool found = false;
        foreach (Conversation conv in sceneDialogues)
        {
            // FIX 1: Actually check if the ID matches before grabbing the lines
            if (conv.conversationID == id)
            {
                currentLines = conv.lines;
                found = true;
                break;
            }
        }
        
        if (!found)
        {
            Debug.LogWarning($"Conversation with ID '{id}' not found in the database!");
            return; // Stop the code here so it doesn't crash trying to read empty lines
        }
        
        StartDialogue();
    }

    public void StartDialogue()
    {
        gameObject.SetActive(true);
        if (dialogueArea != null) dialogueArea.SetActive(true);
        textComponent.gameObject.SetActive(true);
        index = 0;
        isDialogueStarted = true;
        isDialogueEnd = false;
        if (continueCursor != null) continueCursor.SetActive(false);
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        _isPlayingDialogue = true;
        if (continueCursor != null) continueCursor.SetActive(false);
        textComponent.text = currentLines[index];
        textComponent.maxVisibleCharacters = 0;
        textComponent.ForceMeshUpdate();
        int totalChars = textComponent.textInfo.characterCount;
        int visibleChars = 0;
        
        while (visibleChars <= totalChars)
        {
            textComponent.maxVisibleCharacters = visibleChars;
            visibleChars++;
            yield return new WaitForSeconds(textSpeed);
        }
        
        _isPlayingDialogue = false;
        StartBlinkingCursor();
    }

    void StartBlinkingCursor()
    {
        if (continueCursor == null) return;
        if (cursor != null) StopCoroutine(cursor);
        cursor = StartCoroutine(BlinkCursorRoutine());
    }

    IEnumerator BlinkCursorRoutine()
    {
        while (true)
        {
            continueCursor.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            continueCursor.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void NextLine()
    {
        if (cursor != null) StopCoroutine(cursor);
        if (continueCursor != null) continueCursor.SetActive(false);
        
        if (index < currentLines.Length - 1)
        {
            index++;
            // FIX 2: Assign the Coroutine to 'typing' so Update() can stop it later
            typing = StartCoroutine(TypeLine());
        } 
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        OnDialogueComplete?.Invoke();
        isDialogueStarted = false;
        isDialogueEnd = true;
        gameObject.SetActive(false);
        if (dialogueArea != null) dialogueArea.SetActive(false);
    }

    public void SkipDialogue()
    {
        if (!isDialogueStarted) return;
        if (typing != null) StopCoroutine(typing);
        if (cursor != null) StopCoroutine(cursor);
        _isPlayingDialogue = false;
        EndDialogue();
    }
}