using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicLaser : MonoBehaviour
{
    [SerializeField] private float laserGrownTime = 2f;

    private bool isGrowing = true;
    private float laserRange;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider2D;

    private void Awake()
    {
        // Fixed GetComponent() calls
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        LaserFaceMouse();
    }

    private void OnTriggerenter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<Indestructible>() && !other.isTrigger)
        {
            isGrowing = false;
        }
    }

    public void UpdateLaserRange(float laserRange)
    {
        this.laserRange = laserRange;
        StartCoroutine(IncreaseLaserLengthRoutine());
    }

    private IEnumerator IncreaseLaserLengthRoutine()
    {
        float timePassed = 0f;

        // Compare the x component of spriteRenderer.size to laserRange
        while (spriteRenderer.size.x < laserRange && isGrowing)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / laserGrownTime;

            // Sprite resizing
            spriteRenderer.size = new Vector2(Mathf.Lerp(1f, laserRange, linearT), 1f);

            // Collider resizing
            capsuleCollider2D.size = new Vector2(Mathf.Lerp(1f, laserRange, linearT), capsuleCollider2D.size.y);
            capsuleCollider2D.offset = new Vector2(Mathf.Lerp(1f, laserRange, linearT) / 2, capsuleCollider2D.offset.y);

            yield return null;
        }
    // Coroutine for fading, moved inside a method
    StartCoroutine(GetComponent<SpriteFade>().SlowFadeRoutine());
    }

    private void LaserFaceMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 direction = transform.position - mousePosition;
        transform.right = -direction;
    }
}
