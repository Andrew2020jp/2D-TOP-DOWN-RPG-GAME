using UnityEngine;

public class PathBlocker : MonoBehaviour
{
    void Start()
    {
        // Check if the GameManager exists and if the flag is true.
        if (FlagHolder.Instance != null && FlagHolder.Instance.isSpecialEnemyDefeated)
        {
            // --- FLAG IS RAISED, SO REMOVE THE BLOCKER ---
            Debug.Log("Flag was raised. Path blocker is being removed.");
            gameObject.SetActive(false); // Deactivate the object, clearing the path.
        }
        else
        {
            Debug.Log("Flag is not raised. Path blocker remains.");
        }
    }
}