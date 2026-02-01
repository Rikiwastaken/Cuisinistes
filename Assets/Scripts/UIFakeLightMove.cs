using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFakeLightMove : MonoBehaviour
{
    public RectTransform lightCircle;
    public RectTransform stopPoint;
    public RawImage targetImage;

    [Header("Movement")]
    public float speed = 300f;

    [Header("Image")]
    public float imageFadeInDuration = 0.5f;
    public float imageFadeOutDuration = 1.2f;

    [Header("Shake")]
    public float shakeDuration = 0.3f;
    public float shakeStrength = 15f;

    [Header("Light")]
    public float lightFadeOutDuration = 0.4f;

    [Header("Delays")]
    public float imageHoldTime = 0.4f;
    public float loopDelay = 3f;          

    bool reached = false;

    Image lightImage;
    Vector2 lightStartPos;
    Color lightStartColor;

    void Start()
    {
        lightImage = lightCircle.GetComponent<Image>();

        lightStartPos = lightCircle.anchoredPosition;
        lightStartColor = lightImage.color;

        ResetState();
        StartCoroutine(MainLoop());
    }

    IEnumerator MainLoop()
    {
        while (true)
        {
            reached = false;

            while (!reached)
            {
                lightCircle.anchoredPosition = Vector2.MoveTowards(
                    lightCircle.anchoredPosition,
                    stopPoint.anchoredPosition,
                    speed * Time.deltaTime
                );

                if (Vector2.Distance(lightCircle.anchoredPosition, stopPoint.anchoredPosition) < 1f)
                    reached = true;

                yield return null;
            }

            yield return StartCoroutine(FadeInImage());
            yield return StartCoroutine(ShakeLight());
            yield return StartCoroutine(FadeOutLight());
            yield return new WaitForSeconds(imageHoldTime);
            yield return StartCoroutine(FadeOutImage());
            yield return new WaitForSeconds(loopDelay);

            ResetState();
        }
    }

    void ResetState()
    {
        lightCircle.anchoredPosition = lightStartPos;
        lightImage.color = lightStartColor;

        Color c = targetImage.color;
        c.a = 0f;
        targetImage.color = c;
    }

    IEnumerator FadeInImage()
    {
        float t = 0f;
        Color c = targetImage.color;

        while (t < imageFadeInDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / imageFadeInDuration);
            targetImage.color = c;
            yield return null;
        }

        c.a = 1f;
        targetImage.color = c;
    }

    IEnumerator FadeOutImage()
    {
        float t = 0f;
        Color c = targetImage.color;

        while (t < imageFadeOutDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / imageFadeOutDuration);
            targetImage.color = c;
            yield return null;
        }

        c.a = 0f;
        targetImage.color = c;
    }

    IEnumerator ShakeLight()
    {
        float elapsed = 0f;
        Vector2 basePos = lightCircle.anchoredPosition;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            Vector2 offset = Random.insideUnitCircle * shakeStrength;
            lightCircle.anchoredPosition = basePos + offset;
            yield return null;
        }

        lightCircle.anchoredPosition = basePos;
    }

    IEnumerator FadeOutLight()
    {
        float t = 0f;
        Color c = lightImage.color;

        while (t < lightFadeOutDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(lightStartColor.a, 0f, t / lightFadeOutDuration);
            lightImage.color = c;
            yield return null;
        }

        c.a = 0f;
        lightImage.color = c;
    }
}