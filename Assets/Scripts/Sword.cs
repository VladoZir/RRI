using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class Sword : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int swordDamage = 10;

    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    public AudioSource audioSource;
    public AudioClip hitAudio;
    public AudioClip swooshAudio;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            //audioSource.PlayOneShot(swooshAudio);
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                //Debug.Log("Player hit" + enemy.name);
                enemy.GetComponent<IEnemy>().TakeDamage(swordDamage);
                audioSource.PlayOneShot(hitAudio);
            }
            else
            {
                audioSource.PlayOneShot(swooshAudio);
            }
        }
        if(hitEnemies.Length == 0) { audioSource.PlayOneShot(swooshAudio); }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null) 
        { 
            return; 
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);   
    }
}
