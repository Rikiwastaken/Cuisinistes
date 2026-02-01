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

    Image lightImage;
    Vector2 lightStartPos;
    Color lightStartColor;

    Coroutine mainLoopCoroutine;

    void Awake()
    {
        lightImage = lightCircle.GetComponent<Image>();
        lightStartPos = lightCircle.anchoredPosition;
        lightStartColor = lightImage.color;
    }

    void OnEnable()
    {
        ResetState();
        mainLoopCoroutine = StartCoroutine(MainLoop());
    }

    void OnDisable()
    {
        if (mainLoopCoroutine != null)
            StopCoroutine(mainLoopCoroutine);
    }

    IEnumerator MainLoop()
    {
        while (true)
        {
            while (Vector2.Distance(lightCircle.anchoredPosition, stopPoint.anchoredPosition) > 1f)
            {
                lightCircle.anchoredPosition = Vector2.MoveTowards(
                    lightCircle.anchoredPosition,
                    stopPoint.anchoredPosition,
                    speed * Time.unscaledDeltaTime
                );
                yield return null;
            }

            yield return FadeInImage();
            yield return ShakeLight();
            yield return FadeOutLight();

            yield return new WaitForSecondsRealtime(imageHoldTime);
            yield return FadeOutImage();

            yield return new WaitForSecondsRealtime(loopDelay);

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
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / imageFadeInDuration);
            targetImage.color = c;
            yield return null;
        }
    }

    IEnumerator FadeOutImage()
    {
        float t = 0f;
        Color c = targetImage.color;

        while (t < imageFadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / imageFadeOutDuration);
            targetImage.color = c;
            yield return null;
        }
    }

    IEnumerator ShakeLight()
    {
        float elapsed = 0f;
        Vector2 basePos = lightCircle.anchoredPosition;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            lightCircle.anchoredPosition = basePos + Random.insideUnitCircle * shakeStrength;
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
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(lightStartColor.a, 0f, t / lightFadeOutDuration);
            lightImage.color = c;
            yield return null;
        }
    }
}