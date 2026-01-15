using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : Singleton<Stamina>
{
    public int CurrentStamina { get; private set; }

    [SerializeField] private Sprite fullStaminaImage, emptyStaminaImage;
    [SerializeField] private int timeBetweenStaminaRefresh = 3;
    [SerializeField] private GameObject staminaIconPrefab;

    private Transform staminaContainer;
    private int startingStamina = 3;
    private int maxStamina;
    const string STAMINA_CONTAINER_TEXT = "Stamina Container";

    protected override void Awake()
    {
        base.Awake();

        maxStamina = startingStamina;
        CurrentStamina = startingStamina;
    }

    private void Start()
    {
        staminaContainer = GameObject.Find(STAMINA_CONTAINER_TEXT).transform;
    }

    public void UseStamina()
    {
        CurrentStamina--;
        UpdateStaminaImages();
    }

    public void RefreshStamina()
    {
        if (CurrentStamina < maxStamina) 
        {
            CurrentStamina++;
        }
        UpdateStaminaImages();
    }

    public void ResetStamina()
    {
        CurrentStamina = maxStamina;
        StopAllCoroutines();
    }

    // --- CHANGE 'private' to 'public' AND ADD A NULL CHECK ---
    public void UpdateStaminaImages()
    {
        if (staminaContainer == null) { /* ... keep your existing find logic ... */ }

        // Use the actual number of children currently in the container 
        // to avoid index out of bounds errors
        int childCount = staminaContainer.childCount;

        for (int i = 0; i < maxStamina; i++)
        {
            // Safety check: skip if the UI hasn't caught up to the logic yet
            if (i >= childCount) break;

            Transform child = staminaContainer.GetChild(i);
            Image image = child.GetComponent<Image>();

            if (image != null)
            {
                image.sprite = (i <= CurrentStamina - 1) ? fullStaminaImage : emptyStaminaImage;
            }
        }

        if (CurrentStamina < maxStamina)
        {
            StopAllCoroutines();
            StartCoroutine(RefreshStaminaRoutine());
        }
    }

    private IEnumerator RefreshStaminaRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenStaminaRefresh);
            RefreshStamina();
        }
    }

    public void IncreaseMaxStamina(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            maxStamina++;

            if (staminaIconPrefab != null && staminaContainer != null)
            {
                // Instantiate and wait a frame or force the hierarchy to update
                GameObject newIcon = Instantiate(staminaIconPrefab, staminaContainer);
                newIcon.name = "Stamina Image " + maxStamina;
            }
        }

        CurrentStamina = maxStamina;

        // Use a slight delay to let Unity's UI/Transform system catch up
        StartCoroutine(UpdateUIAfterFrame());
    }

    private IEnumerator UpdateUIAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        UpdateStaminaImages();
    }

    /*
    private void UpdateStaminaImages()
    {
        for(int i = 0; i < maxStamina; i++)
        {
            if(i <= CurrentStamina - 1) {
                staminaContainer.GetChild(i).GetComponent<Image>().sprite = fullStaminaImage;
            } else {
                staminaContainer.GetChild(i).GetComponent<Image>().sprite = emptyStaminaImage;
            }
        }

        if(CurrentStamina < maxStamina)
        {
            StopAllCoroutines();
            StartCoroutine(RefreshStaminaRoutine());
        }
    }
    */
}
