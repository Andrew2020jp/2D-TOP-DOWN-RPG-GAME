using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get { return facingLeft; } }


    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;

    [Header("Skills")]
    [SerializeField] private float healCooldown = 1f;
    [SerializeField] private int spinAttackDamage = 2;
    [SerializeField] private float spinAttackRadius = 2.5f;
    [SerializeField] private float spinAttackCooldown = 4f;
    [SerializeField] private float shieldDuration = 3f;
    [SerializeField] private float shieldCooldown = 10f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private Knockback knockback;
    private float startingMoveSpeed;

    private bool facingLeft = false;
    private bool isDashing = false;
    private float healCooldownTimer = 0f;
    private float spinAttackCooldownTimer = 0f;
    private float shieldCooldownTimer = 0f;

    // 0 = ready, 1 = just used; the skill UI reads these to draw cooldown overlays
    public float HealCooldownProgress { get { return healCooldown > 0 ? Mathf.Clamp01(healCooldownTimer / healCooldown) : 0f; } }
    public float SpinAttackCooldownProgress { get { return spinAttackCooldown > 0 ? Mathf.Clamp01(spinAttackCooldownTimer / spinAttackCooldown) : 0f; } }
    public float ShieldCooldownProgress { get { return shieldCooldown > 0 ? Mathf.Clamp01(shieldCooldownTimer / shieldCooldown) : 0f; } }

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();
        playerControls.Combat.Heal.performed += _ => Heal();
        playerControls.Combat.SpinAttack.performed += _ => SpinAttack();
        playerControls.Combat.Shield.performed += _ => Shield();

        startingMoveSpeed = moveSpeed;

        ActiveInventory.Instance.EquipStartingWeapon();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
        TickSkillCooldowns();
    }

    private void TickSkillCooldowns()
    {
        if (healCooldownTimer > 0f) { healCooldownTimer -= Time.deltaTime; }
        if (spinAttackCooldownTimer > 0f) { spinAttackCooldownTimer -= Time.deltaTime; }
        if (shieldCooldownTimer > 0f) { shieldCooldownTimer -= Time.deltaTime; }
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {   if (knockback.GettingKnockedBack || PlayerHealth.Instance.isDead) { return; }

        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector2 aimDirection = AimInput.GetAimDirection(transform.position);

        if (aimDirection.x < 0f)
        {
            mySpriteRender.flipX = true;
            facingLeft = true;
        }
        else
        {
            mySpriteRender.flipX = false;
            facingLeft = false;
        }
    }

    private void Dash()
    {
        if (!isDashing && Stamina.Instance.CurrentStamina > 0)
        {
            Stamina.Instance.UseStamina();
            isDashing = true;
            moveSpeed *= dashSpeed;
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        float dashCD = .25f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }

    private void Heal()
    {
        if (healCooldownTimer > 0f || PlayerHealth.Instance.isDead) { return; }
        if (PlayerHealth.Instance.IsAtFullHealth || Stamina.Instance.CurrentStamina <= 0) { return; }

        Stamina.Instance.UseStamina();
        PlayerHealth.Instance.HealPlayer();
        healCooldownTimer = healCooldown;
    }

    private void SpinAttack()
    {
        if (spinAttackCooldownTimer > 0f || PlayerHealth.Instance.isDead) { return; }
        if (Stamina.Instance.CurrentStamina <= 0) { return; }

        Stamina.Instance.UseStamina();

        // damage benefits from the same stat buffs as normal weapons
        int damage = spinAttackDamage;
        if (PlayerStatManager.Instance != null)
        {
            damage = Mathf.RoundToInt(PlayerStatManager.Instance.GetAdjustedDamage(spinAttackDamage));
        }

        // God of War style storm: glowing nova + arc cage + sky bolts,
        // hitting everything inside 3 times while it lasts
        LightningStorm.Create(transform, spinAttackRadius, damage);
        PlayThunderSound();
        ScreenShakeManager.Instance.ShakeScreen();
        spinAttackCooldownTimer = spinAttackCooldown;
    }

    private void PlayThunderSound()
    {
        AudioClip thunder = Resources.Load<AudioClip>("SkillSfx/thunder");
        if (thunder == null) { return; }

        // played as 2D audio so the volume doesn't drop off with camera distance
        GameObject audioObject = new GameObject("Thunder SFX");
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = thunder;
        source.volume = 0.7f;
        source.spatialBlend = 0f;
        source.Play();
        Destroy(audioObject, thunder.length);
    }

    private void Shield()
    {
        if (shieldCooldownTimer > 0f || PlayerHealth.Instance.isDead) { return; }

        PlayerHealth.Instance.ActivateShield(shieldDuration);
        shieldCooldownTimer = shieldCooldown;
    }
}
