using UnityEngine;
using System.Collections;

public class SpiderBossAI : MonoBehaviour
{
    public enum BossState { Idle, Chase, Attack, Leap }
    private BossState currentState;

    [Header("References")]
    public Transform player;
    public Rigidbody2D rb;
    public Animator anim;
    public GameObject poisonProjectilePrefab;
    public Transform shootPoint;
    // Visual child to rotate independently of root/animator (assign the Sprite/Renderer GameObject)
    public Transform visual;

    [Header("Stats")]
    public float maxHP = 200;
    public float currentHP;
    public float moveSpeed = 2.5f;
    public float chaseRange = 10f;
    public float attackRange = 4f;
    public float leapRange = 6f;

    [Header("Cooldowns")]
    public float spitCooldown = 2f;
    private float spitTimer;
    public float leapCooldown = 5f;
    private float leapTimer;

    // Prevent multiple overlapping attack coroutines
    private bool isAttacking = false;

    [Header("Phase 2 Settings (HP < 50%)")]
    public bool phaseTwo = false;
    public float phase2MoveSpeed = 3.5f;
    public float phase2SpitCooldown = 1.2f;
    public float phase2LeapCooldown = 3.5f;

    [Header("Facing")]
    // When visual is assigned we rotate only the visual (prevents Animator/physics conflicts)
    public bool smoothFacing = false;
    public float facingSpeedDegPerSec = 720f;
    public float facingAngleOffset = 180f; // adjust so sprite's artwork forward direction matches player-facing

    // Optional pathfinding component (CrystalKnight style). If present, use it for movement.
    private EnemyPathfinding enemyPathfinding;

    private void Start()
    {
        currentHP = maxHP;
        currentState = BossState.Idle;
        enemyPathfinding = GetComponent<EnemyPathfinding>();
    }

    private void Update()
    {
        if (player == null) return;

        // Make the boss face the player every frame (rotate visual if set)
        FacePlayer();

        // Phase2 check
        if (!phaseTwo && currentHP <= maxHP * 0.5f)
            EnterPhaseTwo();

        spitTimer -= Time.deltaTime;
        leapTimer -= Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Idle:
                if (dist < chaseRange) currentState = BossState.Chase;
                break;

            case BossState.Chase:
                ChasePlayer();
                // Only leap if player is within leapRange but outside attackRange
                if (dist <= leapRange && dist > attackRange && leapTimer <= 0)
                {
                    currentState = BossState.Leap;
                }
                else if (dist <= attackRange && spitTimer <= 0)
                {
                    currentState = BossState.Attack;
                }
                break;

            case BossState.Attack:
                if (!isAttacking)
                    StartCoroutine(SpitAttack());
                break;

            case BossState.Leap:
                StartCoroutine(LeapAttack());
                break;
        }
    }

    // ----------------------- FACING --------------------------
    void FacePlayer()
    {
        if (player == null) return;
        Vector2 dir = (player.position - transform.position);
        if (dir.sqrMagnitude <= Mathf.Epsilon) return;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + facingAngleOffset;

        // If a visual child is assigned, rotate only that transform to avoid animator/physics conflicts.
        if (visual != null)
        {
            Quaternion targetRot = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            if (smoothFacing)
                visual.rotation = Quaternion.RotateTowards(visual.rotation, targetRot, facingSpeedDegPerSec * Time.deltaTime);
            else
                visual.rotation = targetRot;
            return;
        }

        // Fallback: rotate the root (but prefer visual approach)
        Quaternion fallbackRot = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        if (smoothFacing)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, fallbackRot, facingSpeedDegPerSec * Time.deltaTime);
        else
            transform.rotation = fallbackRot;
    }

    // ----------------------- BEHAVIOUR FUNCTIONS --------------------------
    void ChasePlayer()
    {
        anim.SetBool("Walking", true);
        Vector2 dir = (player.position - transform.position).normalized;

        // If there's an EnemyPathfinding component use it (same pattern as CrystalKnightAI).
        if (enemyPathfinding != null)
        {
            enemyPathfinding.MoveTo(dir);
            return;
        }

        // Otherwise use Rigidbody2D movement as before.
        if (rb != null)
            rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
        else
            transform.position = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;
    }

    IEnumerator SpitAttack()
    {
        isAttacking = true;
        currentState = BossState.Attack;
        anim.SetBool("Walking", false);
        anim.SetTrigger("Spit");

        yield return new WaitForSeconds(0.3f); // Wait for animation

        if (poisonProjectilePrefab != null && shootPoint != null)
        {
            // 1. Calculate direction to the player
            Vector2 direction = (player.position - shootPoint.position).normalized;

            // 2. Convert direction to a rotation (Angle) and apply offset so projectile faces correctly
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // 3. Spawn with the calculated rotation
            Instantiate(poisonProjectilePrefab, shootPoint.position, rotation);
        }

        spitTimer = spitCooldown;
        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
        currentState = BossState.Chase;
    }

    IEnumerator LeapAttack()
    {
        currentState = BossState.Idle;
        anim.SetTrigger("Leap");

        yield return new WaitForSeconds(0.4f);

        Vector2 leapDirection = (player.position - transform.position).normalized;
        if (rb != null)
            rb.AddForce(leapDirection * 700f);
        else
            transform.position = (Vector2)transform.position + leapDirection * 2f; // fallback impulse

        leapTimer = leapCooldown;

        yield return new WaitForSeconds(0.5f);

        // Stops the sliding movement
        if (rb != null)
            rb.velocity = Vector2.zero;

        currentState = BossState.Chase;
    }

    // ----------------------- DAMAGE & PHASE CHANGE ------------------------

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
            Die();
    }

    void EnterPhaseTwo()
    {
        phaseTwo = true;
        moveSpeed = phase2MoveSpeed;
        spitCooldown = phase2SpitCooldown;
        leapCooldown = phase2LeapCooldown;

        anim.SetTrigger("Phase2");
    }

    void Die()
    {
        anim.SetTrigger("Die");
        if (rb != null) rb.simulated = false;
        this.enabled = false;
    }
}