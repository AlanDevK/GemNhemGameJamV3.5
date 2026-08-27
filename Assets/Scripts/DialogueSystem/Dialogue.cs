using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    [SerializeField] InputActionReference continueDialogueAction;
    public string[] lines;
    public float textSpeed;
    bool _isPlayingDialogue = false;
    [SerializeField] GameObject dialogueArea;
    public event Action OnDialogueComplete;

    int index;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (continueDialogueAction.action.WasPressedThisFrame() && !_isPlayingDialogue){
            if (textComponent.text == lines[index]) {
                NextLine();
            }
            else{
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue(){
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine(){
        _isPlayingDialogue = true;
        foreach (char c in lines[index].ToCharArray()){
            textComponent.text+=c;
            yield return new WaitForSeconds(textSpeed);
        }
        _isPlayingDialogue = false;
    }

    void NextLine(){
        if (index < lines.Length - 1){
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        } else{
            OnDialogueComplete?.Invoke();
            gameObject.SetActive(false);
            dialogueArea.SetActive(false);
        }
    }
}
