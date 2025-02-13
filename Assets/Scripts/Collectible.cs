using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectible : MonoBehaviour
{

    [SerializeField] private GameObject playerWithBowPrefab;
    [SerializeField] private GameObject playerWithSwordPrefab;
    [SerializeField] private float floatSpeed = 1f;  
    [SerializeField] private float floatHeight = 0.2f;  

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
        if (other.CompareTag("Player")) 
        {
            if (CompareTag("BowCollectible"))
            {
                GameManager.Instance.UpgradePlayerBow(playerWithBowPrefab, other.gameObject);
                Destroy(gameObject);
            }
            if (CompareTag("SwordCollectible"))
            {
                GameManager.Instance.UpgradePlayerSword(playerWithSwordPrefab, other.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
