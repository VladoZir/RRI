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

    public Sprite muteNormal;
    public Sprite muteHighlighted;
    public Sprite mutePressed;

    public Sprite unmuteNormal;
    public Sprite unmuteHighlighted;
    public Sprite unmutePressed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int unlockedLevels = PlayerPrefs.GetInt("unlockedLevels", 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i < unlockedLevels);
        }

        bool isMuted = (MusicManager.Instance != null) && MusicManager.Instance.IsMuted();
        UpdateMuteButtonSprites(isMuted);
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

        UpdateMuteButtonSprites(isMuted);
    }

    void UpdateMuteButtonSprites(bool isMuted)
    {
        var spriteState = new SpriteState();

        if (isMuted)
        {
            muteButton.image.sprite = muteNormal;
            spriteState.highlightedSprite = muteHighlighted;
            spriteState.pressedSprite = mutePressed;
            spriteState.selectedSprite = muteNormal; 
            spriteState.disabledSprite = muteNormal;
        }
        else
        {
            muteButton.image.sprite = unmuteNormal;
            spriteState.highlightedSprite = unmuteHighlighted;
            spriteState.pressedSprite = unmutePressed;
            spriteState.selectedSprite = unmuteNormal;
            spriteState.disabledSprite = unmuteNormal;
        }

        muteButton.spriteState = spriteState;
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

        int totalLevels = 3; 
        int collectiblesPerLevel = 2; 

        for (int level = 1; level <= totalLevels; level++)
        {
            for (int i = 0; i < collectiblesPerLevel; i++)
            {
                PlayerPrefs.DeleteKey($"collectible_L{level}_C{i}");
            }
        }

        Debug.Log("Progress reset!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}
