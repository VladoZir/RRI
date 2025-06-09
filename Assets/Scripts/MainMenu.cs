using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject levelSelectionPanel;
    public GameObject settingsPanel;

    public int level;

    public Button[] levelButtons;
    public Button muteButton;

    private bool isToggledSelection = false;
    private bool isToggledSettings = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int unlockedLevels = PlayerPrefs.GetInt("unlockedLevels", 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i < unlockedLevels);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleMusic()
    {
        if (MusicManager.Instance == null)
            return;

        bool isMuted = !MusicManager.Instance.IsMuted();
        MusicManager.Instance.SetMute(isMuted);
    }


    public void PlayGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    public void ToggleSelection()
    {
        isToggledSelection = !isToggledSelection;

        if (isToggledSelection)
        {
            levelSelectionPanel.SetActive(true);
        }
        else
        {
            levelSelectionPanel.SetActive(false);
        }
    }

    public void ToggleSettings()
    {
        isToggledSettings = !isToggledSettings;

        if (isToggledSettings)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OpenScene(int levelNumber)
    {
        level = levelNumber;
        SceneManager.LoadScene("Level" + level.ToString());
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("unlockedLevels");
        // Ako imaš collectables, npr. PlayerPrefs.DeleteKey("coins");
        // Dodaj još što trebaš resetirati

        Debug.Log("Progress reset!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}
