// Bucket.cs
using UnityEngine;
using System.Collections;

public class Bucket : MonoBehaviour
{
    public float multiplier; // Множитель для выигрыша

    [Header("Bucket Animation Settings")]
    public float moveDistance = 0.2f; // Расстояние, на которое ящик будет двигаться вниз
    public float moveDuration = 0.1f; // Время (в секундах) для движения вниз или вверх

    [Header("Fade Text Settings")]
    public GameObject fadeTextPrefab; // Префаб FadeText
    public Canvas uiCanvas; // Ссылка на Canvas

    private Vector3 originalPosition; // Исходная позиция ящика
    private bool isMoving = false;    // Флаг, указывающий, движется ли ящик сейчас

    private void Start()
    {
        // Сохраняем исходную позицию ящика при старте
        originalPosition = transform.position;

        // Проверяем, установлена ли ссылка на Canvas
        if (uiCanvas == null)
        {
            uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas == null)
            {
                Debug.LogError("Не найден объект Canvas в сцене. Пожалуйста, добавьте Canvas.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            // Проверяем, был ли шарик уже собран
            if (ball.Collect())
            {
                float winnings = ball.GetBet() * multiplier;
                CoinManager.Instance.AddCoins(Mathf.RoundToInt(winnings)); // Добавляем выигрыш в коины

                // Информируем BallSpawner о выигрышах
                if (BallSpawner.Instance != null)
                {
                    BallSpawner.Instance.AddToWinnings(winnings);
                }
                else
                {
                    Debug.LogError("BallSpawner не найден в сцене.");
                }

                Destroy(ball.gameObject); // Удаляем шарик после попадания в ящик

                // Запускаем анимацию только если ящик сейчас не движется
                if (!isMoving)
                {
                    StartCoroutine(MoveDownAndUp());
                }

                // Создаём временный текст
                CreateFadeText(winnings);
            }
            else
            {
                // Шарик уже был собран ранее, ничего не делаем
                Debug.Log("Шарик уже был собран ранее.");
            }
        }
    }

    private IEnumerator MoveDownAndUp()
    {
        isMoving = true; // Устанавливаем флаг, что ящик сейчас движется

        Vector3 targetPosition = originalPosition - new Vector3(0, moveDistance, 0); // Целевая позиция (на moveDistance ниже)

        float elapsed = 0f;

        // Анимация движения вниз
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition; // Устанавливаем точную целевую позицию

        // Сброс времени для анимации движения вверх
        elapsed = 0f;

        // Анимация движения вверх
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition; // Возвращаемся в исходную позицию

        isMoving = false; // Сбрасываем флаг, что ящик завершил движение
    }

    private void CreateFadeText(float winnings)
    {
        if (fadeTextPrefab != null && uiCanvas != null)
        {
            // Определяем позицию для текста (над ящиком) в мировых координатах
            Vector3 worldPosition = transform.position + new Vector3(0, 0.5f, 0); // Смещение по Y, чтобы текст был над ящиком

            // Конвертируем мировую позицию в позицию на Canvas
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            // Проверяем, назначена ли основная камера
            if (Camera.main == null)
            {
                Debug.LogError("Не назначена основная камера (Camera.main).");
                return;
            }

            // Создаём экземпляр префаба
            GameObject fadeTextInstance = Instantiate(fadeTextPrefab, uiCanvas.transform);

            // Устанавливаем позицию в Canvas
            RectTransform rectTransform = fadeTextInstance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.position = screenPosition;
            }

            // Получаем компонент FadeText и устанавливаем текст
            FadeText fadeText = fadeTextInstance.GetComponent<FadeText>();
            if (fadeText != null)
            {
                // Форматируем текст, например, "Win: 35"
                string displayText = "Win: " + Mathf.RoundToInt(winnings).ToString();
                fadeText.text.text = displayText;
            }
        }
        else
        {
            if (fadeTextPrefab == null)
                Debug.LogError("FadeTextPrefab не установлен в инспекторе Bucket.");
            if (uiCanvas == null)
                Debug.LogError("Canvas не назначен в инспекторе Bucket.");
        }
    }
}
