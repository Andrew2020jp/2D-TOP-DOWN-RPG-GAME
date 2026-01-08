using UnityEngine;

public class PlayerStatManager : Singleton<PlayerStatManager>
{
    [Header("Buff Multipliers")]
    public float attackBuffFlat = 0f;      // Adds to base damage
    public float cooldownReduction = 0f;   // Subtracts from cooldown time

    public void BuffAttack(float amount)
    {
        attackBuffFlat += amount;
    }

    public void BuffCooldown(float amount)
    {
        cooldownReduction += amount;
    }

    // Helper methods to get the "Real" stats
    public float GetAdjustedDamage(float baseDamage) => baseDamage + attackBuffFlat;

    public float GetAdjustedCooldown(float baseCooldown)
    {
        float val = baseCooldown - cooldownReduction;
        return Mathf.Max(val, 0.1f); // Don't let cooldown reach 0
    }
}