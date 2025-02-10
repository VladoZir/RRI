using UnityEngine;
using UnityEngine.SceneManagement;

public class BowCollectible : MonoBehaviour
{

    [SerializeField] private GameObject playerWithBowPrefab;
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
            GameManager.Instance.UpgradePlayer(playerWithBowPrefab, other.gameObject);
            Destroy(gameObject);
        }
    }
}
