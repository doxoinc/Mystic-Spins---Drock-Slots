// LuckyWheelController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class LuckyWheelController : MonoBehaviour
{
    [Header("Wheel Settings")]
    public List<WheelSection> wheelSections; // Список секций барабана
    public float spinDuration = 5f;         // Длительность прокрутки
    public float spinSpeed = 360f;          // Начальная скорость прокрутки

    [Header("UI Elements")]
    public Button spinButton;                // Кнопка прокрутки
    public Text timerText;                   // Текст таймера

    [Header("Visualization Settings")]
    [SerializeField] private bool enableVisualization = true; // Галочка для включения/отключения визуализации
    [SerializeField] private Color borderColor = Color.white; // Цвет границ секций
    [SerializeField] private float borderWidth = 2f; // Ширина линий границ
    [SerializeField] private Color highlightColor = Color.red; // Цвет для выделения выигрышной секции

    private List<LineRenderer> borderLines = new List<LineRenderer>();
    private bool isSpinning = false;
    private float targetAngle;
    private int selectedSectionIndex = 0;

    void Start()
    {
        // Проверка наличия необходимых компонентов
        if (spinButton == null)
        {
            Debug.LogError("spinButton не назначен в инспекторе!");
            return;
        }

        if (timerText == null)
        {
            Debug.LogError("timerText не назначен в инспекторе!");
            return;
        }

        if (GameData.Instance == null)
        {
            Debug.LogError("GameData.Instance не найден!");
            return;
        }

        if (CoinManager.Instance == null)
        {
            Debug.LogError("CoinManager.Instance не найден!");
            return;
        }

        // Подписываемся на событие нажатия кнопки
        spinButton.onClick.AddListener(SpinWheel);

        // Обновляем состояние кнопки при запуске
        UpdateButtonState();

        // Запускаем повторяющийся вызов метода UpdateTimer каждую секунду
        InvokeRepeating("UpdateTimer", 0f, 1f);

        // Рисуем границы секций, если визуализация включена
        if (enableVisualization)
        {
            DrawSectionBorders();
        }
    }

    /// <summary>
    /// Метод, вызываемый при нажатии кнопки Spin.
    /// </summary>
    public void SpinWheel()
    {
        if (isSpinning) return;

        isSpinning = true;

        // Отключаем кнопку прокрутки
        spinButton.gameObject.SetActive(false);
        Debug.Log("Кнопка Spin отключена.");

        // Определяем случайную секцию с учётом весов
        selectedSectionIndex = GetRandomSectionIndex();
        Debug.Log("Selected Section Index: " + selectedSectionIndex);
        Debug.Log("Selected Section: " + wheelSections[selectedSectionIndex].itemName);

        // Вычисляем целевой угол для остановки на выбранной секции
        float sectionAngle = 360f / wheelSections.Count;
        float randomSpin = UnityEngine.Random.Range(5, 10) * 360f; // Дополнительные обороты
        targetAngle = (360f - (selectedSectionIndex * sectionAngle)) + randomSpin;

        StartCoroutine(AnimateSpin());
    }

    /// <summary>
    /// Анимация прокрутки колеса.
    /// </summary>
    /// <returns></returns>
    System.Collections.IEnumerator AnimateSpin()
    {
        float elapsed = 0f;
        float startAngle = transform.eulerAngles.z;
        float endAngle = startAngle + targetAngle;

        while (elapsed < spinDuration)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, elapsed / spinDuration);
            transform.eulerAngles = new Vector3(0, 0, angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Устанавливаем конечный угол колеса
        transform.eulerAngles = new Vector3(0, 0, endAngle % 360f);
        isSpinning = false;

        Debug.Log("Прокрутка завершена. Конечный угол: " + (endAngle % 360f));

        // Награждаем игрока
        RewardPlayer();

        // Сохраняем время последней прокрутки в GameData
        GameData.Instance.SetLastSpinTime(DateTime.UtcNow);
        Debug.Log("Время последней прокрутки сохранено: " + GameData.Instance.GetLastSpinTime());

        // Обновляем состояние кнопки
        UpdateButtonState();

        // Выделяем выигрышную секцию
        if (enableVisualization)
        {
            HighlightSelectedSection();
            // Логирование выигрышной секции
            Debug.Log("Выигрышная секция: " + selectedSectionIndex + " (" + wheelSections[selectedSectionIndex].itemName + ")");
        }
    }

    /// <summary>
    /// Получает случайный индекс секции с учётом весов.
    /// </summary>
    /// <returns>Индекс выбранной секции.</returns>
    int GetRandomSectionIndex()
    {
        int totalWeight = 0;
        foreach (var section in wheelSections)
        {
            totalWeight += section.weight;
        }

        int randomWeight = UnityEngine.Random.Range(1, totalWeight + 1);
        int currentWeight = 0;

        for (int i = 0; i < wheelSections.Count; i++)
        {
            currentWeight += wheelSections[i].weight;
            if (randomWeight <= currentWeight)
            {
                return i;
            }
        }

        return 0; // По умолчанию
    }

    /// <summary>
    /// Награждает игрока монетами.
    /// </summary>
    void RewardPlayer()
    {
        int coinsEarned = wheelSections[selectedSectionIndex].coins;
        CoinManager.Instance.AddCoins(coinsEarned);
        Debug.Log("Вы получили " + coinsEarned + " монет!");
    }

    /// <summary>
    /// Обновляет состояние кнопки прокрутки на основе времени последней прокрутки.
    /// </summary>
    void UpdateButtonState()
    {
        DateTime lastSpinTime = GameData.Instance.GetLastSpinTime();
        if (lastSpinTime == DateTime.MinValue)
        {
            // Если прокрутка никогда не была выполнена
            spinButton.gameObject.SetActive(true);
            timerText.gameObject.SetActive(false);
            Debug.Log("Кнопка Spin активна, таймер скрыт.");
        }
        else
        {
            DateTime nextSpinTime = lastSpinTime.AddDays(1);
            DateTime currentTime = DateTime.UtcNow;

            if (currentTime >= nextSpinTime)
            {
                // Если прошло больше суток с последней прокрутки
                spinButton.gameObject.SetActive(true);
                timerText.gameObject.SetActive(false);
                Debug.Log("Кнопка Spin активна (прошло >1 дня), таймер скрыт.");
            }
            else
            {
                // Если ещё не прошло суток
                spinButton.gameObject.SetActive(false);
                timerText.gameObject.SetActive(true);
                Debug.Log("Кнопка Spin отключена, таймер отображается.");
            }
        }
    }

    /// <summary>
    /// Обновляет текст таймера.
    /// </summary>
    void UpdateTimer()
    {
        DateTime lastSpinTime = GameData.Instance.GetLastSpinTime();
        if (lastSpinTime == DateTime.MinValue)
        {
            // Если прокрутка никогда не была выполнена
            timerText.text = "";
            Debug.Log("TimerText: Прокрутка никогда не выполнялась.");
            return;
        }

        DateTime nextSpinTime = lastSpinTime.AddDays(1);
        DateTime currentTime = DateTime.UtcNow;

        if (currentTime >= nextSpinTime)
        {
            // Если прошло больше суток с последней прокрутки
            timerText.text = "";
            Debug.Log("TimerText: Прошло больше суток, таймер скрыт.");
            return;
        }

        TimeSpan timeRemaining = nextSpinTime - currentTime;
        string formattedTime = string.Format("{0:00}:{1:00}:{2:00}",
            (int)timeRemaining.TotalHours,
            timeRemaining.Minutes,
            timeRemaining.Seconds);

        timerText.text = formattedTime;
        Debug.Log("TimerText обновлён: " + formattedTime);
    }

    /// <summary>
    /// Рисует границы секций барабана.
    /// </summary>
    private void DrawSectionBorders()
    {
        if (wheelSections == null || wheelSections.Count == 0)
        {
            Debug.LogWarning("wheelSections не назначен или пустой!");
            return;
        }

        float anglePerSection = 360f / wheelSections.Count;
        RectTransform rt = GetComponent<RectTransform>();

        if (rt == null)
        {
            Debug.LogError("RectTransform отсутствует на объекте барабана!");
            return;
        }

        // Используем минимальное значение из ширины и высоты для радиуса
        float radius = Mathf.Min(rt.sizeDelta.x, rt.sizeDelta.y) / 2f;

        for (int i = 0; i < wheelSections.Count; i++)
        {
            float angle = i * anglePerSection; // Начинаем с 0 градусов (право)
            Vector3 startPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0);
            Vector3 endPoint = Vector3.zero; // Центр барабана

            GameObject lineObj = new GameObject("BorderLine_" + i);
            lineObj.transform.SetParent(this.transform);
            lineObj.transform.localPosition = Vector3.zero;

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, endPoint);
            lr.SetPosition(1, startPoint);

            lr.startWidth = borderWidth;
            lr.endWidth = borderWidth;
            lr.useWorldSpace = false;

            // Создаём простой материал для линий
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = borderColor;
            lr.endColor = borderColor;

            lr.alignment = LineAlignment.TransformZ;

            borderLines.Add(lr);
        }

        Debug.Log("Границы секций нарисованы.");
    }

    /// <summary>
    /// Выделяет выигрышную секцию.
    /// </summary>
    private void HighlightSelectedSection()
    {
        // Сначала сбрасываем все границы к исходному цвету
        foreach (var lr in borderLines)
        {
            lr.startColor = borderColor;
            lr.endColor = borderColor;
        }

        // Затем выделяем границу выбранной секции
        if (selectedSectionIndex >= 0 && selectedSectionIndex < borderLines.Count)
        {
            borderLines[selectedSectionIndex].startColor = highlightColor;
            borderLines[selectedSectionIndex].endColor = highlightColor;
            Debug.Log("Выигрышная секция выделена: " + selectedSectionIndex + " (" + wheelSections[selectedSectionIndex].itemName + ")");
        }
        else
        {
            Debug.LogWarning("selectedSectionIndex выходит за пределы допустимого диапазона.");
        }
    }
}
