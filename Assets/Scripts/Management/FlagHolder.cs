using UnityEngine;
using System.Collections.Generic; // Needed for Dictionaries and Lists

// This is a Singleton that persists across scenes to hold game state.
public class FlagHolder : MonoBehaviour
{
    // The static instance that can be accessed from anywhere.
    public static FlagHolder Instance { get; private set; }

    // --- 1. ORIGINAL FLAG ---
    // This bool tracks if the *permanent* path-blocking enemy is defeated.
    public bool isSpecialEnemyDefeated = false;

    // --- 2. NEW COOLDOWN SYSTEM ---

    // This is a custom class that will let you define cooldowns
    // in the Inspector for each boss.
    [System.Serializable]
    public class BossCooldownConfig
    {
        public string bossId; // Must be a unique name, e.g., "GoblinKing"
        public float cooldownInSeconds = 3600f; // Default 1 hour
    }

    // You can drag/drop and set up all your bosses here in the Inspector.
    [Header("Boss Respawn Cooldowns")]
    public List<BossCooldownConfig> bossCooldowns = new List<BossCooldownConfig>();

    // These dictionaries are for fast lookup.
    // Maps bossId -> cooldown duration
    private Dictionary<string, float> bossCooldownMap = new Dictionary<string, float>();
    // Maps bossId -> time of defeat
    private Dictionary<string, float> bossDefeatTimestamps = new Dictionary<string, float>();


    void Awake()
    {
        // --- Singleton Pattern ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // --- New: Populate the cooldown map ---
            InitializeCooldownMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Converts the public list into a fast-lookup dictionary.
    /// </summary>
    private void InitializeCooldownMap()
    {
        bossCooldownMap.Clear();
        foreach (var config in bossCooldowns)
        {
            if (!bossCooldownMap.ContainsKey(config.bossId))
            {
                bossCooldownMap.Add(config.bossId, config.cooldownInSeconds);
            }
            else
            {
                Debug.LogWarning($"Duplicate bossId found in FlagHolder: {config.bossId}");
            }
        }
    }

    // --- NEW METHODS FOR COOLDOWN SYSTEM ---

    /// <summary>
    /// Called by a FlagRaiser when its boss is destroyed.
    /// Records the time of defeat.
    /// </summary>
    public void RecordBossDefeat(string bossId)
    {
        if (bossCooldownMap.ContainsKey(bossId))
        {
            // We use realtimeSinceStartup as it's not affected by
            // Time.timeScale (pausing) and persists across scene loads.
            float defeatTime = Time.realtimeSinceStartup;
            bossDefeatTimestamps[bossId] = defeatTime;
            Debug.Log($"Recorded defeat for '{bossId}' at time {defeatTime}. Cooldown started.");
        }
        else
        {
            Debug.LogWarning($"RecordBossDefeat called with unknown bossId: {bossId}");
        }
    }

    /// <summary>
    /// Called by a BossSpawner to check if a boss is allowed to respawn.
    /// </summary>
    public bool CanBossSpawn(string bossId)
    {
        // 1. Do we even track this boss?
        if (!bossCooldownMap.ContainsKey(bossId))
        {
            Debug.LogWarning($"CanBossSpawn check for untracked bossId: {bossId}. Allowing spawn.");
            return true; // Not a tracked boss, so let it spawn.
        }

        // 2. Has this boss been defeated before?
        if (!bossDefeatTimestamps.ContainsKey(bossId))
        {
            return true; // Never been defeated, so let it spawn.
        }

        // 3. If defeated, has the cooldown expired?
        float lastDefeatTime = bossDefeatTimestamps[bossId];
        float cooldownDuration = bossCooldownMap[bossId];
        float timePassed = Time.realtimeSinceStartup - lastDefeatTime;

        if (timePassed >= cooldownDuration)
        {
            Debug.Log($"Cooldown for '{bossId}' has ended. Allowing spawn.");
            return true; // Cooldown is over.
        }
        else
        {
            // Cooldown is still active.
            float timeLeft = cooldownDuration - timePassed;
            Debug.Log($"Cooldown for '{bossId}' is still active. {timeLeft:F0}s remaining.");
            return false;
        }
    }
}
