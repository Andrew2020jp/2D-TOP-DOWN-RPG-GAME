using UnityEngine;

// This component tells the FlagHolder when this object is destroyed.
// It now supports both the *permanent flag* and the *respawn cooldown* system.
public class FlagRaiser : MonoBehaviour
{
    [Header("Respawn Cooldown")]
    [Tooltip("The unique ID for this boss, e.g., 'GoblinKing'. Must match an ID in the FlagHolder.")]
    public string bossId;

    [Header("Permanent Flag")]
    [Tooltip("Check this ONLY if this boss should trigger the *permanent* 'isSpecialEnemyDefeated' flag (for path blockers, etc.)")]
    public bool isSpecialPathBlockerBoss = false;


    private void OnDestroy()
    {
        // Check if an Instance exists in case we are quitting the application.
        if (FlagHolder.Instance == null)
        {
            return;
        }

        // --- 1. Handle Respawn Cooldown ---
        // If this boss has an ID, record its defeat time.
        if (!string.IsNullOrEmpty(bossId))
        {
            FlagHolder.Instance.RecordBossDefeat(bossId);
        }

        // --- 2. Handle Permanent Flag ---
        // If this is the special boss, set the permanent flag.
        if (isSpecialPathBlockerBoss)
        {
            Debug.Log("Special Enemy (Path Blocker) has been destroyed. Raising the permanent flag!");
            FlagHolder.Instance.isSpecialEnemyDefeated = true;
        }
    }
}
