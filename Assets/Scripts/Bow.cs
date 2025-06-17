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
            if (Input.GetMouseButtonDown(0)) 
            {
                Shoot();
                lastShootTime = Time.time; 
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

        GameObject newArrow = Instantiate(arrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().linearVelocity = transform.right * launchForce;
    }

    private IEnumerator WaitForShootAnimation()
    {
        yield return new WaitForSeconds(0.15f); 
        animator.SetBool("IsShooting", false);
    }
}