using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicSetter : MonoBehaviour
{
    void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusicForScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}
