using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class InGameplayDialogue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textComponent;
    [SerializeField] string[] lines;
    [SerializeField] float textSpeed;
    [SerializeField] GameObject dialogueArea;
    [SerializeField] float timeBetweenLines;
    int index;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (textComponent.text == lines[index] && !_isPlayingDialogue){
    //         NextLine();
    //     } else{
    //         StopAllCoroutines();
    //         textComponent.text = lines[index];
    //     }
    // }

    void StartDialogue(){
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine(){
        foreach (char c in lines[index].ToCharArray()){
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        yield return new WaitForSeconds(timeBetweenLines);
        NextLine();
    }

    void NextLine(){
        if (index < lines.Length - 1){
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        } else {
            gameObject.SetActive(false);
            dialogueArea.SetActive(false);
        }
    }
}
