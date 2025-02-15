using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTileActivator : MonoBehaviour
{
    public Tilemap tilemap; // Assign in the inspector
    private bool activated = false; // Prevents re-triggering

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            tilemap.gameObject.SetActive(true);
            activated = true; // Prevents further changes
        }
    }
}