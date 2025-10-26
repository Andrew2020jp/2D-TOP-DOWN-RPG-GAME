using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeKingAI : MonoBehaviour
{
    public Transform player;            // Reference to the player's transform
    public int teleportHealthThreshold = 60; // Health threshold for teleportation
    public float teleportCooldown = 5f; // Cooldown time between each teleport can be activated
    public float detectionRange = 30f;   // Distance at which the Slime King detects the player 
    public float moveSpeed = 3f;        // Speed of movement towards the player
    public float attackRange = 1f;      // Range to trigger attack
    public float attackCooldown = 2.5f;   // Time between attacks
    public int slimeKingContactDamage = 2; // Slime King's damage

    public GameObject slimePrefab;
    public int numberOfSlimesToSpawn = 5;
    public float spawnRadius = 2f;
    public float spawnDelay = 1.5f;
    public float spawnHealthThreshold = 20f;

    private bool hasSpawnedSlimes = false;

    public float explosionRadius = 5f;
    // public int explosionDamage = 10;
    public float teleportAnimationDuration = 0.8f;   // Time it takes for slime king to teleport to the player(animation duration)
    private float lastTeleportTime = 0f;
    private float nextAttackTime = 0f;  // Timer for the attack cooldown

    private bool isTeleporting = false; // flag to check if slimeking is teleporting

    [SerializeField] private int startingHealth = 60;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 2f;

    public int currentHealth;
    private Knockback knockback;
    private Flash flash;

    private void Start()
    {
        currentHealth = startingHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Update()
    {
        if (!isTeleporting)
        {
            if(currentHealth <= spawnHealthThreshold && !hasSpawnedSlimes)
            {
                // Start spawning slimes around when health fall below threshold
                StartCoroutine(SpawnSlimesRoutine());
            }
            else
            {
                // check if teleportation skill is available
                HandleTeleportation();

                // if not teleporting, continue detecting the player and move towards them
                DetectPlayerAndMove();
            }
        }
    }

    void HandleTeleportation()
    {
        // Check if health fall below the threshold for teleportation skill to be activated(and if time has passed enough)
        if (currentHealth <= teleportHealthThreshold && Time.time >= lastTeleportTime + teleportCooldown)
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
        // ExplosiveAttack();
        Debug.Log("Slime King Teleported to the player!");

        lastTeleportTime = Time.time;

        isTeleporting = false;
    }

    IEnumerator SpawnSlimesRoutine()
    {
        hasSpawnedSlimes = true;
        isTeleporting = true;

        yield return new WaitForSeconds(spawnDelay);

        for(int i = 0; i < numberOfSlimesToSpawn; i++)
        {
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle.normalized * spawnRadius;
            Instantiate(slimePrefab, spawnPosition, Quaternion.identity);
        }

        isTeleporting = false;
    }

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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        knockback.GetKnockedBack(PlayerController.Instance.transform, knockBackThrust);
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    public void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            GetComponent<PickUpSpawner>().DropItems();
            Destroy(gameObject);
        }
    }
}

