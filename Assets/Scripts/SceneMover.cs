using UnityEngine;

public class SceneMover : MonoBehaviour
{
    public float scrollSpeed = 2f; 
    public GameObject player; 
    private Vector3 startPosition; 
    public bool isPaused = false; 

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (!isPaused)
        {
            Vector3 scenePosition = transform.position;

            scenePosition.x -= scrollSpeed * Time.deltaTime;

            transform.position = scenePosition;
        }
    }

    public void PauseSceneMovement(bool pause)
    {
        isPaused = pause;
    }
}
