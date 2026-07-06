using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Watches the final boss scene; when the boss dies, fades in an ending screen
// and returns to the main menu on any key press. The watcher spawns itself via
// the bootstrap below, so no scene or prefab wiring is needed.
public class GameEnding : MonoBehaviour
{
    // Scenes whose boss death triggers the ending. Add more scene names here
    // if another boss should also end the game.
    private static readonly string[] finalBossScenes = { "Scene Boss-Andrew" };

    // The boss is identified by its GameObject name containing this text
    // (matches the "Big Ghost " prefab instance, but not its summoned "Ghost" minions).
    private const string BOSS_NAME_CONTAINS = "Big Ghost";

    private const string MAIN_MENU_SCENE = "Main Menu";
    private const float DELAY_BEFORE_FADE = 2f;
    private const float FADE_DURATION = 1.5f;

    private Transform boss;
    private bool bossSeen = false;
    private bool endingStarted = false;
    private bool canReturnToMenu = false;
    private CanvasGroup canvasGroup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (string sceneName in finalBossScenes)
        {
            if (scene.name == sceneName)
            {
                new GameObject("Game Ending Watcher").AddComponent<GameEnding>();
                return;
            }
        }
    }

    private void Update()
    {
        if (endingStarted)
        {
            if (canReturnToMenu && Input.anyKeyDown)
            {
                ReturnToMainMenu();
            }
            return;
        }

        if (!bossSeen)
        {
            FindBoss();
        }
        else if (boss == null)
        {
            endingStarted = true;
            StartCoroutine(EndingRoutine());
        }
    }

    private void FindBoss()
    {
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy.name.Contains(BOSS_NAME_CONTAINS))
            {
                boss = enemy.transform;
                bossSeen = true;
                return;
            }
        }
    }

    private IEnumerator EndingRoutine()
    {
        // let the death VFX play before taking over the screen
        yield return new WaitForSeconds(DELAY_BEFORE_FADE);

        BuildEndingCanvas();
        Time.timeScale = 0f;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / FADE_DURATION;
            yield return null;
        }

        canReturnToMenu = true;
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        if (ActiveWeapon.Instance != null)
        {
            Destroy(ActiveWeapon.Instance.gameObject);
        }
        if (PlayerController.Instance != null)
        {
            Destroy(PlayerController.Instance.gameObject);
        }

        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }

    private void BuildEndingCanvas()
    {
        GameObject canvasObject = new GameObject("Ending Canvas");
        canvasObject.transform.SetParent(transform);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        Image background = CreateChild<Image>(canvasObject.transform, "Background");
        background.color = Color.black;
        StretchToFill(background.rectTransform);

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Text title = CreateChild<Text>(canvasObject.transform, "Title");
        SetUpText(title, font, "The End", 96, Color.white, new Vector2(0f, 120f));

        Text subtitle = CreateChild<Text>(canvasObject.transform, "Subtitle");
        SetUpText(subtitle, font, "You defeated the final boss.\nThanks for playing!",
            34, new Color(0.8f, 0.8f, 0.8f), new Vector2(0f, -20f));

        Text hint = CreateChild<Text>(canvasObject.transform, "Hint");
        SetUpText(hint, font, "Press any key to return to the main menu",
            24, new Color(0.6f, 0.6f, 0.6f), new Vector2(0f, -200f));
    }

    private T CreateChild<T>(Transform parent, string name) where T : Component
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent, false);
        return child.AddComponent<T>();
    }

    private void SetUpText(Text text, Font font, string content, int size, Color color, Vector2 anchoredPosition)
    {
        text.font = font;
        text.text = content;
        text.fontSize = size;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;

        RectTransform rectTransform = text.rectTransform;
        rectTransform.sizeDelta = new Vector2(1600f, 200f);
        rectTransform.anchoredPosition = anchoredPosition;
    }

    private void StretchToFill(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
