using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private GameObject playerWithBowPrefab;
    [SerializeField] private GameObject playerWithSwordPrefab;
    [SerializeField] private GameObject playerWithSpaceGunPrefab;

    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 0.2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Floating effect
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
            }
            else if (CompareTag("SwordCollectible"))
            {
                GameManager.Instance.UpgradePlayerSword(playerWithSwordPrefab, other.gameObject);
            }
            else if (CompareTag("SpaceGunCollectible")) 
            {
                GameManager.Instance.UpgradePlayerSpaceGun(playerWithSpaceGunPrefab, other.gameObject);
            }

            Destroy(gameObject); 
        }
    }
}
