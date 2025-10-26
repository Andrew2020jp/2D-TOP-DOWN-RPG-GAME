using UnityEngine;

// This component's only job is to tell the GameManager when this object is destroyed.
// Add this script ONLY to the one special enemy you want to track.
public class FlagRaiser : MonoBehaviour
{
    // OnDestroy is a built-in Unity function that is automatically called
    // when a GameObject is destroyed.
    private void OnDestroy()
    {
        // We check if an Instance exists in case we are quitting the application,
        // which can sometimes destroy objects in a weird order.
        if (FlagHolder.Instance != null)
        {
            Debug.Log("Special Enemy has been destroyed. Raising the flag!");
            FlagHolder.Instance.isSpecialEnemyDefeated = true;
        }
    }
}