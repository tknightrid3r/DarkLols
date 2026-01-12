using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 1;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;

    [Header("Animation")]
    public Animator animator;

    [Header("Audio")]
    public AudioClip hitEnemySound;
    private AudioSource audioSource;

    private float lastAttackTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>(); // common for FPS arms
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // 🔥 Play attack animation
        animator.SetTrigger("Attack");

        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, 0.5f, direction, attackRange);

        foreach (RaycastHit hit in hits)
        {
            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (hitEnemySound != null)
                    audioSource.PlayOneShot(hitEnemySound);

                break;
            }
        }
    }
}