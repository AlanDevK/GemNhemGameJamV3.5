using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class IntroTutorial : MonoBehaviour
{
    [SerializeField] PlayerMovement player;
    [SerializeField] InputActionReference dashAction;
    [SerializeField] InputActionReference continueDialogueAction;
    [SerializeField] Volume postProcessingVolume;
    [SerializeField] VolumeProfile defaultProfile;

    [SerializeField] Dialogue dialogueSystem;
    [Header("Calling UI")]
    [SerializeField] GameObject callButton;
    [SerializeField] GameObject pressStartText;

    [Header("Dialogue Transition")]
    bool isDialogueStarted = false;
    bool isDialogueReady = false;
    [SerializeField] float volumeLerp;
    DepthOfField dop;
    [SerializeField] int targetDOP;
    int originalDOP = 1;

    [Header("Dialogue UI")]
    [SerializeField] GameObject dialogueArea;
    [SerializeField] GameObject dialogueAll;
    [SerializeField] GameObject caller1Portrait;
    [SerializeField] GameObject caller2Portrait;
    [SerializeField] GameObject timerGameObject;
    [SerializeField] GameObject caller1PortraitBorder;
    [SerializeField] GameObject caller2PortraitBorder;
    [SerializeField] GameObject skipButton;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] Timer timer;

    [Header("Dialogue Portraits")]
    [SerializeField] GameObject caller1PortraitClosed;
    [SerializeField] GameObject caller1PortraitOpen;
    [SerializeField] GameObject caller2PortraitClosed;
    [SerializeField] GameObject caller2PortraitOpen;
    [SerializeField] float flapSpeed = 0.1f;
    [SerializeField] int charactersPerFlap = 2;

    [Header("Game Start UI")]
    [SerializeField] GameObject gameStartText;
    [SerializeField] GameObject gameStartBorder;
    [SerializeField] Animator gameStartEntryAnimator;

    [Header("Buttons UI")]
    [SerializeField] GameObject wButton;
    [SerializeField] GameObject aButton;
    [SerializeField] GameObject sButton;
    [SerializeField] GameObject dButton;
    [SerializeField] GameObject spaceButton;
    [SerializeField] GameObject leftMouseButton;


    [Header("Camera")]
    [SerializeField] CinemachineCamera cam;
    int dialoguePhase = 0;

    [SerializeField] GameObject traversalTrigger;
    [SerializeField] GameObject startTrigger;
    [SerializeField] AudioClip blipCallSound;
    AudioSource gameManagerAudio;
    
    // We need a variable to store the exact running coroutine so we can stop it later
    Coroutine flickerCoroutine; 
    Coroutine portraitRoutine;

    void Awake()
    {
        LeanTween.reset();
    }
    
    void Start()
    {
        gameManagerAudio = GetComponent<AudioSource>();
        dashAction.action.Disable();
        player.enabled = false;
        continueDialogueAction.action.Enable();
        dialogueSystem.enabled = true;
        postProcessingVolume.profile = defaultProfile;
        postProcessingVolume.profile.TryGet(out dop);
        dialogueSystem.OnDialogueComplete += HandleDialogueComplete;
        dop.focalLength.Override(originalDOP);
        
        // Turn the UI on ONCE when the scene starts, not every frame
        callButton.SetActive(true);
        // Start the coroutine and save it to our variable
        flickerCoroutine = StartCoroutine(PressStartFlicker()); 
    }

    void OnEnable()
    {
        dialogueSystem.OnSpeakerChanged += HandleSpeakerChanged;
    }
    
    void OnDisable()
    { 
        dialogueSystem.OnSpeakerChanged -= HandleSpeakerChanged;
    }

    void HandleSpeakerChanged(string speakerName)
    {
        if (portraitRoutine != null) StopCoroutine(portraitRoutine);
        if (speakerName == "Pruner")
        {
            portraitRoutine = StartCoroutine(AnimatePortrait(caller1PortraitOpen, caller1PortraitClosed));
        }
        else
        {
            portraitRoutine = StartCoroutine(AnimatePortrait(caller2PortraitOpen, caller2PortraitClosed));
        }
    }
    void Update()
    {
        // Check for controller/keyboard input
        if (!isDialogueStarted && continueDialogueAction.action.WasPressedThisFrame())
        {
            BeginCall();
        }
        else if (isDialogueStarted && isDialogueReady)
        {
            timer.StartTimer();
        }
    }

    // NEW METHOD: Connect your UI Button 'On Click ()' event to this method!
    public void OnCallButtonClicked()
    {
        if (!isDialogueStarted)
        {
            BeginCall();
        }
    }

    // We extract the transition logic here so both the Button and the Keyboard can trigger it
    void BeginCall()
    {
        isDialogueStarted = true;
        
        // Stop the exact coroutine instance we saved earlier
        if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
        
        callButton.SetActive(false);
        pressStartText.SetActive(false);
        
        StartCoroutine(StartTutorialDialogue());
    }

    void HandleDialogueComplete()
    {
        StartCoroutine(EndTutorialCoroutine());
    }

    IEnumerator AnimatePortrait(GameObject mouthOpen, GameObject mouthClosed)
    {
        float syncSpeed = dialogueSystem.textSpeed * charactersPerFlap;
        while (dialogueSystem.IsTyping)
        {
            mouthClosed.SetActive(false);
            mouthOpen.SetActive(true);
            yield return new WaitForSeconds(syncSpeed);
            mouthOpen.SetActive(false);
            mouthClosed.SetActive(true);
            yield return new WaitForSeconds(syncSpeed);
        }
        mouthOpen.SetActive(false);
        mouthClosed.SetActive(true);
    }
    IEnumerator StartTutorialDialogue()
    {
        float timeElapsed = 0f;
        float transitionDuration = 1f;
        StartCoroutine(StartDialogueBoxAnimation());
        while (timeElapsed < transitionDuration)
        {
            float t = timeElapsed / transitionDuration;
            dop.focalLength.Override(Mathf.Lerp(originalDOP, targetDOP, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        dialogueBox.SetActive(true);
        dop.focalLength.Override(targetDOP);
        yield return new WaitForSeconds(0.02f);
        isDialogueReady = true;
        dialogueAll.GetComponent<UITiltEffect>().enabled = true;
        dialogueSystem.PlayConversation("Conversation 1");
    }

    IEnumerator StartDialogueBoxAnimation()
    {
        dialogueArea.SetActive(true);
        LeanTween.cancel(dialogueArea);
        RectTransform rect = dialogueArea.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -1093f);
        dialogueArea.LeanMoveLocalY(3.8f, 1f).setEaseInOutQuart();
        yield return new WaitForSeconds(1f);
        for (int i = 0; i<3; i++)
        {
            caller1Portrait.SetActive(true);
            caller2Portrait.SetActive(true);
            timerGameObject.SetActive(true);
            skipButton.SetActive(true);
            yield return new WaitForSeconds(0.03f);
            caller1Portrait.SetActive(false);
            caller2Portrait.SetActive(false);
            timerGameObject.SetActive(false);
            skipButton.SetActive(false);
            yield return new WaitForSeconds(0.03f);
        }
        caller1Portrait.SetActive(true);
        caller2Portrait.SetActive(true);
        timerGameObject.SetActive(true);
        skipButton.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        caller1PortraitBorder.SetActive(true);
        caller2PortraitBorder.SetActive(true);
        for (int i = 0; i<3; i++)
        {
            caller1PortraitClosed.SetActive(true);
            caller2PortraitClosed.SetActive(true);
            yield return new WaitForSeconds(0.03f);
            caller1PortraitClosed.SetActive(false);
            caller2PortraitClosed.SetActive(false);
            yield return new WaitForSeconds(0.03f);
        }
        caller1PortraitClosed.SetActive(true);
        caller2PortraitClosed.SetActive(true);
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator EndTutorialCoroutine()
    {
        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            dop.focalLength.Override(Mathf.Lerp(targetDOP, originalDOP, timeElapsed));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        StartGame();
    }
    
    IEnumerator PressStartFlicker()
    {
        while (true)
        {
            pressStartText.SetActive(true);
            gameManagerAudio.PlayOneShot(blipCallSound);
            yield return new WaitForSeconds(0.5f);
            pressStartText.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void StartGame()
    { 
        StartCoroutine(GameStartAnimation());
        continueDialogueAction.action.Disable();
        dashAction.action.Enable();
        player.enabled = true;
        this.enabled = false;
    }

    IEnumerator GameStartAnimation()
    {
        StartCoroutine(FlickeringGameStartBorder());
        yield return new WaitForSeconds(0.5f);
        gameStartText.SetActive(true);
        gameStartEntryAnimator.Play("StartText");
        yield return new WaitForSeconds(1f);
        gameStartEntryAnimator.Play("StartTextExit");
        StartCoroutine(FlickeringGameExitBorder());
        startTrigger.SetActive(true);
        yield return new WaitForSeconds(1f);
        StartCoroutine(FlickeringButtons());
        yield return new WaitForSeconds(10f);
        StartCoroutine(FlickeringButtonsExit());
        yield return new WaitForSeconds(2f);
        StartCoroutine(FlickeringSpace());
        yield return new WaitForSeconds(10f);
        StartCoroutine(FlickeringSpaceExit());
        yield return new WaitForSeconds(2f);
        StartCoroutine(FlickeringLeftMouse());
        yield return new WaitForSeconds(10f);
        StartCoroutine(FlickeringLeftMouseExit());
        yield return new WaitForSeconds(2f);
        traversalTrigger.SetActive(true);
    }

    IEnumerator FlickeringGameStartBorder()
    {
        for (int i = 0; i<3; i++)
        {
            gameStartBorder.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            gameStartBorder.SetActive(false);
            yield return new WaitForSeconds(0.05f);
        }
        gameStartBorder.SetActive(true);
    }

    IEnumerator FlickeringGameExitBorder()
    {
        for (int i = 0; i<3; i++)
        {
            gameStartBorder.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            gameStartBorder.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        gameStartBorder.SetActive(false);
    }

    IEnumerator FlickeringButtons()
    {
        for (int i = 0; i<3; i++)
        {
            wButton.SetActive(true);
            aButton.SetActive(true);
            sButton.SetActive(true);
            dButton.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            wButton.SetActive(false);
            aButton.SetActive(false);
            sButton.SetActive(false);
            dButton.SetActive(false);
            yield return new WaitForSeconds(0.05f);
        }
        wButton.SetActive(true);
        aButton.SetActive(true);
        sButton.SetActive(true);
        dButton.SetActive(true);
    }

    IEnumerator FlickeringButtonsExit()
    {
        for (int i = 0; i<3; i++)
        {
            wButton.SetActive(false);
            aButton.SetActive(false);
            sButton.SetActive(false);
            dButton.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            wButton.SetActive(true);
            aButton.SetActive(true);
            sButton.SetActive(true);
            dButton.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        wButton.SetActive(false);
        aButton.SetActive(false);
        sButton.SetActive(false);
        dButton.SetActive(false);
    }

    IEnumerator FlickeringSpace()
    {
        for (int i = 0; i<3; i++)
        {
            spaceButton.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            spaceButton.SetActive(false);
            yield return new WaitForSeconds(0.05f);
        }
        spaceButton.SetActive(true);
    }

    IEnumerator FlickeringSpaceExit()
    {
        for (int i = 0; i<3; i++)
        {
            spaceButton.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            spaceButton.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        spaceButton.SetActive(false);
    }

    IEnumerator FlickeringLeftMouse()
    {
        for (int i = 0; i<3; i++)
        {
            leftMouseButton.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            leftMouseButton.SetActive(false);
            yield return new WaitForSeconds(0.05f);
        }
        leftMouseButton.SetActive(true);
    }

    IEnumerator FlickeringLeftMouseExit()
    {
        for (int i = 0; i<3; i++)
        {
            leftMouseButton.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            leftMouseButton.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        leftMouseButton.SetActive(false);
    }
}