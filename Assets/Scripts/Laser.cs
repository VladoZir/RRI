using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    Rigidbody2D rb;
    bool hasHit;
    public int damage = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public AudioSource laserHitAudio;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Disable gravity for the laser

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasHit = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Make the laser invisible and disable its collider
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            laserHitAudio.Play();
            IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Call TakeDamage for any enemy type
            }

            StartCoroutine(DestroyAfterSound()); // Wait for audio before destroying
        }
        else
        {
            Destroy(gameObject); // If not an enemy, destroy immediately
        }

        //Debug.Log(collision.transform.name);
    }

    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(laserHitAudio.clip.length); // Wait for the audio to finish
        Destroy(gameObject);
    }


}
