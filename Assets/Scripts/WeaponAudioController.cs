using UnityEngine;

public class WeaponAudioController : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("The AudioSource component that will play the sound.")]
    public AudioSource weaponAudioSource;

    [Tooltip("The specific sound clip to play when attacking.")]
    public AudioClip slashSound;

    [Header("Input Settings")]
    [Tooltip("The key or mouse button that triggers the attack.")]
    public KeyCode attackTrigger = KeyCode.Mouse0; // Default is Left Click

    [Tooltip("Optional: Randomize pitch slightly for variety?")]
    public bool randomizePitch = true;

    [Range(0.8f, 1.2f)]
    public float pitchRangeMin = 0.9f;
    [Range(0.8f, 1.2f)]
    public float pitchRangeMax = 1.1f;

    void Update()
    {
        // Check if the configured key was pressed down this frame
        if (Input.GetKeyDown(attackTrigger))
        {
            PlayAttackSound();
        }
    }

    void PlayAttackSound()
    {
        // Safety check to ensure we have an AudioSource and a Clip assigned
        if (weaponAudioSource != null && slashSound != null)
        {
            // Optional: Change pitch slightly every time to make it sound less repetitive
            if (randomizePitch)
            {
                weaponAudioSource.pitch = Random.Range(pitchRangeMin, pitchRangeMax);
            }
            else
            {
                weaponAudioSource.pitch = 1.0f; // Reset to normal if randomization is off
            }

            // PlayOneShot allows the sound to overlap if you click fast (doesn't cut off the previous sound)
            weaponAudioSource.PlayOneShot(slashSound);
        }
        else
        {
            Debug.LogWarning("WeaponAudioController: Missing AudioSource or AudioClip assignment!");
        }
    }
}