using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 6f;
    private Transform target;

    public void Init(Transform playerTarget)
    {
        target = playerTarget;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // damage player here
            Destroy(gameObject);
        }
    }
}
