using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }

    private PlayerControls playerControls;
    private float timeBetweenAttacks;

    private bool attackButtonDown, isAttacking = false;

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();

        AttackCooldown();
    }

    private void Update()
    {
        Attack();
    }

    public void NewWeapon(MonoBehaviour newWeapon)
    {
        CurrentActiveWeapon = newWeapon;

        // Get the base cooldown from the weapon
        float baseCooldown = (CurrentActiveWeapon as IWeapon).GetWeaponInfo().weaponCooldown;

        // Apply the buffed cooldown via the Stat Manager
        timeBetweenAttacks = PlayerStatManager.Instance.GetAdjustedCooldown(baseCooldown);

        AttackCooldown();
    }

    public void WeaponNull()
    {
        CurrentActiveWeapon = null;
    }

    private void AttackCooldown()
    {
        isAttacking = true;
        StopAllCoroutines();
        StartCoroutine(TimeBetweenAttacksRoutine());
    }

    private IEnumerator TimeBetweenAttacksRoutine()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Attack()
    {
        if (attackButtonDown && !isAttacking && CurrentActiveWeapon)
        {
            // Re-calculate cooldown here in case a buff was gained mid-session
            float baseCooldown = (CurrentActiveWeapon as IWeapon).GetWeaponInfo().weaponCooldown;
            timeBetweenAttacks = PlayerStatManager.Instance.GetAdjustedCooldown(baseCooldown);

            AttackCooldown();
            (CurrentActiveWeapon as IWeapon).Attack();
        }
    }
}
