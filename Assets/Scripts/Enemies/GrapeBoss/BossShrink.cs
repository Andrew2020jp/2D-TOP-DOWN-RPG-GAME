using UnityEngine;

// Assumes you have your IEnemy interface defined somewhere
// public interface IEnemy { void Attack(); }

public class BossShrink : MonoBehaviour, IEnemy
{
    [Header("Boss Health Settings")]
    [Tooltip("The number of times the boss can attack (and shrink) before it dies.")]
    [SerializeField] private int shotsUntilDeath = 10;

    [Header("Attack Settings")]
    [Tooltip("The projectile prefab to be fired.")]
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("The total number of projectiles to fire in a single volley.")]
    [SerializeField] private int projectilesPerVolley = 5;

    [Tooltip("The radius around the player where projectiles will randomly land.")]
    [SerializeField] private float spreadRadiusAroundPlayer = 5f;

    private int shotsFired = 0;
    private Vector3 initialScale;
    private SpriteRenderer spriteRenderer;
    private Flash flash; // --- NEW: Reference to the Flash script ---

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flash = GetComponent<Flash>(); // --- NEW: Get the Flash component on startup ---
    }

    private void Start()
    {
        initialScale = transform.localScale;
    }

    public void Attack()
    {
        if (PlayerController.Instance == null) return;

        FacePlayer();
        FireProjectileVolley();
        ShrinkAndCheckDeath();
    }

    private void FacePlayer()
    {
        if (transform.position.x < PlayerController.Instance.transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    private void ShrinkAndCheckDeath()
    {
        shotsFired++;

        // --- NEW: Trigger the flash effect right when it shrinks ---
        if (flash != null)
        {
            StartCoroutine(flash.FlashRoutine());
        }

        if (shotsFired >= shotsUntilDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            float scaleMultiplier = 1.0f - ((float)shotsFired / shotsUntilDeath);
            transform.localScale = initialScale * scaleMultiplier;
        }
    }

    private void FireProjectileVolley()
    {
        if (PlayerController.Instance == null) return;

        Vector2 playerPosition = PlayerController.Instance.transform.position;

        for (int i = 0; i < projectilesPerVolley; i++)
        {
            Vector2 targetPosition;

            if (i == 0)
            {
                targetPosition = playerPosition;
            }
            else
            {
                Vector2 randomOffset = Random.insideUnitCircle * spreadRadiusAroundPlayer;
                targetPosition = playerPosition + randomOffset;
            }

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            GrapeBossProjectile projectileScript = projectile.GetComponent<GrapeBossProjectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(targetPosition);
            }
        }
    }
}