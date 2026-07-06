using UnityEngine;

public class GoldGun : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject goldBullet;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private int goldCostPerShot = 1;

    readonly int TRIGGER_HASH = Animator.StringToHash("Trigger");

    private Animator myAnimator;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void Attack()
    {
        // ❌ Not enough gold → stop
        if (!EconomyManager.Instance.SpendGold(goldCostPerShot))
        {
            Debug.Log("Not enough gold to fire!");
            return;
        }

        // ✅ Fire
        myAnimator.SetTrigger(TRIGGER_HASH);

        GameObject newBullet = Instantiate(
            goldBullet,
            bulletSpawnPoint.position,
            ActiveWeapon.Instance.transform.rotation
        );

        newBullet
            .GetComponent<Projectile>()
            .UpdateProjectileRange(weaponInfo.weaponRange);
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}