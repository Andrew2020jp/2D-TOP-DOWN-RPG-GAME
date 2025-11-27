using UnityEngine;

public class WeaponAudioController : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("The AudioSource component that will play the sound.")]
    public AudioSource weaponAudioSource;

    [Tooltip("The specific sound clip to play when attacking.")]
    public AudioClip soundClip;

    [Header("Pitch Settings")]
    [Tooltip("Optional: Randomize pitch slightly for variety?")]
    public bool randomizePitch = true;

    [Range(-3.0f, 2.9f)]
    public float pitchRangeMin = 0.9f;
    [Range(-2.9f, 3.0f)]
    public float pitchRangeMax = 1.1f;

    // Note: Update() and Input checks are removed. 
    // This script now waits for another script to call PlayAttackSound().

    public void PlayAttackSound()
    {
        // Safety check
        if (weaponAudioSource != null && soundClip != null)
        {
            if (randomizePitch)
            {
                weaponAudioSource.pitch = Random.Range(pitchRangeMin, pitchRangeMax);
            }
            else
            {
                weaponAudioSource.pitch = 1.0f;
            }

            weaponAudioSource.PlayOneShot(soundClip);
        }
    }
}