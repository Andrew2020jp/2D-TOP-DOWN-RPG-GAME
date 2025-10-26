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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() && !loadStarted)
        {
            loadStarted = true; // Mark that the load process has begun

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