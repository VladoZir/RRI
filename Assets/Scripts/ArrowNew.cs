using System.Collections;
using UnityEngine;

public class ArrowNew : MonoBehaviour
{
    Rigidbody2D rb;
    bool hasHit;
    public int damage = 10;

    public AudioSource arrowHitAudio;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (hasHit == false)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasHit = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            arrowHitAudio.Play();
            IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); 
            }

            StartCoroutine(DestroyAfterSound()); 
        }
        else
        {
            Destroy(gameObject); 
        }

    }

    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(arrowHitAudio.clip.length);
        Destroy(gameObject);
    }


}
