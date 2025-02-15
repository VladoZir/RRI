using UnityEngine;

public class SpaceGun : MonoBehaviour
{
    public GameObject projectile;
    public float launchSpeed = 20f;
    public Transform shotPoint;

    public float shootCooldown = 0.3f;
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
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
    }

    public void AimAtMouse()
    {
        Vector2 gunPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - gunPosition).normalized;

        // Calculate angle to rotate towards mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation
        transform.rotation = Quaternion.Euler(0, 0, angle);

        bool facingLeft = transform.parent.localScale.x < 0;

        // Check if gun is pointing upward
        bool isPointingUp = Mathf.Abs(angle) > 90f;

        // Combine both conditions to determine Y scale
        float yScale = isPointingUp ? -1f : 1f;
        float xScale = facingLeft ? -1f : 1f;

        transform.localScale = new Vector3(xScale, yScale, 1f);
    }

    void Shoot()
    {
        if (gunAudio != null)
        {
            gunAudio.Play();
        }

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        GameObject newProjectile = Instantiate(projectile, shotPoint.position, shotPoint.rotation);

        // Calculate the shooting direction based on the mouse position
        Vector2 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        newProjectile.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * launchSpeed;
    }
}