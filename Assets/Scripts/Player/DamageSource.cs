using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    /*
    private int damageAmount;

    private void Start()
    {
        MonoBehaviour currenActiveWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
        damageAmount = (currenActiveWeapon as IWeapon).GetWeaponInfo().weaponDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        enemyHealth?.TakeDamage(damageAmount);
    }
    */

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            // 1. Get the current active weapon RIGHT NOW
            MonoBehaviour currentActiveWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;

            // 2. Get its damage info RIGHT NOW
            int currentDamage = (currentActiveWeapon as IWeapon).GetWeaponInfo().weaponDamage;

            // 3. Apply that damage
            enemyHealth.TakeDamage(currentDamage);
        }
    }
}
