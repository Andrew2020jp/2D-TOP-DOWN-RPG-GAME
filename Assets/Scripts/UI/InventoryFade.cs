using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryFade : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float fadedAlpla = 0.05f;  // Opacity of the UI
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float bufferZone = 30f;    // To prevent flickering

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Convert player world(map/scene) position -> screen position(x and y)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position);
        float playerY = screenPos.y;

        float panelHeight = rectTransform.rect.height;

        float targetAlpha;

        if (playerY < panelHeight)                      // fade when player is behind the UI
        {
            targetAlpha = fadedAlpla;
        }
        else if (playerY > panelHeight + bufferZone)    // fade back in when player left the UI area
        {
            targetAlpha = 1f;
        }
        else
        {
            return;
        }

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }
}
