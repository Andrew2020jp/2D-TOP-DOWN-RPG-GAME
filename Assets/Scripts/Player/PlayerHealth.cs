using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool isDead { get; private set; }
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;
    [SerializeField] private float damageReductionMultiplier = 1f; // 1.0 = normal, 0.8 = 20% less damage

    private Slider healthSlider;
    private int currentHealth;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;

    public CinemachineVirtualCamera vcam;
    public GameObject player;

    const string HEALTH_SLIDER_TEXT = "Health Slider";
    public string TOWN_TEXT = "Scene1";
    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();

        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        isDead = false;
        currentHealth = maxHealth;

        UpdateHealthSlider();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();

        if (enemy)
        {
            TakeDamage(1, other.transform);
        }else if(other.gameObject.TryGetComponent<SlimeKingAI>(out var bossEnemy))
        {
            TakeDamage(bossEnemy.slimeKingContactDamage, other.transform);
        }
    }

    public void HealPlayer()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += 1;
            UpdateHealthSlider();
        }
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage) { return; }

        // Calculate reduced damage (Rounding to nearest int since your health is int)
        int finalDamage = Mathf.RoundToInt(damageAmount * damageReductionMultiplier);
        if (finalDamage < 1 && damageAmount > 0) finalDamage = 1; // Ensure they take at least 1 damage

        ScreenShakeManager.Instance.ShakeScreen();
        knockback.GetKnockedBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;

        currentHealth -= finalDamage; // Use finalDamage here

        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckIfPlayerDeath();
    }

    private void CheckIfPlayerDeath()
    {
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Destroy(ActiveWeapon.Instance.gameObject);
            currentHealth = 0;
            Debug.Log("Player Death");
            GetComponent<Animator>().SetTrigger(DEATH_HASH);
            StartCoroutine(DeathLoadSceneRoutine());
        }
    }

    private IEnumerator DeathLoadSceneRoutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        SceneManager.LoadScene(TOWN_TEXT);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        GameObject sp = GameObject.Find(TOWN_TEXT);
        if (sp != null && player != null)
            player.transform.position = sp.transform.position;

        if (vcam == null)
            vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null && player != null)
            vcam.Follow = player.transform;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider == null)
        {
            healthSlider = GameObject.Find(HEALTH_SLIDER_TEXT).GetComponent<Slider>();
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void BuffMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Heal the player for the amount gained
        UpdateHealthSlider();
    }

    public void BuffDefense(float reductionPercent)
    {
        // Example: reductionPercent = 0.1f (10% reduction)
        damageReductionMultiplier -= reductionPercent;

        // Clamp it so player doesn't become invincible (min 10% damage taken)
        if (damageReductionMultiplier < 0.1f) damageReductionMultiplier = 0.1f;
    }
}
