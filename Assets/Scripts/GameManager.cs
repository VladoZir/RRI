using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerPrefab; 
    //public Vector3 spawnPosition = new Vector3(5f, 0f, 0f); 

    public Camera mainCamera;

    private GameObject currentPlayer;

    public AudioSource collectAudio;

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

        // Assign the new player to the camera
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
        yield return new WaitForSeconds(delay); // Sačekaj 5 sekundi

        if (portalAnim != null)
        {
            portalAnim.Play("Portal_Disappear"); // Pokreni animaciju nestajanja

            // Sačekaj da se animacija završi
            yield return new WaitForSeconds(portalAnim.GetCurrentAnimatorStateInfo(0).length);

            // Deaktiviraj portal nakon animacije
            playerSpawnPosition.SetActive(false);
        }
    }

    public void UpgradePlayer(GameObject newPrefab, GameObject oldPlayer)
    {
        Vector3 position = oldPlayer.transform.position; 
        Destroy(oldPlayer); 
        currentPlayer = Instantiate(newPrefab, position, Quaternion.identity); 
        collectAudio.Play();

        // Assign the new player to the camera
        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.player = currentPlayer;
            }
        }

        // Link the bow to the PlayerController (if the new player prefab has the bow)
        PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Transform bowTransform = currentPlayer.transform.Find("Bow"); // Assuming Bow is a child of the player prefab
            if (bowTransform != null)
            {
                Debug.Log("bow found");
                playerController.bow = bowTransform; // Link the bow to the PlayerController
            }
        }
    }


}
