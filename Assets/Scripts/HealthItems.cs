using UnityEngine;

public class HealthItems : MonoBehaviour
{
    public int potionHealAmount = 20;  // Heal amount for health potion
    public int medkitHealAmount = 50;  // Heal amount for medkit
    public int shieldPotionAmount = 50;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Provjera je li objekt s kojim je došlo do kontakta označen tagom "Player"
        if (other.CompareTag("Player"))
        {
            // Pokušaj dobiti PlayerHealth komponentu na igraču
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                if (playerHealth.currentShield < playerHealth.maxShield)
                {
                    if (CompareTag("ShieldPotion"))
                    {
                        playerHealth.RetoreShield(shieldPotionAmount);
                        Destroy(gameObject);
                        Debug.Log("Shield Potion Used! Current Shield: " + playerHealth.currentShield);
                    }
                }

                // Provjera da li igračovo zdravlje nije već maksimalno
                if (playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    // Provjeriti tip itema i primijeniti odgovarajuće liječenje
                    if (CompareTag("HealthPotion"))
                    {
                        playerHealth.Heal(potionHealAmount);  // Liječenje pomoću potiona
                        Destroy(gameObject);  // Uništiti health potion
                        Debug.Log("Health Potion Used! Current Health: " + playerHealth.currentHealth);
                    }
                    else if (CompareTag("Medkit"))
                    {
                        playerHealth.Heal(medkitHealAmount);  // Liječenje pomoću medkita
                        Destroy(gameObject);  // Uništiti medkit
                        Debug.Log("Medkit Used! Current Health: " + playerHealth.currentHealth);
                    }
                }
                else
                {
                    // Ako je zdravlje već puno
                    Debug.Log("Health is already full! Item not used.");
                }
            }
            else
            {
                // Ako se ne može naći PlayerHealth komponenta
                Debug.Log("No PlayerHealth component found! Item not used.");
            }
        }
    }
}
