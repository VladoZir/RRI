using UnityEngine;
using Pathfinding;
using System;

public class AITrigger : MonoBehaviour
{
    public static event Action<Transform> OnPlayerTriggered;
    public GameObject bossHealthContainer;

  

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Transform playerTransform = other.transform;

            AssignEnemiesTarget(playerTransform);
            bossHealthContainer.SetActive(true);

            // Notify all listeners that the player has triggered this AI
            OnPlayerTriggered?.Invoke(playerTransform);

            FinalBossAI finalBossAI = FindFirstObjectByType<FinalBossAI>();
            if (finalBossAI != null)
            {
                finalBossAI.OnHealthBarActivated();
            }
        }
    }

    private void AssignEnemiesTarget(Transform playerTransform)
    {
        AIDestinationSetter[] aiDestinations = FindObjectsByType<AIDestinationSetter>(FindObjectsSortMode.None);
        foreach (AIDestinationSetter ai in aiDestinations)
        {
            ai.target = playerTransform;
        }
    }
}
