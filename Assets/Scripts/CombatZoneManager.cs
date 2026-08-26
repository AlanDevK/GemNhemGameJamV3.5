using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
public class CombatZoneManager : MonoBehaviour
{
    public static CombatZoneManager Instance {get; private set;}

    [Header("Cinemachine Reference")]
    [SerializeField] CinemachineCamera cineCamera;

    [Header("Targets")]
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTargetTransform;

    readonly HashSet<EnemyAI> activeEnemiesInZone = new HashSet<EnemyAI>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SetCameraFollowTarget(playerTransform);
    }

    public void RegisterEnemy(EnemyAI enemy)
    {
        if (activeEnemiesInZone.Add(enemy)) UpdateCameraTarget();
    }

    public void UnregisterEnemy(EnemyAI enemy)
    {
        if (activeEnemiesInZone.Remove(enemy)) UpdateCameraTarget();
    }

    void UpdateCameraTarget()
    {
        if (cineCamera == null) return;
        if (activeEnemiesInZone.Count > 0)
        {
            SetCameraFollowTarget(cameraTargetTransform);
        } else SetCameraFollowTarget(playerTransform);
    }

    void SetCameraFollowTarget(Transform target)
    {
        if (cineCamera != null && target != null) cineCamera.Follow = target;
    }
}
