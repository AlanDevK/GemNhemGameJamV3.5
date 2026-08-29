using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class StartAllUI : MonoBehaviour
{
    [SerializeField] GameObject healthBar;
    [SerializeField] GameObject repairBar;
    bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered= true;
            StartCoroutine(OtherUIStart());
        }
    }

    IEnumerator OtherUIStart()
    {
        for (int i = 0; i<3; i++)
        {
            healthBar.SetActive(true);
            repairBar.SetActive(true);
            yield return new WaitForSeconds(0.03f);
            healthBar.SetActive(false);
            repairBar.SetActive(false);
            yield return new WaitForSeconds(0.03f);
        }
        healthBar.SetActive(true);
        repairBar.SetActive(true);
    }
}
