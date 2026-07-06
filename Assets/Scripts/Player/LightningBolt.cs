using UnityEngine;

// Procedural lightning bolt: a jagged, flickering line that fades out quickly.
// Built entirely from code so no prefab, sprite or material asset is needed.
public class LightningBolt : MonoBehaviour
{
    private const float LIFETIME = 0.25f;
    private const int SEGMENTS = 9;
    private const float JITTER = 0.35f;
    private const float REJITTER_INTERVAL = 0.04f;

    private LineRenderer line;
    private Vector3 from;
    private Vector3 to;
    private float age;
    private float rejitterTimer;

    public static void Create(Vector3 from, Vector3 to)
    {
        GameObject boltObject = new GameObject("Lightning Bolt");
        LightningBolt bolt = boltObject.AddComponent<LightningBolt>();
        bolt.from = from;
        bolt.to = to;
        bolt.BuildLine();
    }

    private void BuildLine()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.positionCount = SEGMENTS;
        line.startWidth = 0.09f;
        line.endWidth = 0.03f;
        line.numCapVertices = 2;
        line.sortingOrder = 100;
        line.startColor = new Color(1f, 1f, 0.7f, 1f);    // hot yellow-white core
        line.endColor = new Color(0.55f, 0.75f, 1f, 1f);  // electric blue tip
        RandomizePoints();
    }

    private void RandomizePoints()
    {
        Vector3 direction = to - from;
        Vector3 normal = new Vector3(-direction.y, direction.x, 0f).normalized;

        for (int i = 0; i < SEGMENTS; i++)
        {
            float t = i / (float)(SEGMENTS - 1);
            Vector3 point = Vector3.Lerp(from, to, t);
            if (i > 0 && i < SEGMENTS - 1)
            {
                point += normal * (Random.Range(-JITTER, JITTER) * Mathf.Sin(t * Mathf.PI));
            }
            line.SetPosition(i, point);
        }
    }

    private void Update()
    {
        age += Time.deltaTime;
        if (age >= LIFETIME)
        {
            Destroy(gameObject);
            return;
        }

        // re-roll the jagged shape a few times so the bolt flickers like electricity
        rejitterTimer += Time.deltaTime;
        if (rejitterTimer >= REJITTER_INTERVAL)
        {
            rejitterTimer = 0f;
            RandomizePoints();
        }

        float alpha = 1f - (age / LIFETIME);
        Color startColor = line.startColor;
        Color endColor = line.endColor;
        startColor.a = alpha;
        endColor.a = alpha;
        line.startColor = startColor;
        line.endColor = endColor;
    }
}
