using UnityEngine;

public class BossBuffs : MonoBehaviour
{
    [Header("Buff Values (0 = No Buff)")]
    public int hpIncrease = 0;
    public float damageReductionPercent = 0f;
    public float attackPowerBoost = 0f;
    public float cooldownReduction = 0f;
    public int staminaIncrease = 0;

    public void GrantBossBuffs()
    {
        Debug.Log($"[BossBuffs] Attempting to grant buffs from: {gameObject.name}");

        if (PlayerStatManager.Instance == null)
        {
            Debug.LogError("CRITICAL: PlayerStatManager instance is missing from the scene!");
        }
        if (PlayerHealth.Instance == null)
        {
            Debug.LogError("CRITICAL: PlayerHealth instance is missing!");
        }

        if (hpIncrease > 0)
            PlayerHealth.Instance.BuffMaxHealth(hpIncrease);

        if (damageReductionPercent > 0)
            PlayerHealth.Instance.BuffDefense(damageReductionPercent);

        if (staminaIncrease > 0 && Stamina.Instance != null)
            Stamina.Instance.IncreaseMaxStamina(staminaIncrease);

        if (attackPowerBoost > 0 && PlayerStatManager.Instance != null)
            PlayerStatManager.Instance.BuffAttack(attackPowerBoost);

        if (cooldownReduction > 0 && PlayerStatManager.Instance != null)
            PlayerStatManager.Instance.BuffCooldown(cooldownReduction);
    }
}