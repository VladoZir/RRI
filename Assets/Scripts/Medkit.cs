using UnityEngine;

public class Medkit : MonoBehaviour
{
    public int healAmount = 50; // Amount of health restored

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount); // Heal the player
            Destroy(gameObject); // Remove the medkit from the scene
        }
    }
}
