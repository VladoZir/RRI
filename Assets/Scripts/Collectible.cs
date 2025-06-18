using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private GameObject playerWithBowPrefab;
    [SerializeField] private GameObject playerWithSwordPrefab;
    [SerializeField] private GameObject playerWithSpaceGunPrefab;

    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 0.2f;

    [SerializeField] private int levelNumber;    
    [SerializeField] private int collectibleIndex;

    private Vector3 startPosition;

    private string GetKey()
    {
        // Jedinstveni ključ za svaki collectible
        return $"collectible_L{levelNumber}_C{collectibleIndex}";
    }

    private void Start()
    {
        startPosition = transform.position;

        if (PlayerPrefs.GetInt(GetKey(), 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
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
            }else if (CompareTag("GameCollectible"))
            {
                PlayerPrefs.SetInt(GetKey(), 1);
                PlayerPrefs.Save();
                gameObject.SetActive(false);

                LevelCollectibleUI ui = FindFirstObjectByType<LevelCollectibleUI>();
                if (ui != null)
                {
                    ui.UpdateCollectibleUI();
                }
            }

            Destroy(gameObject); 
        }
    }
}
