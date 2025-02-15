using UnityEngine;

public class SpaceGun : MonoBehaviour
{
    public GameObject projectile;  // The space bullet or laser
    public float launchSpeed = 20f;
    public Transform shotPoint;

    public float shootCooldown = 0.3f; // Adjust fire rate
    private float lastShootTime = 0f;

    private Animator animator;
    public AudioSource gunAudio;

    void Start()
    {
        animator = GetComponent<Animator>();
        gunAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        AimAtMouse();

        if (Time.time - lastShootTime >= shootCooldown)
        {
            if (Input.GetMouseButtonDown(0)) // Left click to shoot
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
    }

    void AimAtMouse()
    {
        Vector2 gunPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - gunPosition).normalized;

        // Rotate the gun to face the mouse
        if (transform != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        // Flip gun to face the correct direction
        if (direction.x < 0)
        {
            FlipGun(true);  // Flip gun to the left
        }
        else
        {
            FlipGun(false); // Flip gun to the right
        }
    }

    void FlipGun(bool flipLeft)
    {
        if (flipLeft)
        {
            transform.localScale = new Vector3(1f, -1f, 1f);  // Flip the gun to face left
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);   // Reset the gun to face right
        }
    }



    void Shoot()
    {
        if (gunAudio != null)
        {
            gunAudio.Play();
        }

        if (animator != null)
        {
            animator.SetTrigger("Shoot"); // Using a trigger instead of a bool
        }

        // Create projectile and set its velocity
        GameObject newProjectile = Instantiate(projectile, shotPoint.position, shotPoint.rotation);
        newProjectile.GetComponent<Rigidbody2D>().linearVelocity = shotPoint.right * launchSpeed;
    }
}
