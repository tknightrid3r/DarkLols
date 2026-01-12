using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 1;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    [Header("Audio")]
    public AudioClip attackSound;

    private float lastAttackTime;
    private AudioSource audioSource;

    private Transform player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // Damage the player
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);

            // Play attack sound
            if (attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            Debug.Log($"{name} attacked player for {damage} damage");
        }
    }
}
