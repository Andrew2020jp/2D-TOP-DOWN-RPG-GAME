using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// God of War 1 "Poseidon's Rage" style lightning storm: a glowing electric
// nova around the caster, a cage of crackling arcs along its rim, sky bolts
// crashing down on every enemy caught inside, a screen flash, and repeated
// damage ticks while the storm lasts. Built entirely from code — no prefab,
// sprite or material asset is needed.
public class LightningStorm : MonoBehaviour
{
    private const float DURATION = 0.9f;
    private const float TICK_INTERVAL = 0.3f;      // 3 damage ticks over the duration
    private const float ARC_SPAWN_INTERVAL = 0.05f;
    private const float SKY_BOLT_HEIGHT = 5f;

    private Transform center;
    private float radius;
    private int tickDamage;

    private float age;
    private float arcTimer;
    private float tickTimer;
    private SpriteRenderer glow;

    private static Sprite glowSprite;

    public static void Create(Transform center, float radius, int tickDamage)
    {
        GameObject stormObject = new GameObject("Lightning Storm");
        LightningStorm storm = stormObject.AddComponent<LightningStorm>();
        storm.center = center;
        storm.radius = radius;
        storm.tickDamage = tickDamage;
    }

    private void Start()
    {
        transform.position = center.position;
        BuildGlow();
        StartCoroutine(ScreenFlashRoutine());
        DamageTick(); // first hit lands immediately
    }

    private void Update()
    {
        if (center == null)
        {
            Destroy(gameObject);
            return;
        }

        // the storm follows the caster, like Poseidon's Rage does
        transform.position = center.position;

        age += Time.deltaTime;
        if (age >= DURATION)
        {
            Destroy(gameObject);
            return;
        }

        // flickering glow that fades out towards the end of the storm
        float pulse = 0.75f + 0.25f * Mathf.Sin(age * 40f);
        float fade = 1f - Mathf.Clamp01((age - DURATION * 0.6f) / (DURATION * 0.4f));
        glow.color = new Color(0.65f, 0.8f, 1f, 0.45f * pulse * fade);

        arcTimer += Time.deltaTime;
        while (arcTimer >= ARC_SPAWN_INTERVAL)
        {
            arcTimer -= ARC_SPAWN_INTERVAL;
            SpawnArcs();
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= TICK_INTERVAL)
        {
            tickTimer -= TICK_INTERVAL;
            DamageTick();
        }
    }

    private void SpawnArcs()
    {
        Vector3 position = transform.position;

        // arc from the caster out to the edge of the nova
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 rim = position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
        LightningBolt.Create(position, rim);

        // arc running along the rim, caging the area in electricity
        float angle2 = angle + Random.Range(0.8f, 1.6f);
        Vector3 rim2 = position + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0f) * radius;
        LightningBolt.Create(rim, rim2);
    }

    private void DamageTick()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            SlimeKingAI slimeKing = hit.GetComponent<SlimeKingAI>();
            if (enemyHealth == null && slimeKing == null) { continue; }

            // sky bolt crashing down on the enemy plus an arc from the caster
            Vector3 targetPosition = hit.transform.position;
            LightningBolt.Create(targetPosition + Vector3.up * SKY_BOLT_HEIGHT, targetPosition);
            LightningBolt.Create(transform.position, targetPosition);

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(tickDamage);
            }
            else
            {
                slimeKing.TakeDamage(tickDamage);
            }
        }
    }

    private void BuildGlow()
    {
        GameObject glowObject = new GameObject("Storm Glow");
        glowObject.transform.SetParent(transform, false);
        glow = glowObject.AddComponent<SpriteRenderer>();
        glow.sprite = GetGlowSprite();
        glow.sortingOrder = 90; // just under the bolts (100)
        glow.color = new Color(0.65f, 0.8f, 1f, 0.45f);
        glowObject.transform.localScale = Vector3.one * radius; // glow sprite is 2 units wide at scale 1
    }

    private static Sprite GetGlowSprite()
    {
        if (glowSprite != null) { return glowSprite; }

        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[size * size];
        Vector2 mid = new Vector2(size / 2f, size / 2f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), mid) / (size / 2f);
                float alpha = Mathf.Clamp01(1f - distance);
                alpha *= alpha; // soft radial falloff
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
        glowSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size / 2f);
        return glowSprite;
    }

    private IEnumerator ScreenFlashRoutine()
    {
        // brief pale-blue full screen flash when the storm erupts
        GameObject flashObject = new GameObject("Lightning Flash");
        flashObject.transform.SetParent(transform);

        Canvas canvas = flashObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;

        GameObject imageObject = new GameObject("Flash");
        imageObject.transform.SetParent(flashObject.transform, false);
        Image image = imageObject.AddComponent<Image>();
        image.color = new Color(0.75f, 0.85f, 1f, 0.35f);
        image.raycastTarget = false;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        float flashTime = 0.2f;
        float elapsed = 0f;
        while (elapsed < flashTime)
        {
            elapsed += Time.deltaTime;
            Color color = image.color;
            color.a = 0.35f * (1f - elapsed / flashTime);
            image.color = color;
            yield return null;
        }

        Destroy(flashObject);
    }
}
