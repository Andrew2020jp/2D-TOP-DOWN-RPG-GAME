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
        SlimeKingAI slimeKing = other.gameObject.GetComponent<SlimeKingAI>();

        if (enemyHealth != null || slimeKing != null)
        {
            MonoBehaviour currentActiveWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;

            if (currentActiveWeapon != null)
            {
                // 1. Get the base damage from the WeaponInfo ScriptableObject
                int baseDamage = (currentActiveWeapon as IWeapon).GetWeaponInfo().weaponDamage;

                // 2. Add the buff from the Stat Manager
                // We use Mathf.RoundToInt because Damage is an int, but buffs might be floats
                int finalDamage = Mathf.RoundToInt(PlayerStatManager.Instance.GetAdjustedDamage(baseDamage));

                // 3. Apply the buffed damage
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(finalDamage);
                }
                else
                {
                    slimeKing.TakeDamage(finalDamage);
                }
            }
        }
    }
}
