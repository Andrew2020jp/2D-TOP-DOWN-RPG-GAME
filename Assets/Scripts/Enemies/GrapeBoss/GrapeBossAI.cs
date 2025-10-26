using System.Collections;
using UnityEngine;

public class GrapeBossAI : MonoBehaviour
{
    [Tooltip("The range within which the boss will start attacking the player.")]
    [SerializeField] private float attackRange = 15f;

    [Tooltip("The time in seconds between each attack volley.")]
    [SerializeField] private float attackCooldown = 3f;

    [Tooltip("Drag the component that has the boss's specific attack logic (e.g., the ShrinkingBoss script).")]
    [SerializeField] private MonoBehaviour enemyType;

    private bool canAttack = true;

    void Update()
    {
        // Check if the player exists and is within attack range
        if (PlayerController.Instance == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distanceToPlayer <= attackRange && canAttack)
        {
            // Set cooldown flag and trigger the attack
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

    // This is useful for debugging in the editor to see the attack range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
