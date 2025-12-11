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

    [Header("Phase 2 Settings (HP < 50%)")]
    public bool phaseTwo = false;
    public float phase2MoveSpeed = 3.5f;
    public float phase2SpitCooldown = 1.2f;
    public float phase2LeapCooldown = 3.5f;

    private void Start()
    {
        currentHP = maxHP;
        currentState = BossState.Idle;
    }

    private void Update()
    {
        if (player == null) return;

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
                if (dist <= attackRange && spitTimer <= 0)
                    currentState = BossState.Attack;
                else if (dist <= leapRange && leapTimer <= 0)
                    currentState = BossState.Leap;
                break;

            case BossState.Attack:
                StartCoroutine(SpitAttack());
                break;

            case BossState.Leap:
                StartCoroutine(LeapAttack());
                break;
        }
    }

    // ----------------------- BEHAVIOUR FUNCTIONS --------------------------

    void ChasePlayer()
    {
        anim.SetBool("Walking", true);
        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }

    IEnumerator SpitAttack()
    {
        currentState = BossState.Idle;
        anim.SetTrigger("Spit");

        yield return new WaitForSeconds(0.3f);

        Instantiate(poisonProjectilePrefab, shootPoint.position, shootPoint.rotation);

        spitTimer = spitCooldown;

        yield return new WaitForSeconds(0.2f);
        currentState = BossState.Chase;
    }

    IEnumerator LeapAttack()
    {
        currentState = BossState.Idle;
        anim.SetTrigger("Leap");

        yield return new WaitForSeconds(0.4f);

        Vector2 leapDirection = (player.position - transform.position).normalized;
        rb.AddForce(leapDirection * 700f);

        leapTimer = leapCooldown;

        yield return new WaitForSeconds(0.5f);
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
        rb.simulated = false;
        this.enabled = false;
    }
}
