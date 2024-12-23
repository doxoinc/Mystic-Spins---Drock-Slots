using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    public float fadeDuration = 0.5f; // Длительность анимации прозрачности
    public float scaleDuration = 0.5f; // Длительность анимации масштаба
    public bool playOnStart = true; // Автоматически запускать анимацию при старте

    private CanvasGroup canvasGroup;
    private Vector3 initialScale;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        initialScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (playOnStart)
        {
            PlayAnimation();
        }
    }

    public void PlayAnimation()
    {
        StartCoroutine(AnimateElement());
    }

    private IEnumerator AnimateElement()
    {
        // Начинаем с невидимого и с нулевым масштабом
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.zero;
        canvasGroup.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < Mathf.Max(fadeDuration, scaleDuration))
        {
            elapsed += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(elapsed / fadeDuration);
            float scaleProgress = Mathf.Clamp01(elapsed / scaleDuration);

            canvasGroup.alpha = fadeProgress;
            transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, scaleProgress);

            yield return null;
        }

        // В конце анимации устанавливаем финальные значения
        canvasGroup.alpha = 1f;
        transform.localScale = initialScale;
    }
}