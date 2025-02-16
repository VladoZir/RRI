using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    Rigidbody2D rb;
    public int damage = 10;
    public float lifetime = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        Destroy(gameObject, lifetime);
        //Debug.Log($"Projectile started with collision layer: {gameObject.layer}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"Collision detected with: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        
            if (collision.gameObject.CompareTag("Player"))
            {
               
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    //Debug.Log($"PlayerHealth found. Current health before damage: {playerHealth.currentHealth}");
                    playerHealth.TakeDamage(damage);
                    //Debug.Log("Damage method called");
                }
            }
        

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject);
    }
}