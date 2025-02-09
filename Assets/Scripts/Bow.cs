using System.Collections;
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

    private Animator animator;

    public AudioSource bowAudio;

    void Start()
    {
        animator = GetComponent<Animator>(); 
        bowAudio = GetComponent<AudioSource>();
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

        if (bowAudio != null)
        {
            bowAudio.Play();
        }
       
        if (animator != null)
        {
            animator.SetBool("IsShooting", true);
            StartCoroutine(WaitForShootAnimation());
        }

        // Instantiate and launch arrow
        GameObject newArrow = Instantiate(arrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().linearVelocity = transform.right * launchForce;
    }

    private IEnumerator WaitForShootAnimation()
    {
        // Wait for the shoot animation to finish (adjust time based on animation length)
        yield return new WaitForSeconds(0.15f); // Adjust this time based on your shoot animation length
        animator.SetBool("IsShooting", false); // Reset back to idle state after shooting
    }
}