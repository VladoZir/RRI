using UnityEngine;
using Pathfinding;

public class AITrigger : MonoBehaviour
{
    private Transform currentPlayer;
    public GameObject bossHealthContainer;

    public void SetPlayer(Transform playerTransform)
    {
        currentPlayer = playerTransform;
        //Debug.Log("AITrigger: Player assigned successfully.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AssignEnemiesTarget();
            bossHealthContainer.SetActive(true);

            FinalBossAI finalBossAI = Object.FindFirstObjectByType<FinalBossAI>();
            if (finalBossAI != null)
            {
                finalBossAI.OnHealthBarActivated(); 
                //Debug.Log("Final Boss AI health bar activated.");
            }
            else
            {
                //Debug.LogError("FinalBossAI not found in the scene!");
            }
        }
    }

    private void AssignEnemiesTarget()
    {
        if (currentPlayer == null)
        {
            //Debug.LogWarning("Player not found! Make sure the player is spawned before triggering AI.");
            return;
        }

        AIDestinationSetter[] aiDestinations = FindObjectsByType<AIDestinationSetter>(FindObjectsSortMode.None);
        foreach (AIDestinationSetter ai in aiDestinations)
        {
            ai.target = currentPlayer;
        }

        //Debug.Log("Enemies now targeting the player.");
    }
}
