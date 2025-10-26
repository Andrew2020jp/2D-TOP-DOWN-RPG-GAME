using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalKnightAI : MonoBehaviour
{
    [Header("Behavior Timings")]
    [SerializeField] private float chaseTime = 5f; // How long to chase the player
    [SerializeField] private float restTime = 2f;  // How long to rest after chasing

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private bool stopMovingWhileAttacking = true; // Set this in the Inspector
    [SerializeField] private MonoBehaviour enemyType; // The script that handles the attack (e.g., Shooter)
    [SerializeField] private float attackCooldown = 2f;

    private bool canAttack = true;

    // The different states the Crystal Knight can be in
    private enum State
    {
        Chasing,
        Resting,
        Attacking
    }

    private State state;
    private EnemyPathfinding enemyPathfinding;
    private float timer; // A general-purpose timer for states

    private void Awake()
    {
        // Get the pathfinding component and set the initial state
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Chasing;
        timer = chaseTime; // Start with a full chase timer
    }

    private void Update()
    {
        // This is the "brain" that controls the knight's actions each frame
        MovementStateControl();
    }

    private void MovementStateControl()
    {
        // Decrease the timer for the current state unless we are attacking
        if (state != State.Attacking)
        {
            timer -= Time.deltaTime;
        }

        switch (state)
        {
            default:
            case State.Chasing:
                Chasing();
                break;

            case State.Resting:
                Resting();
                break;

            case State.Attacking:
                Attacking();
                break;
        }
    }

    private void Chasing()
    {
        // Calculate the direction to the player
        Vector2 directionToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;

        // Tell the pathfinding script to move towards the player
        enemyPathfinding.MoveTo(directionToPlayer);

        // If the player gets close enough, switch to the Attacking state
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange)
        {
            state = State.Attacking;
            if (stopMovingWhileAttacking)
            {
                enemyPathfinding.StopMoving();
            }
            return;
        }

        // When the chase timer runs out, switch to Resting
        if (timer <= 0)
        {
            state = State.Resting;
            timer = restTime; // Reset the timer for the rest duration
            enemyPathfinding.StopMoving();
        }
    }

    private void Resting()
    {
        // The knight does nothing while resting (already stopped moving)

        // When the rest timer runs out, switch back to Chasing
        if (timer <= 0)
        {
            state = State.Chasing;
            timer = chaseTime; // Reset the timer for the chase duration
        }
    }

    private void Attacking()
    {
        // If the player moves out of attack range, go back to chasing them
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > attackRange)
        {
            state = State.Chasing;
            timer = chaseTime; // Start a new chase cycle
            return;
        }

        // If we are allowed to move while attacking, continue to chase the player
        if (!stopMovingWhileAttacking)
        {
            Vector2 directionToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;
            enemyPathfinding.MoveTo(directionToPlayer);
        }

        // If the knight is ready to attack, perform the attack
        if (canAttack)
        {
            canAttack = false;
            (enemyType as IEnemy).Attack();
            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
