using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;  // Max health value
    public int currentHealth;    // Current health value
    public Slider healthSlider;
    public Image healthImage;
    public List<Sprite> healthSprites = new List<Sprite>();
    public GameObject healthContainer;

    void Start()
    {
        // Initialize health at the start
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;  // Set the slider max value to match max health
            healthSlider.value = currentHealth; // Initialize slider value with the current health
        }
        healthContainer = GameObject.Find("HealthBar");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        changeHealthSprite(currentHealth);

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

    public void changeHealthSprite(int curHealth)
    {
        if (curHealth == 100)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[10];

        }
        else if (curHealth == 90)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[9];
        }
        if (curHealth == 80)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[8];
        }
        else if (curHealth == 70)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[7];
        }
        if (curHealth == 60)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[6];
        }
        else if (curHealth == 50)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[5];
        }
        if (curHealth == 40)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[4];
        }
        else if (curHealth == 30)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[3];
        }
        if (curHealth == 20)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[2];
        }
        else if (curHealth == 10)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[1];
        }
        else if (curHealth == 0)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[0];
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

        // Update the UI slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // Update health sprite
        changeHealthSprite(currentHealth);
    }
}
