using UnityEngine;
using System.Collections;
using Unity.Cinemachine; // Required for Cinemachine 3.x
using UnityEngine.InputSystem; // Required to pause the New Input System

public class ObjectiveTourTrigger : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The main camera following the player.")]
    public CinemachineCamera playerCamera;
    
    [Tooltip("The cameras placed at each objective.")]
    public CinemachineCamera[] objectiveCameras;

    [Header("Timing")]
    [Tooltip("Time to wait at each objective (does not include travel time).")]
    public float viewDuration = 0.5f;
    [Tooltip("How long the camera takes to travel between points (match this to your Cinemachine Brain default blend).")]
    public float blendTime = 0.5f;
    public float blendToPlayerTime = 4f;

    [Header("Player Control")]
    [Tooltip("Drag your Player GameObject here to disable their input during the sequence.")]
    public PlayerMovement playerInput;

    private bool hasTriggered = false;

    // Use OnTriggerEnter2D(Collider2D other) if your 2.5D game uses 2D physics
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ensure this only fires once and only when the Player touches it
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(PlayTourSequence());
        }
    }

    private IEnumerator PlayTourSequence()
    {
        // 1. Freeze gameplay by disabling the New Input System component
        if (playerInput != null) playerInput.enabled = false;
        
        // Ensure player camera is at its baseline priority
        playerCamera.Priority = 10;

        // 2. Loop through each objective camera
        foreach (CinemachineCamera cam in objectiveCameras)
        {
            // Boost priority to steal the shot
            cam.Priority = 20;

            // Wait for the camera to travel there PLUS the time you want to stare at it
            yield return new WaitForSeconds(blendTime + viewDuration);

            // Drop priority so it stops rendering
            cam.Priority = 0;
        }

        // 3. Wait for the final blend back to the player
        yield return new WaitForSeconds(blendToPlayerTime);

        // 4. Resume gameplay
        if (playerInput != null) playerInput.enabled = true;
    }
}