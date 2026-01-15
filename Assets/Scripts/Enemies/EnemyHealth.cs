using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 15f;

    private int currentHealth;
    private Knockback knockback;
    private Flash flash;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        knockback.GetKnockedBack(PlayerController.Instance.transform, knockBackThrust);
        StartCoroutine(flash.FlashRoutine());

        StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    public void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            BossBuffs bossBuffs = GetComponent<BossBuffs>();
            if (bossBuffs != null)
            {
                Debug.Log("SUCCESS: BossBuffs found! Triggering GrantBossBuffs.");
                bossBuffs.GrantBossBuffs();
            }
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            GetComponent<PickUpSpawner>().DropItems();
            EnemyAudioController audioCtrl = GetComponent<EnemyAudioController>();

            if (audioCtrl != null)
            {
                audioCtrl.PlayDeathSoundAndDestroy();
            }
            else
            {
                // Fallback if you forgot to add the script
                Destroy(gameObject);
            }
        }
    }
}
