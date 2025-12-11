using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneTransitionName;

    [Header("Audio Settings")]
    [Tooltip("If empty, it will try to load 'AreaExitDefault' from the Resources folder.")]
    [SerializeField] private AudioClip exitSound;

    private AudioSource areaSource;
    private float waitToLoadTime = 1f;

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
            exitSound = Resources.Load<AudioClip>("45_Landing_01");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            // 3. Play Audio Logic
            if (areaSource != null)
            {
                // We prioritize the 'exitSound' variable (which might now be the loaded Resource)
                if (exitSound != null)
                {
                    areaSource.PlayOneShot(exitSound);
                }
                // Fallback to the component's clip if for some reason the above failed
                else if (areaSource.clip != null)
                {
                    areaSource.PlayOneShot(areaSource.clip);
                }
            }

            // 4. Handle visual transitions
            SceneManagement.Instance.SetTransitionName(sceneTransitionName);
            UIFade.Instance.FadeToBlack();

            // 5. Start the timer
            StartCoroutine(LoadSceneRoutine());
        }
    }

    private IEnumerator LoadSceneRoutine()
    {
        while (waitToLoadTime >= 0)
        {
            waitToLoadTime -= Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}