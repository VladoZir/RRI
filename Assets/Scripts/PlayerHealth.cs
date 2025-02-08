using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;  // Max health value
    public int currentHealth;    // Current health value
    public List<Sprite> healthSprites = new List<Sprite>();
    public GameObject healthContainer;

    public int maxShield = 50;
    public int currentShield;
    public List<Sprite> shieldSprites = new List<Sprite>();
    public GameObject shieldContainer;

    private bool isInvincible = false;
    public float invincibilityDuration = 1.5f; // Time in seconds
    public float flickerInterval = 0.1f; // Flicker speed
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Initialize health at the start
        currentHealth = maxHealth;
        healthContainer = GameObject.Find("HealthBar");

        currentShield = 0;
        shieldContainer = GameObject.Find("ShieldBar");

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        if (currentShield > 0)
        {
            currentShield -= damage;
            if (currentShield < 0)
            {
                currentShield = 0;
            }

            changeShieldSprite(currentShield);

        }
        else
        {
            currentHealth -= damage;
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            changeHealthSprite(currentHealth);

            // You can add more behavior here, like triggering death when health reaches 0
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        StartCoroutine(ActivateInvincibility()); // Start invincibility frames
    }

    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true;

        float timer = 0;
        while (timer < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Toggle visibility for flicker effect
            yield return new WaitForSeconds(flickerInterval);
            timer += flickerInterval;
        }

        spriteRenderer.enabled = true; // Ensure player is visible at the end
        isInvincible = false;
    }

    public void changeShieldSprite(int curShield)
    {
        // Izračunaj postotak od 50
        int shieldPercentage = (curShield * 100) / maxShield;
        // Na temelju postotka, odaberi odgovarajući sprite
        int spriteIndex = Mathf.FloorToInt(shieldPercentage / 10); // Za svakih 5%
        // Osiguraj da je spriteIndex u granicama (0 do 10 za 11 spriteova)
        spriteIndex = Mathf.Clamp(spriteIndex, 0, shieldSprites.Count - 1);
        // Postavi sprite za shield
        shieldContainer.GetComponent<Image>().sprite = shieldSprites[spriteIndex];
        // Debug.Log("Shield: " + curShield + " | Sprite Index: " + spriteIndex);
    }

    public void changeHealthSprite(int curHealth)
    {
        int index = Mathf.Clamp(curHealth / 10, 0, healthSprites.Count - 1);
        healthContainer.GetComponent<Image>().sprite = healthSprites[index];

    }

    private void Die()
    {
        // Logic for when the player dies, for example:
        Debug.Log("Player has died!");
        // Optionally you can disable the player or restart the scene here
        gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Prevent overhealing

        // Update health sprite
        changeHealthSprite(currentHealth);
    }

    public void RestoreShield(int amount)
    {
        currentShield = Mathf.Min(currentShield + amount, maxShield);

        changeShieldSprite(currentShield);
    }
}
