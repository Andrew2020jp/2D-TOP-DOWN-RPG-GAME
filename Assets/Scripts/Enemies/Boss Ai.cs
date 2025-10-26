using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;            // Reference to the player's transform
    public float teleportHealthThreshold = 10f; // Health threshold for teleportation
    public float teleportCooldown = 5f; // Cooldown time between each teleport can be activated
    public float detectionRange = 5f;   // Distance at which the Slime King detects the player 
    public float moveSpeed = 2f;        // Speed of movement towards the player
    public float attackRange = 1f;      // Range to trigger attack
    public float attackCooldown = 2f;   // Time between attacks
    public float slimeKingHealth = 80f;    // Slime King's Health

    public float explosionRadius = 5f;
    public int explosionDamage = 20;
    public float teleportAnimationDuration = 0.5f;   // Time it takes for slime king to teleport to the player(animation duration)
    private float lastTeleportTime = 0f;
    private float nextAttackTime = 0f;  // Timer for the attack cooldown

    private bool isTeleporting = false; // flag to check if slimeking is teleporting

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!isTeleporting)
        {
            // check if teleportation skill is available
            HandleTeleportation();

            // if not teleporting, continue detecting the player and move towards them
            DetectPlayerAndMove();
        }
    }

    void HandleTeleportation()
    {
        // Check if health fall below the threshold for teleportation skill to be activated(and if time has passed enough)
        if (slimeKingHealth <= teleportHealthThreshold && Time.time >= lastTeleportTime + teleportCooldown)
        {
            StartCoroutine(TeleportAnimation());
        }
    }

    IEnumerator TeleportAnimation()
    {
        isTeleporting = true;

        // Shrink
        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsedTime < teleportAnimationDuration / 2)
        {
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / (teleportAnimationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure fully shrunk
        transform.localScale = Vector3.zero;

        // Teleport to player's position
        transform.position = player.position;

        // Growing
        elapsedTime = 0f;
        while (elapsedTime < teleportAnimationDuration / 2)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsedTime / (teleportAnimationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        //ExplosiveAttack();
        Debug.Log("Slime King Teleported to the player!");

        lastTeleportTime = Time.time;

        isTeleporting = false;
    }

    //void ExplosiveAttack()
    //{
    //    Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
    //}

    void DetectPlayerAndMove()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > attackRange)
            {
                MoveTowardsPlayer();
            }
            else if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        Debug.Log("Slime King attacks!");

        nextAttackTime = Time.time + attackCooldown;
    }
}

