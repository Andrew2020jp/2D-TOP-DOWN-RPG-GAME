using UnityEngine;

public class BossDestructible : MonoBehaviour
{
    [Tooltip("The visual effect to create when the object is destroyed.")]
    [SerializeField] private GameObject destroyVFX;

    [Tooltip("How many times the boss must collide with this object to destroy it.")]
    [SerializeField] private int hitsToDestroy = 3;

    private int currentHits = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            currentHits++;
            // This message will appear in the Console every time the boss hits the object
            //Debug.Log(gameObject.name + " hit! Current hits: " + currentHits + " / " + hitsToDestroy, this);

            if (currentHits >= hitsToDestroy)
            {
                PerformDestruction();
            }
        }
    }

    private void PerformDestruction()
    {
        //Debug.Log(gameObject.name + " has taken enough hits. Destroying now.", this);

        // Safely try to spawn pickups
        PickUpSpawner spawner = GetComponent<PickUpSpawner>();
        if (spawner != null)
        {
            spawner.DropItems();
        }

        // Safely instantiate the visual effect
        if (destroyVFX != null)
        {
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
        }
        else
        {
            // If the VFX is missing, print a warning instead of an error
            Debug.LogWarning("Destroy VFX is not assigned on " + gameObject.name, this);
        }

        // Finally, destroy this object
        Destroy(gameObject);
    }
}