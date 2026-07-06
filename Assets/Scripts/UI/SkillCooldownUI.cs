using UnityEngine;
using UnityEngine.UI;

// Q/E/R skill slots styled to match the weapon inventory bar (same Box sprite,
// tint, 120x120 cells and 20px spacing), placed just right of the weapon bar.
// Spawns itself at startup and builds its UI at runtime, so no scene or prefab
// wiring is needed. Sprites are loaded from Assets/Resources/SkillIcons.
// Hides itself whenever there is no player (main menu, after death).
public class SkillCooldownUI : MonoBehaviour
{
    private const float SLOT_SIZE = 120f;   // same cell size as the weapon bar grid
    private const float SLOT_SPACING = 20f; // same spacing as the weapon bar grid
    private const float BAR_CENTER_Y = 100f; // weapon bar center height above screen bottom
    private const float FIRST_SLOT_CENTER_X = 445f; // right edge of weapon bar (365) + spacing + half slot

    private static SkillCooldownUI instance;

    private GameObject canvasObject;
    private Image[] cooldownOverlays = new Image[3];
    private GameObject shieldHighlight;

    // same tint the weapon bar uses on its Box slots / Active outline
    private readonly Color slotColor = new Color(0.95113087f, 0.98490566f, 0.71916693f, 1f);
    private readonly Color highlightColor = new Color(0.6622641f, 1f, 0.97226566f, 1f);
    private readonly Color shieldIconTint = new Color(0.5f, 0.85f, 1f, 1f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) { return; }

        instance = new GameObject("Skill Cooldown UI").AddComponent<SkillCooldownUI>();
        DontDestroyOnLoad(instance.gameObject);
    }

    private void Awake()
    {
        BuildCanvas();
    }

    private void Update()
    {
        PlayerController player = PlayerController.Instance;

        bool showHud = player != null && PlayerHealth.Instance != null && !PlayerHealth.Instance.isDead;
        if (canvasObject.activeSelf != showHud)
        {
            canvasObject.SetActive(showHud);
        }
        if (!showHud) { return; }

        cooldownOverlays[0].fillAmount = player.HealCooldownProgress;
        cooldownOverlays[1].fillAmount = player.SpinAttackCooldownProgress;
        cooldownOverlays[2].fillAmount = player.ShieldCooldownProgress;

        shieldHighlight.SetActive(PlayerHealth.Instance.IsShieldActive);
    }

    private void BuildCanvas()
    {
        canvasObject = new GameObject("Skill Canvas");
        canvasObject.transform.SetParent(transform);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        // same scaling setup as the UICanvas prefab so sizes match on screen
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0f;

        Sprite boxSprite = Resources.Load<Sprite>("SkillIcons/slot_box");
        Sprite outlineSprite = Resources.Load<Sprite>("SkillIcons/slot_outline");

        string[] keys = { "Q", "E", "R" };
        string[] iconNames = { "SkillIcons/icon_heal", "SkillIcons/icon_lightning", "SkillIcons/icon_shield" };
        Color[] iconTints = { Color.white, Color.white, shieldIconTint }; // lightning icon is already colored

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        for (int i = 0; i < 3; i++)
        {
            BuildSlot(i, keys[i], Resources.Load<Sprite>(iconNames[i]), iconTints[i], boxSprite, outlineSprite, font);
        }
    }

    private void BuildSlot(int index, string key, Sprite icon, Color iconTint, Sprite boxSprite, Sprite outlineSprite, Font font)
    {
        GameObject slot = new GameObject("Slot " + key);
        slot.transform.SetParent(canvasObject.transform, false);

        Image background = slot.AddComponent<Image>();
        background.sprite = boxSprite;
        background.color = slotColor;

        RectTransform slotRect = background.rectTransform;
        slotRect.anchorMin = new Vector2(0.5f, 0f);
        slotRect.anchorMax = new Vector2(0.5f, 0f);
        slotRect.pivot = new Vector2(0.5f, 0.5f);
        slotRect.sizeDelta = new Vector2(SLOT_SIZE, SLOT_SIZE);
        slotRect.anchoredPosition = new Vector2(FIRST_SLOT_CENTER_X + index * (SLOT_SIZE + SLOT_SPACING), BAR_CENTER_Y);

        // skill icon, like the weapon sprite inside an inventory slot
        Image iconImage = CreateChildImage(slot.transform, "Icon");
        iconImage.sprite = icon;
        iconImage.color = iconTint;
        iconImage.preserveAspect = true;
        iconImage.rectTransform.sizeDelta = new Vector2(SLOT_SIZE * 0.6f, SLOT_SIZE * 0.6f);

        // cooldown overlay: fills the box shape and empties as the skill recharges
        Image overlay = CreateChildImage(slot.transform, "Cooldown Overlay");
        overlay.sprite = boxSprite;
        overlay.color = new Color(0f, 0f, 0f, 0.62f);
        overlay.type = Image.Type.Filled;
        overlay.fillMethod = Image.FillMethod.Vertical;
        overlay.fillOrigin = (int)Image.OriginVertical.Bottom;
        overlay.fillAmount = 0f;
        overlay.rectTransform.sizeDelta = new Vector2(SLOT_SIZE, SLOT_SIZE);
        cooldownOverlays[index] = overlay;

        // outline shown while the shield is active, same as the weapon bar highlight
        if (index == 2)
        {
            Image outline = CreateChildImage(slot.transform, "Shield Highlight");
            outline.sprite = outlineSprite;
            outline.color = highlightColor;
            outline.rectTransform.sizeDelta = new Vector2(SLOT_SIZE, SLOT_SIZE);
            shieldHighlight = outline.gameObject;
            shieldHighlight.SetActive(false);
        }

        // key hint in the corner of the slot
        GameObject textObject = new GameObject("Key " + key);
        textObject.transform.SetParent(slot.transform, false);
        Text keyText = textObject.AddComponent<Text>();
        keyText.font = font;
        keyText.text = key;
        keyText.fontSize = 26;
        keyText.fontStyle = FontStyle.Bold;
        keyText.color = new Color(0.35f, 0.27f, 0.16f, 1f);
        keyText.alignment = TextAnchor.UpperLeft;
        keyText.raycastTarget = false;
        RectTransform textRect = keyText.rectTransform;
        textRect.sizeDelta = new Vector2(SLOT_SIZE - 24f, SLOT_SIZE - 16f);
        textRect.anchoredPosition = Vector2.zero;
    }

    private Image CreateChildImage(Transform parent, string name)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent, false);
        Image image = child.AddComponent<Image>();
        image.raycastTarget = false;
        return image;
    }
}
