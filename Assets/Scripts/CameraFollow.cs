using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player; 
    public GameObject playerSpawn; 
    public Vector3 offset;    

    void Start()
    {
        if (playerSpawn != null)
        {
            player = playerSpawn; 
        }
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, transform.position.z);
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        player = newTarget;
    }
}
