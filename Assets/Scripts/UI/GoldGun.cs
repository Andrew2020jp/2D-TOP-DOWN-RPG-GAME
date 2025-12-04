using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldGun : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject goldBullet;
    [SerializeField] private Transform bulletSpawnPoint;
    //public WeaponAudioController audioController;

    readonly int TRIGGER_HASH = Animator.StringToHash("Trigger");

    private Animator myAnimator;
    
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();    
    }

    public void Attack()
    {
        myAnimator.SetTrigger(TRIGGER_HASH);
        GameObject newBullet = Instantiate(goldBullet, bulletSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
        newBullet.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
        /*if (audioController != null)
        {
            audioController.PlayAttackSound();
        }
        */
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}