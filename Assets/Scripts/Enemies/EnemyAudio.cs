using UnityEngine;

public class EnemyAudioController : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("The AudioSource component on the enemy.")]
    public AudioSource enemyAudioSource;

    [Tooltip("The sound to play when the enemy dies.")]
    public AudioClip deathClip;

    [Header("Pitch Settings")]
    [Tooltip("Randomize pitch to make enemies sound slightly different.")]
    public bool randomizePitch = true;

    [Range(0.8f, 1.2f)]
    public float pitchRangeMin = 0.8f;
    [Range(0.8f, 1.2f)]
    public float pitchRangeMax = 1.2f;

    private void Start()
    {
        // Auto-fetch the audio source if you forgot to drag it in
        if (enemyAudioSource == null)
        {
            enemyAudioSource = GetComponent<AudioSource>();
        }
    }

    // Call this INSTEAD of Destroy(gameObject) in your EnemyHealth script
    public void PlayDeathSoundAndDestroy()
    {
        // 1. If no sound is set, just destroy the enemy immediately
        if (deathClip == null)
        {
            Destroy(gameObject);
            return;
        }

        // 2. Create a temporary "Ghost" object at the enemy's position
        // We do this because the Enemy object is about to be destroyed, 
        // and we need something left behind to play the sound.
        GameObject tempAudioObj = new GameObject("TempEnemyDeathAudio");
        tempAudioObj.transform.position = transform.position;

        // 3. Add an AudioSource to the ghost and copy settings (volume, 3D settings, etc.)
        AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();

        if (enemyAudioSource != null)
        {
            tempSource.volume = enemyAudioSource.volume;
            tempSource.spatialBlend = enemyAudioSource.spatialBlend;
            tempSource.outputAudioMixerGroup = enemyAudioSource.outputAudioMixerGroup;
        }

        // 4. Randomize the pitch
        if (randomizePitch)
        {
            tempSource.pitch = Random.Range(pitchRangeMin, pitchRangeMax);
        }

        // 5. Play the sound
        tempSource.PlayOneShot(deathClip);

        // 6. Destroy the "Ghost" object after the clip finishes
        Destroy(tempAudioObj, deathClip.length);

        // 7. Finally, remove the actual Enemy from the game
        Destroy(gameObject);
    }
}