using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrow;
    public float launchForce;
    public Transform shotPoint;

    public GameObject point;
    GameObject[] points;
    public int numberOfPoints;
    public float spaceBetweenPoints;
    Vector2 direction;

    public float shootCooldown = 0.5f;
    private float lastShootTime = 0f;

    private Animator animator; // Reference to Animator

    void Start()
    {
        animator = GetComponent<Animator>(); // Get Animator component
    }

    void Update()
    {
        Vector2 bowPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - bowPosition;
        transform.right = direction;

        if (Time.time - lastShootTime >= shootCooldown)
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse click
            {
                Shoot();
                lastShootTime = Time.time; // Update the last shoot time
            }
        }
    }

    void Shoot()
    {
        // Play shooting animation
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        // Instantiate and launch arrow
        GameObject newArrow = Instantiate(arrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().linearVelocity = transform.right * launchForce;
    }
}