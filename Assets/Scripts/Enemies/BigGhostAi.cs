using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigGhostAi : MonoBehaviour, IEnemy
{
    [Header("Boss 攻击配置")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 5f;

    [Header("召唤配置")]
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private Transform[] summonPoints;
    [SerializeField] private int summonCount = 2;

    // EnemyAI 会调用这个方法
    public void Attack()
    {
        // 50% 概率发射子弹，50% 概率召唤
        if (Random.value > 0.5f)
        {
            Shoot();
        }
        else
        {
            SummonMinions();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = (PlayerController.Instance.transform.position - firePoint.position).normalized;
            rb.velocity = dir * projectileSpeed;
        }
    }

    private void SummonMinions()
    {
        if (minionPrefab == null || summonPoints.Length == 0) return;

        for (int i = 0; i < summonCount; i++)
        {
            Transform point = summonPoints[Random.Range(0, summonPoints.Length)];
            Instantiate(minionPrefab, point.position, Quaternion.identity);
        }
    }
}
