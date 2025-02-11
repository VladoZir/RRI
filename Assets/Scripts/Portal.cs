using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Portal : MonoBehaviour
{
    public float fadeDuration = 1f;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(PlayPortalAnimation());
    }

    private IEnumerator PlayPortalAnimation()
    {
        anim.Play("Portal_Appear");

        // Wait for the appear animation to finish
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.Play("Portal_Loop");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;  
                rb.bodyType = RigidbodyType2D.Kinematic;  
            }

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false; 
            }
            SceneMover sceneMover = Object.FindAnyObjectByType<SceneMover>();
            if (sceneMover != null)
            {
                sceneMover.PauseSceneMovement(true); // Pause the scene movement while teleporting
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

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; 
        }

        SceneManager.LoadScene(nextSceneIndex);
    }
}
