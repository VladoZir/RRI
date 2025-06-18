using UnityEngine;
using TMPro;

public class LevelCollectibleUI : MonoBehaviour
{
    public int levelNumber; 
    public int totalCollectibles = 2; 
    public TMP_Text collectibleText; 

    void Start()
    {
        UpdateCollectibleUI();
    }

    public void UpdateCollectibleUI()
    {
        int collected = 0;
        for (int i = 0; i < totalCollectibles; i++)
        {
            if (PlayerPrefs.GetInt($"collectible_L{levelNumber}_C{i}", 0) == 1)
                collected++;
        }
        collectibleText.text = $"{collected} / {totalCollectibles}";
    }
}
