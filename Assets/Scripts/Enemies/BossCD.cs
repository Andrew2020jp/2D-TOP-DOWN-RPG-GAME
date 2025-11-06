using UnityEngine;
using System.Collections; // Using for the 1-frame delay

// Add this component TO THE BOSS object that is already placed in the scene.
// This script will check the FlagHolder on scene load and
// deactivate the boss if its respawn cooldown is still active.
public class BossCD : MonoBehaviour
{
    [Tooltip("The ID of this boss. Must match the ID in the FlagHolder.")]
    public string bossId;

    // We use Start() which runs *after* Awake().
    // We also make it a coroutine to be extra safe and wait one frame.
    IEnumerator Start()
    {
        // Wait for a single frame. This ensures the FlagHolder
        // (which is persistent) is fully ready in the new scene.
        yield return null;

        // Check if the FlagHolder instance exists
        if (FlagHolder.Instance == null)
        {
            Debug.LogWarning("FlagHolder.Instance not found. Boss will be active by default.");
            yield break; // Can't check, so just let the boss be active.
        }

        // Check with the FlagHolder if this boss is allowed to be active.
        if (FlagHolder.Instance.CanBossSpawn(bossId))
        {
            // --- Cooldown is over (or never defeated) ---
            Debug.Log($"Boss '{bossId}' is allowed to be active (cooldown over).");
            gameObject.SetActive(true); // Ensure it's active.
        }
        else
        {
            // --- Cooldown is still active ---
            Debug.Log($"Boss '{bossId}' is on cooldown. Deactivating.");
            gameObject.SetActive(false); // Deactivate the boss.
        }
    }
}
