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

            StartCoroutine(FadeOutAndTeleport(other.gameObject));
        }
        else if (other.CompareTag("PlayerSpaceship"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            FlyingController flyingController = other.GetComponent<FlyingController>();
            if (flyingController != null)
            {
                flyingController.enabled = false;
            }
            SceneMover sceneMover = Object.FindAnyObjectByType<SceneMover>();
            if (sceneMover != null)
            {
                sceneMover.PauseSceneMovement(true); // Pause the scene movement while teleporting
            }

            StartCoroutine(PullShipIn(rb, other.transform));
        }
    }

    private IEnumerator PullShipIn(Rigidbody2D rb, Transform shipTransform)
    {
        Vector2 targetPosition = transform.position; // The center of the teleport area
        float pullSpeed = 3f; // Adjust pull speed as needed

        while (Vector2.Distance(shipTransform.position, targetPosition) > 0.1f)
        {
            shipTransform.position = Vector2.MoveTowards(shipTransform.position, targetPosition, pullSpeed * Time.deltaTime);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(FadeOutAndTeleport(shipTransform.gameObject));
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
