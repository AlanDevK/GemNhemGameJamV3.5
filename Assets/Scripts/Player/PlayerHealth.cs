using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private HealthBarUI healthBarUI;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Auto-reference health bar UI in scene
        healthBarUI = FindObjectOfType<HealthBarUI>();
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0) Die();
    }

    void UpdateHealthUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.SetHealth(currentHealth, maxHealth);
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // TODO: Handle game over or scene reload
    }
}