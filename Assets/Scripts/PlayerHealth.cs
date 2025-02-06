using NUnit.Framework;
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

    void Start()
    {
        // Initialize health at the start
        currentHealth = maxHealth;
        healthContainer = GameObject.Find("HealthBar");

        currentShield = 0;
        shieldContainer = GameObject.Find("ShieldBar");
    }

    public void TakeDamage(int damage)
    {
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

        Debug.Log("Shield: " + curShield + " | Sprite Index: " + spriteIndex);
    }

    public void changeHealthSprite(int curHealth)
    {
        switch (curHealth)
        {
            case 100:
                healthContainer.GetComponent<Image>().sprite = healthSprites[10];
                break;
            case 90:
                healthContainer.GetComponent<Image>().sprite = healthSprites[9];
                break;
            case 80:
                healthContainer.GetComponent<Image>().sprite = healthSprites[8];
                break;
            case 70:
                healthContainer.GetComponent<Image>().sprite = healthSprites[7];
                break;
            case 60:
                healthContainer.GetComponent<Image>().sprite = healthSprites[6];
                break;
            case 50:
                healthContainer.GetComponent<Image>().sprite = healthSprites[5];
                break;
            case 40:
                healthContainer.GetComponent<Image>().sprite = healthSprites[4];
                break;
            case 30:
                healthContainer.GetComponent<Image>().sprite = healthSprites[3];
                break;
            case 20:
                healthContainer.GetComponent<Image>().sprite = healthSprites[2];
                break;
            case 10:
                healthContainer.GetComponent<Image>().sprite = healthSprites[1];
                break;
            case 0:
                healthContainer.GetComponent<Image>().sprite = healthSprites[0];
                break;
        }

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

        changeShieldSprite(currentHealth);
    }
}
