using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;  // Max health value
    public int currentHealth;    // Current health value
    public Slider healthSlider;

    void Start()
    {
        // Initialize health at the start
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;  // Set the slider max value to match max health
            healthSlider.value = currentHealth; // Initialize slider value with the current health
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // Update the slider value when the player takes damage
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // You can add more behavior here, like triggering death when health reaches 0
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Logic for when the player dies, for example:
        Debug.Log("Player has died!");
        // Optionally you can disable the player or restart the scene here
        gameObject.SetActive(false);
    }
}
