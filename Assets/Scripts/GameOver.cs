using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject deathPanel; // Assign Game Over Panel in the Inspector

    public void ShowGameOverScreen()
    {
        deathPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume game speed
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload level
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Resume game speed
        SceneManager.LoadScene("MainMenu"); // Make sure your main menu scene is added to build settings
    }
}
