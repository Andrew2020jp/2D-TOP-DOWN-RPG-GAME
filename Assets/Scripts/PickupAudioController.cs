using UnityEngine;

public class PickupAudioController : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("The AudioSource component on the pickup item.")]
    public AudioSource pickupAudioSource;

    [Tooltip("The sound to play when picked up.")]
    public AudioClip pickupClip;

    [Header("Pitch Settings")]
    [Tooltip("Randomize pitch to make collecting multiple items sound interesting.")]
    public bool randomizePitch = true;

    [Range(-3.0f, 2.9f)]
    public float pitchRangeMin = 0.9f;
    [Range(-2.9f, 3.0f)]
    public float pitchRangeMax = 1.1f;

    // Call this method if you are NOT destroying the object immediately
    public void PlayPickupSound()
    {
        if (pickupAudioSource != null && pickupClip != null)
        {
            ApplyPitch(pickupAudioSource);
            pickupAudioSource.PlayOneShot(pickupClip);
        }
    }

    // Call this method if you WANT to destroy the object immediately
    public void PlaySoundAndDestroy()
    {
        if (pickupClip == null)
        {
            Destroy(gameObject);
            return;
        }

        // 1. Create a temporary GameObject to act as the speaker
        GameObject tempAudioObj = new GameObject("TempPickupAudio");
        tempAudioObj.transform.position = transform.position;

        // 2. Add an AudioSource and copy settings from our original source
        AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();

        if (pickupAudioSource != null)
        {
            tempSource.volume = pickupAudioSource.volume;
            tempSource.spatialBlend = pickupAudioSource.spatialBlend; // Keeps 2D/3D settings
            tempSource.outputAudioMixerGroup = pickupAudioSource.outputAudioMixerGroup;
        }

        // 3. Apply our randomized pitch
        ApplyPitch(tempSource);

        // 4. Play the sound
        tempSource.PlayOneShot(pickupClip);

        // 5. Destroy the temp speaker after the clip is done
        Destroy(tempAudioObj, pickupClip.length);

        // 6. Finally, destroy the actual pickup item
        Destroy(gameObject);
    }

    // Helper function to keep code clean
    private void ApplyPitch(AudioSource source)
    {
        if (randomizePitch)
        {
            source.pitch = Random.Range(pitchRangeMin, pitchRangeMax);
        }
        else
        {
            source.pitch = 1.0f;
        }
    }
}