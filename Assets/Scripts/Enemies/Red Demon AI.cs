using UnityEngine;

public class RedDemonAI : MonoBehaviour
{
    [Header("基本設定")]
    public float moveSpeed = 3f;
    public float chaseRange = 8f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 20;

    [Header("引用")]
    public Transform target;
    private Animator animator;
    private Rigidbody2D rb;

    private float lastAttackTime;
    private string currentState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // 自動找到玩家
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        // 攻擊範圍內
        if (distance <= attackRange)
        {
            rb.velocity = Vector2.zero;

            // 控制攻擊間隔
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Debug.Log("🔴 攻擊觸發!");
                ChangeAnimationState("Attack");
                lastAttackTime = Time.time;
            }
            else
            {
                ChangeAnimationState("Idle");
            }
        }
        // 追擊範圍內
        else if (distance <= chaseRange)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            // 翻轉朝向
            if (direction.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            ChangeAnimationState("Walk");
        }
        // 太遠 -> Idle
        else
        {
            rb.velocity = Vector2.zero;
            ChangeAnimationState("Idle");
        }
    }

    // ✅ 切換動畫狀態
    void ChangeAnimationState(string newState)
    {
        if (animator == null) return;

        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("Attack");

        animator.SetTrigger(newState);
    }

    // ✅ 攻擊事件 (從動畫事件觸發)
    public void DealDamage()
    {
        if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // 傳入 Boss 的 transform 當作 hitTransform
                playerHealth.TakeDamage(attackDamage, transform);
                Debug.Log("🗡️ Red Demon 對玩家造成 " + attackDamage + " 傷害!");
            }
        }
    }
}
