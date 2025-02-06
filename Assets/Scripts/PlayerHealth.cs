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

    void Start()
    {
        // Initialize health at the start
        currentHealth = maxHealth;
        healthContainer = GameObject.Find("HealthBar");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        changeHealthSprite(currentHealth);

        // You can add more behavior here, like triggering death when health reaches 0
        if (currentHealth <= 0)
        {
            Die();
        }
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
}
