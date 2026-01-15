using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadedAlpha = 0.05f;   // UI最低透明度
    [SerializeField] private float fadeSpeed = 5f;        // 淡入淡出速度
    [SerializeField] private float bufferZone = 30f;      // 防止闪烁的缓冲区

    private Transform player;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        FindPlayer();
    }

    void Update()
    {
        // Player 可能被 Destroy（死亡 / 切场景），必须检查
        if (player == null)
        {
            FindPlayer();
            return;
        }

        Camera cam = Camera.main;
        if (cam == null) return;

        // 世界坐标 → 屏幕坐标
        Vector3 screenPos = cam.WorldToScreenPoint(player.position);

        // 如果玩家在镜头后面，直接显示 UI
        if (screenPos.z < 0f)
        {
            FadeTo(1f);
            return;
        }

        float playerY = screenPos.y;
        float panelHeight = rectTransform.rect.height;

        if (playerY < panelHeight)
        {
            FadeTo(fadedAlpha);
        }
        else if (playerY > panelHeight + bufferZone)
        {
            FadeTo(1f);
        }
    }

    /// <summary>
    /// 平滑淡入淡出
    /// </summary>
    private void FadeTo(float targetAlpha)
    {
        canvasGroup.alpha = Mathf.Lerp(
            canvasGroup.alpha,
            targetAlpha,
            Time.deltaTime * fadeSpeed
        );
    }

    /// <summary>
    /// 自动寻找 Player（支持重生 / 切场景）
    /// </summary>
    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    /// <summary>
    /// 给外部（如 PlayerSpawner）手动绑定 Player（推荐）
    /// </summary>
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}
