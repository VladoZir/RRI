using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // If teleporting to a new scene

public class Portal : MonoBehaviour
{
    public float fadeDuration = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;  // Stop movement
                rb.bodyType = RigidbodyType2D.Kinematic;  // Make player unaffected by gravity
            }

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;  // Disable player controls
            }

            StartCoroutine(FadeOutAndTeleport(other.gameObject));
        }
    }

    private IEnumerator FadeOutAndTeleport(GameObject player)
    {
        // Get all SpriteRenderers on the player and its children
        SpriteRenderer[] spriteRenderers = player.GetComponentsInChildren<SpriteRenderer>();

        float timer = 0f;

        // Store original colors
        Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            originalColors[sr] = sr.color;
        }

        // Gradually fade out all SpriteRenderers
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            foreach (SpriteRenderer sr in spriteRenderers)
            {
                sr.color = new Color(originalColors[sr].r, originalColors[sr].g, originalColors[sr].b, alpha);
            }

            yield return null; // Wait for next frame
        }

        // Ensure all objects are fully invisible
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = new Color(originalColors[sr].r, originalColors[sr].g, originalColors[sr].b, 0f);
        }

        // Optional: Wait a bit before teleporting
        //yield return new WaitForSeconds(0.5f);

        // Deactivate player (or teleport)
        //player.SetActive(false);

        // Option 2: Load next scene (if transitioning levels)
         SceneManager.LoadScene("Level2"); // Uncomment if needed
    }
}
