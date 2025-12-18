using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [Tooltip("The music you want to play in THIS scene. If you leave this empty, the music will stop.")]
    public AudioClip sceneMusic;

    private void Start()
    {
        // Ensure the MusicManager exists before trying to access it
        // (This prevents errors if you start the game directly in this scene without loading the MainMenu first)
        if (MusicManager.Instance != null)
        {
            // Tell the manager to switch to this scene's music
            MusicManager.Instance.ChangeMusic(sceneMusic);
        }
    }
}