using UnityEngine;
using UnityEngine.InputSystem;

public class IntroTutorial : MonoBehaviour
{
    [SerializeField] PlayerMovement player;
    [SerializeField] InputActionReference dashAction;
    [SerializeField] InputActionReference continueDialogueAction;
    [SerializeField] Dialogue dialogueSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dashAction.action.Disable();
        player.enabled = false;
        continueDialogueAction.action.Enable();
        dialogueSystem.enabled = true;
        dialogueSystem.OnDialogueComplete += EndTutorial;
    }

    public void EndTutorial()
    {
        dialogueSystem.OnDialogueComplete -= EndTutorial;
        continueDialogueAction.action.Disable();
        dashAction.action.Enable();
        player.enabled = true;
        this.enabled = false;
    }
}
