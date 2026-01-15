using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoodedKnightWeapon : MonoBehaviour
{
    public int attackDamage = 20;
   

    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    public void Attack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);

        if (colInfo == null) return;

        PlayerHealth PlayerHealth = colInfo.GetComponent<PlayerHealth>();
        if (PlayerHealth == null)
        {
            Debug.LogWarning("Hit object has no PlayerHealth!", colInfo.gameObject);
            return;
        }

        PlayerHealth.TakeDamage(attackDamage, transform);
    }

}


