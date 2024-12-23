// FadeText.cs
using UnityEngine;
using TMPro; // Используем TextMeshPro
using System.Collections;

public class FadeText : MonoBehaviour
{
    public TextMeshProUGUI text; // Компонент TextMeshProUGUI для отображения текста
    public float fadeDuration = 1f; // Длительность фейд-аута
    public float moveDistance = 50f; // Расстояние, на которое текст поднимется (в пикселях)

    private Vector3 originalPosition; // Исходная позиция текста

    private void Start()
    {
        if (text == null)
        {
            Debug.LogError("TextMeshProUGUI не назначен в скрипте FadeText.");
            return;
        }

        // Сохраняем исходную позицию
        originalPosition = transform.position;

        // Запуск корутины для фейд-аута и подъёма текста
        StartCoroutine(FadeOutAndMoveUp());
    }

    private IEnumerator FadeOutAndMoveUp()
    {
        Color originalColor = text.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Линейное затухание альфа-канала
            float alpha = Mathf.Lerp(originalColor.a, 0, t);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // Линейное перемещение вверх
            Vector3 newPosition = originalPosition + new Vector3(0, moveDistance * t, 0);
            transform.position = Vector3.Lerp(originalPosition, newPosition, t);

            yield return null;
        }

        // Убедимся, что текст полностью прозрачный и в конечной позиции
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        transform.position = originalPosition + new Vector3(0, moveDistance, 0);

        Destroy(gameObject); // Удаление объекта после завершения анимации
    }
}
