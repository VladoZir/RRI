using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    public GameObject levelSelectionPanel;
   
    public int level;

    public Button[] levelButtons;

    private bool isToggled = false;

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

    public void ToggleSelection()
    {
        isToggled = !isToggled;

        if (isToggled)
        {
            levelSelectionPanel.SetActive(true);
        }
        else
        {
            levelSelectionPanel.SetActive(false);
        }
    }

    public void OpenScene(int levelNumber)
    {
        level = levelNumber;
        SceneManager.LoadScene("Level" + level.ToString());
    }
}
