using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float destroyTime = 1f; // Time to explode (secs)

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}