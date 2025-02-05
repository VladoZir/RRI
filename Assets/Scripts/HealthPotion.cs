using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healAmount = 20; // Amount of health restored

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            if (playerHealth.currentHealth < playerHealth.maxHealth) // Only heal if not at max
            {
                playerHealth.Heal(healAmount); // Heal the player
                Destroy(gameObject); // Remove the potion from the scene
            }
            else
            {
                Debug.Log("Health is already full! Potion not used."); // Debug message
            }
        }
    }
}