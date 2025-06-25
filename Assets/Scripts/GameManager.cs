using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerPrefab;
    public Camera mainCamera;

    public GameObject currentPlayer;

    public AudioSource collectBowAudio;
    public AudioSource collectSwordAudio;
    public AudioSource collectSpaceGunAudio;

    public GameObject playerSpawnPosition;
    private Animator portalAnim;

    private void Awake()
    {
        Instance = this;

        if (playerSpawnPosition != null)
        {
            portalAnim = playerSpawnPosition.GetComponentInChildren<Animator>();
        }
    }

    void Start()
    {
        Invoke("SpawnPlayerWithDelay", 1f);
    }

    void SpawnPlayerWithDelay()
    {
        SpawnPlayer(playerPrefab);
    }

    public void SpawnPlayer(GameObject prefab)
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(prefab, playerSpawnPosition.transform.position, Quaternion.identity);

        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(currentPlayer);
            }
        }

        if (portalAnim != null)
        {
            StartCoroutine(HidePortalAfterDelay(5f));
        }

    }

    private IEnumerator HidePortalAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (portalAnim != null)
        {
            portalAnim.Play("Portal_Disappear");
            yield return new WaitForSeconds(portalAnim.GetCurrentAnimatorStateInfo(0).length);
            playerSpawnPosition.SetActive(false);
        }
    }

    public void UpgradePlayerBow(GameObject newPrefab, GameObject oldPlayer)
    {
        ReplacePlayer(newPrefab, oldPlayer);
        if (collectBowAudio != null) collectBowAudio.Play();
    }

    public void UpgradePlayerSword(GameObject newPrefab, GameObject oldPlayer)
    {
        ReplacePlayer(newPrefab, oldPlayer);
        if (collectSwordAudio != null) collectSwordAudio.Play();
    }

    public void UpgradePlayerSpaceGun(GameObject newPrefab, GameObject oldPlayer) 
    {
        ReplacePlayer(newPrefab, oldPlayer);
        if (collectSpaceGunAudio != null) collectSpaceGunAudio.Play();
    }

    private void ReplacePlayer(GameObject newPrefab, GameObject oldPlayer)
    {
        Vector3 position = oldPlayer.transform.position;
        Destroy(oldPlayer);
        currentPlayer = Instantiate(newPrefab, position, Quaternion.identity);

        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.player = currentPlayer;
            }
        }
    }
}
