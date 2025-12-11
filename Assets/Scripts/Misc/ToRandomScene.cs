using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToRandomScene : MonoBehaviour
{
    // Changed to arrays to hold multiple options
    [SerializeField] private string[] scenesToLoad;
    [SerializeField] private string[] sceneTransitionNames;

    // A check to prevent the trigger from being activated multiple times
    private bool loadStarted = false;

    [Header("Audio Settings")]
    [Tooltip("If empty, it will try to load 'AreaExitDefault' from the Resources folder.")]
    [SerializeField] private AudioClip exitSound;

    private AudioSource areaSource;

    private void Start()
    {
        // 1. AUTO-SETUP: Find AudioSource, or Add one if it's missing
        areaSource = GetComponent<AudioSource>();

        if (areaSource == null)
        {
            // Automatically add the component so you don't have to edit 100 objects
            areaSource = gameObject.AddComponent<AudioSource>();
            areaSource.playOnAwake = false;
        }

        // 2. AUTO-SETUP: If no sound is dragged in, load a default one
        if (exitSound == null)
        {
            // Looks for a file named "AreaExitDefault" inside any "Resources" folder
            exitSound = Resources.Load<AudioClip>("AreaExitDefault");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() && !loadStarted)
        {
            loadStarted = true; // Mark that the load process has begun

            if (areaSource != null)
            {
                // Priority 1: Use the specific clip assigned in this script
                if (exitSound != null)
                {
                    areaSource.PlayOneShot(exitSound);
                }
                // Priority 2: Use the clip assigned in the Inspector's Audio Source component
                else if (areaSource.clip != null)
                {
                    areaSource.PlayOneShot(areaSource.clip);
                }
            }

            // --- Core Change: Randomly select an index ---
            // This will pick a random number between 0 and the number of scenes available.
            // For two scenes, this will be either 0 or 1.
            int randomIndex = Random.Range(0, scenesToLoad.Length);

            // Set the transition name using the same random index
            SceneManagement.Instance.SetTransitionName(sceneTransitionNames[randomIndex]);

            UIFade.Instance.FadeToBlack(); // fade to black when exiting area

            // Start the coroutine, passing in the randomly selected scene name
            StartCoroutine(LoadSceneRoutine(scenesToLoad[randomIndex]));
        }
    }

    // The coroutine now accepts the scene name as a parameter
    private IEnumerator LoadSceneRoutine(string sceneToLoad)
    {
        // Use a dedicated wait time so it's consistent for every load
        yield return new WaitForSeconds(1f);

        // Load the scene that was passed into the coroutine
        SceneManager.LoadScene(sceneToLoad);
    }
}