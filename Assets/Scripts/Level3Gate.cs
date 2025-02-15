using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTileActivator : MonoBehaviour
{
    public Tilemap tilemap; // Assign in the inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tilemap.gameObject.SetActive(true); // Show tiles
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tilemap.gameObject.SetActive(false); // Hide tiles
        }
    }
}
