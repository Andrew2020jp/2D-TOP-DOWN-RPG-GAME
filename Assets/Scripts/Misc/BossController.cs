using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject exitPortal; // Assign your exit portal GameObject in the inspector

    void Start()
    {
        // Ensure the portal is initially inactive
        if (exitPortal != null)
        {
            exitPortal.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the boss is destroyed
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss == null)
        {
            ShowExitPortal();
        }
    }

    void ShowExitPortal()
    {
        if (exitPortal != null)
        {
            exitPortal.SetActive(true);
        }
    }
}

