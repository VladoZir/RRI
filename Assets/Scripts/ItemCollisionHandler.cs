using UnityEngine;

public class ItemCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if item hit the ground (assumes the ground has the tag "Ground")
        if (collision.gameObject.CompareTag("Ground"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Destroy(rb); // Remove Rigidbody2D so it stops moving
            }

            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.isTrigger = true; // Allow player to walk through it
            }

            // Destroy this script since it's no longer needed
            Destroy(this);
        }
    }
}
