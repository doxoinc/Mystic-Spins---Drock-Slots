// DailyRewardBox.cs
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DailyRewardBox : MonoBehaviour
{
    [Header("UI Элементы")]
    public Button viewBoxButton; // Кнопка "View Box"
    public Text timerText; // Текст для отображения таймера под ящиком
    public Image boxImage; // Изображение коробки

    [Header("Настройки Коробки")]
    public string boxId; // Уникальный идентификатор коробки ("Red", "Green", "Blue")
    public Sprite closedBoxSprite; // Спрайт закрытой коробки
    public Sprite openedBoxSprite; // Спрайт открытой коробки

    private Coroutine timerCoroutine;

    private void Start()
    {
        // Добавляем слушатель на кнопку "View Box"
        if (viewBoxButton != null)
        {
            viewBoxButton.onClick.AddListener(OnViewBoxClicked);
        }

        // Обновляем состояние кнопки и изображения при старте
        UpdateButtonState(GameData.Instance.PlayerCoins.CoinCount);
    }

    private void OnEnable()
    {
        // Подписываемся на событие изменения монет
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged += UpdateButtonState;
        }
    }

    private void OnDisable()
    {
        // Отписываемся от события изменения монет
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged -= UpdateButtonState;
        }

        // Останавливаем таймер, если он запущен
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    /// <summary>
    /// Обработчик нажатия на кнопку "View Box".
    /// </summary>
    private void OnViewBoxClicked()
    {
        if (DailyRewardManager.Instance != null)
        {
            DailyRewardManager.Instance.OpenBoxPanel(boxId);
        }
        else
        {
            Debug.LogError("DailyRewardManager.Instance не установлен.");
        }
    }

    /// <summary>
    /// Обновляет состояние кнопки, таймера и изображения коробки.
    /// Этот метод соответствует делегату CoinManager.OnCoinsChanged.
    /// </summary>
    /// <param name="newCoinCount">Новое количество монет.</param>
    public void UpdateButtonState(int newCoinCount)
    {
        DateTime lastOpenTime = GameData.Instance.GetLastOpenTime(boxId);
        DateTime now = DateTime.UtcNow;
        TimeSpan timeSinceLastOpen = now - lastOpenTime;

        if (timeSinceLastOpen >= TimeSpan.FromDays(1))
        {
            // Коробка доступна для открытия
            viewBoxButton.interactable = true;
            timerText.text = "";

            // Устанавливаем спрайт закрытой коробки
            if (boxImage != null && closedBoxSprite != null)
            {
                boxImage.sprite = closedBoxSprite;
            }
        }
        else
        {
            // Коробка на перезарядке
            viewBoxButton.interactable = false;
            TimeSpan remaining = TimeSpan.FromDays(1) - timeSinceLastOpen;

            // Устанавливаем спрайт открытой коробки
            if (boxImage != null && openedBoxSprite != null)
            {
                boxImage.sprite = openedBoxSprite;
            }

            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            timerCoroutine = StartCoroutine(UpdateTimer(remaining));
        }
    }

    /// <summary>
    /// Coroutine для обновления таймера каждую секунду.
    /// </summary>
    /// <param name="remaining">Оставшееся время.</param>
    /// <returns></returns>
    private IEnumerator UpdateTimer(TimeSpan remaining)
    {
        while (remaining.TotalSeconds > 0)
        {
            timerText.text = FormatTimeSpan(remaining);
            yield return new WaitForSeconds(1f);
            remaining = remaining.Subtract(TimeSpan.FromSeconds(1));
        }

        // Таймер завершен, обновляем состояние кнопки и изображения
        UpdateButtonState(GameData.Instance.PlayerCoins.CoinCount);
    }

    /// <summary>
    /// Форматирует TimeSpan в строку формата "HH:MM:SS".
    /// </summary>
    /// <param name="timeSpan">TimeSpan для форматирования.</param>
    /// <returns>Форматированная строка.</returns>
    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
                             timeSpan.Hours,
                             timeSpan.Minutes,
                             timeSpan.Seconds);
    }
}
