using UnityEngine;
using TMPro;

public class TotalCollectibleUI : MonoBehaviour
{
    public int totalLevels = 3; 
    public int collectiblesPerLevel = 2; 
    public TMP_Text collectibleText;

    void Start()
    {
        UpdateTotalCollectibles();
    }

    public void UpdateTotalCollectibles()
    {
        int totalCollected = 0;
        int totalCollectibles = totalLevels * collectiblesPerLevel;

        for (int level = 1; level <= totalLevels; level++)
        {
            for (int i = 0; i < collectiblesPerLevel; i++)
            {
                if (PlayerPrefs.GetInt($"collectible_L{level}_C{i}", 0) == 1)
                    totalCollected++;
            }
        }
        collectibleText.text = $"{totalCollected} / {totalCollectibles}";
    }
}
