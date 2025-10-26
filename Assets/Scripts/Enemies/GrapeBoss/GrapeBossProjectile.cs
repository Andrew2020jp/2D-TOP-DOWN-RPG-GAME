using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapeBossProjectile : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 3f;
    [SerializeField] private GameObject grapeProjectileShadow;
    [SerializeField] private GameObject splatterPrefab;

    // NEW: A public method to be called by the script that creates this projectile.
    public void Initialize(Vector3 targetPosition)
    {
        // We moved all the logic from Start() into this method.
        GameObject grapeShadow =
        Instantiate(grapeProjectileShadow, transform.position + new Vector3(0, -0.3f, 0), Quaternion.identity);

        // We now use the 'targetPosition' that was passed in, instead of finding the player here.
        Vector3 endPosition = targetPosition;
        Vector3 grapeShadowStartPosition = grapeShadow.transform.position;

        StartCoroutine(ProjectileCurveRoutine(transform.position, endPosition));
        StartCoroutine(MoveGrapeShadowRoutine(grapeShadow, grapeShadowStartPosition, endPosition));
    }

    private IEnumerator ProjectileCurveRoutine(Vector3 startPosition, Vector3 endPosition)
    {
        float timePassed = 0f;
        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position = Vector2.Lerp(startPosition, endPosition, linearT) + new Vector2(0f, height);

            yield return null;
        }
        Instantiate(splatterPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator MoveGrapeShadowRoutine(GameObject grapeShadow, Vector3 startPosition, Vector3 endPosition)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;
            grapeShadow.transform.position = Vector2.Lerp(startPosition, endPosition, linearT);
            yield return null;
        }

        Destroy(grapeShadow);
    }
}