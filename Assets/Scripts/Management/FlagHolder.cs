using UnityEngine;

// This is a Singleton that persists across scenes to hold game state.
public class FlagHolder : MonoBehaviour
{
    // The static instance that can be accessed from anywhere.
    public static FlagHolder Instance { get; private set; }

    // THE FLAG: This bool will track if the specific enemy is defeated.
    public bool isSpecialEnemyDefeated = false;

    void Awake()
    {
        // This is the Singleton pattern logic.
        if (Instance == null)
        {
            // If this is the first instance, make it the singleton.
            Instance = this;
            // Don't destroy this object when loading a new scene.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this one.
            Destroy(gameObject);
        }
    }
}