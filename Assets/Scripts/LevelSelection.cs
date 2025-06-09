using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public GameObject levelSelectionPanel;
   
    public int level;

    private bool isToggled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
