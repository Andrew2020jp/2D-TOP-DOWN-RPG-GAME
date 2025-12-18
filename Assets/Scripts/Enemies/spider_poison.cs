using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Moves the projectile in its "forward" direction (Right in 2D)
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Trigger player damage here
            Destroy(gameObject);
        }
    }
}