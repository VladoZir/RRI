using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;  
    public int currentHealth;    
    public List<Sprite> healthSprites = new List<Sprite>();
    public GameObject healthContainer;

    public int maxShield = 50;
    public int currentShield;
    public List<Sprite> shieldSprites = new List<Sprite>();
    public GameObject shieldContainer;

    private bool isInvincible = false;
    public float invincibilityDuration = 1f; 
    public float flickerInterval = 0.1f; 
    private SpriteRenderer spriteRenderer;

    public AudioSource hurtAudio;
    public AudioSource deathAudio;
    public AudioSource healAudio;
    public AudioSource shieldPickUpAudio;
    public AudioSource shieldBreakAudio;

    private GameOver gameOverManager;

    public Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        healthContainer = GameObject.Find("HealthBar");

        currentShield = 0;
        shieldContainer = GameObject.Find("ShieldBar");

        spriteRenderer = GetComponent<SpriteRenderer>();

        gameOverManager = FindFirstObjectByType<GameOver>();

        rb.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DeathBarrier"))
        {
            Die();
        }
    }


    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        if (rb != null)
        {
            Vector2 knockbackForce = new Vector2(0, 10f);
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        }

        if (currentShield > 0)
        {
            hurtAudio.Play();
            currentShield -= damage;
            if (currentShield < 0)
            {
                shieldBreakAudio.Play();
                currentShield = 0;
            }

            changeShieldSprite(currentShield);

        }
        else
        {
            hurtAudio.Play();
            currentHealth -= damage;
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            changeHealthSprite(currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ActivateInvincibility()); 
        }
    }

    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true;

        float timer = 0;
        while (timer < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; 
            yield return new WaitForSeconds(flickerInterval);
            timer += flickerInterval;
        }

        spriteRenderer.enabled = true; 
        isInvincible = false;
    }

    public void changeShieldSprite(int curShield)
    {
        int shieldPercentage = (curShield * 100) / maxShield;
        int spriteIndex = Mathf.FloorToInt(shieldPercentage / 10); // Za svakih 5%
        spriteIndex = Mathf.Clamp(spriteIndex, 0, shieldSprites.Count - 1);

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
        //Debug.Log("Player has died!");
        deathAudio.Play();
        StartCoroutine(WaitForAudioToFinish());
    }
    private IEnumerator WaitForAudioToFinish()
    {
        yield return new WaitForSeconds(deathAudio.clip.length);  
        gameObject.SetActive(false);
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOverScreen();
        }
        Time.timeScale = 0f;
    }

    public void Heal(int amount)
    {
        healAudio.Play();
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        changeHealthSprite(currentHealth);
    }

    public void RestoreShield(int amount)
    {
        shieldPickUpAudio.Play();
        currentShield = Mathf.Min(currentShield + amount, maxShield);

        changeShieldSprite(currentShield);
    }
}
