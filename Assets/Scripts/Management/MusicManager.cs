using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Music Settings")]
    [Tooltip("The music clip to play. If you change this in a different scene, the music will crossfade or switch.")]
    public AudioClip backgroundMusic;

    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    // Singleton instance to ensure only one MusicManager exists
    public static MusicManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        // 1. Singleton Logic: Ensure we don't have duplicate music players
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // This keeps the object alive when changing scenes

            // Setup the audio source
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Configure
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = musicVolume;

            PlayMusic();
        }
        else
        {
            // If a MusicManager already exists (from a previous scene), destroy this new one.
            // This prevents having two songs playing at once.
            Destroy(gameObject);
        }
    }

    public void PlayMusic()
    {
        if (backgroundMusic != null)
        {
            // If the music is already playing, don't restart it!
            if (audioSource.clip == backgroundMusic && audioSource.isPlaying)
            {
                return;
            }

            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
        else
        {
            // If backgroundMusic is null, we should STOP playing.
            audioSource.Stop();
        }
    }

    // Optional: Call this from other scripts to change tracks
    public void ChangeMusic(AudioClip newClip)
    {
        // Only trigger a change if the new clip is actually different
        if (newClip != backgroundMusic)
        {
            backgroundMusic = newClip;
            PlayMusic();
        }
    }
}