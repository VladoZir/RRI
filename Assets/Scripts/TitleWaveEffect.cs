using UnityEngine;

public class TitleWaveEffect : MonoBehaviour
{
    public float amplitude = 10f; // How high the letters move
    public float frequency = 2f; // Speed of the wave motion
    public float phaseOffset = 0f; // Used to stagger the letters

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency + phaseOffset) * amplitude;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}
