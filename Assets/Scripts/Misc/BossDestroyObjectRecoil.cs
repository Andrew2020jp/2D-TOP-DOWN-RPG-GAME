using UnityEngine;

// We can now require the Knockback component since we rely on it.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Knockback))]
public class BossDestroyObjectRecoil : MonoBehaviour
{
    [Tooltip("The force of the recoil when the boss hits a destructible object.")]
    [SerializeField] private float recoilForce = 5f;

    // Reference to your existing Knockback script
    private Knockback knockback;

    private void Awake()
    {
        // Get the Knockback component that's already on the boss
        knockback = GetComponent<Knockback>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit a destructible object
        if (collision.gameObject.GetComponent<BossDestructible>() != null)
        {
            // Use your existing Knockback system to apply the recoil.
            // The "damage source" is the obstacle we collided with.
            knockback.GetKnockedBack(collision.transform, recoilForce);
        }
    }
}