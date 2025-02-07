using UnityEngine;
using UnityEngine.SceneManagement;

public class BowCollectible : MonoBehaviour
{

    [SerializeField] private GameObject playerWithBowPrefab;
    [SerializeField] private float floatSpeed = 1f;  // Speed of floating
    [SerializeField] private float floatHeight = 0.2f;  // Height of floating movement
    //[SerializeField] private AudioClip collectSound; // Sound effect on pickup

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Move up and down using sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure the player has the "Player" tag
        {

            /*
            // Play sound if assigned
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            */

            GameManager.Instance.UpgradePlayer(playerWithBowPrefab, other.gameObject);
            Destroy(gameObject);
        }
    }
}
