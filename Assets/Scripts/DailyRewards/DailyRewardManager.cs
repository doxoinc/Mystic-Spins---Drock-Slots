// DailyRewardManager.cs
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DailyRewardManager : MonoBehaviour
{
    public static DailyRewardManager Instance { get; private set; }

    [Header("Панель Подтверждения Открытия Коробки")]
    public GameObject confirmationPanel; // Панель подтверждения открытия коробки
    public Button openBoxButton; // Кнопка "OpenBox" внутри панели подтверждения
    public Image confirmationBoxImage; // Image "BoxImage" внутри панели подтверждения
    public Text rewardDescriptionText; // Текст для вывода выигранных монет

    [Header("Кнопка Back")]
    public Button backButton; // Кнопка "Back" на экране

    [Header("Настройки наград")]
    [SerializeField]
    private List<BoxConfig> boxConfigs; // Список конфигураций коробок

    private Dictionary<string, Sprite> boxSprites; // Словарь для быстрого доступа к спрайтам по boxId
    private string currentBoxId; // Текущий выбранный boxId
    private bool isOnCooldown = false;
    private DateTime nextAvailableTime;

    [Header("Анимация")]
    public Animator panelAnimator; // Animator для панели подтверждения
    public string openAnimationTrigger = "Open"; // Триггер для открытия панели
    public string closeAnimationTrigger = "Close"; // Триггер для закрытия панели

    [System.Serializable]
    public class BoxConfig
    {
        public string boxId; // "Red", "Green", "Blue"
        public Sprite boxSprite; // Спрайт для панели подтверждения
        public Sprite openedBoxSprite; // Спрайт открытой коробки
    }

    private void Awake()
    {
        // Реализация паттерна Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManager()
    {
        // Инициализируем словарь boxSprites
        boxSprites = new Dictionary<string, Sprite>();
        foreach (var config in boxConfigs)
        {
            if (!boxSprites.ContainsKey(config.boxId))
            {
                boxSprites.Add(config.boxId, config.boxSprite);
            }
            else
            {
                Debug.LogWarning($"Duplicate boxId {config.boxId} в BoxConfig.");
            }
        }

        // Инициализируем кнопку "OpenBox"
        if (openBoxButton != null)
        {
            openBoxButton.onClick.AddListener(OnOpenBoxClicked);
        }

        // Закрываем панель подтверждения по умолчанию
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        // Инициализируем текст награды
        if (rewardDescriptionText != null)
        {
            rewardDescriptionText.text = "";
        }

        // Инициализируем кнопку "Back"
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    /// <summary>
    /// Открывает панель подтверждения для выбранной коробки.
    /// </summary>
    /// <param name="boxId">Идентификатор коробки.</param>
    public void OpenBoxPanel(string boxId)
    {
        currentBoxId = boxId;

        // Устанавливаем спрайт коробки на панели подтверждения
        if (confirmationBoxImage != null && boxSprites.ContainsKey(boxId))
        {
            confirmationBoxImage.sprite = boxSprites[boxId];
        }
        else
        {
            Debug.LogWarning($"Спрайт для коробки {boxId} не найден.");
        }

        // Обновляем состояние кнопки в зависимости от времени последнего открытия
        UpdateCooldown();

        // Показываем панель подтверждения
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            if (panelAnimator != null)
            {
                panelAnimator.SetTrigger(openAnimationTrigger);
            }
        }

        // Отключаем кнопку "Back" во время анимации
        if (backButton != null)
        {
            backButton.interactable = false;
        }
    }

    /// <summary>
    /// Обработчик нажатия на кнопку "OpenBox".
    /// </summary>
    private void OnOpenBoxClicked()
    {
        if (string.IsNullOrEmpty(currentBoxId))
        {
            Debug.LogError("Текущий boxId не установлен.");
            return;
        }

        if (isOnCooldown)
        {
            Debug.Log("Коробка на перезарядке.");
            return;
        }

        // Получаем случайное количество монет
        int coinsWon = GetRandomCoinAmount();

        // Добавляем монеты через CoinManager
        CoinManager.Instance.AddCoins(coinsWon);
        Debug.Log($"Вы получили {coinsWon} монет из коробки {currentBoxId}.");

        // Обновляем текст награды (только число)
        if (rewardDescriptionText != null)
        {
            rewardDescriptionText.text = coinsWon.ToString();
        }

        // Устанавливаем время последнего открытия
        DateTime now = DateTime.UtcNow;
        GameData.Instance.SetLastOpenTime(currentBoxId, now);

        // Устанавливаем следующий доступный раз
        nextAvailableTime = now.AddDays(1);

        // Запускаем анимацию закрытия панели подтверждения
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger(closeAnimationTrigger);
        }

        // Отключаем кнопку "Back" во время анимации
        if (backButton != null)
        {
            backButton.interactable = false;
        }

        // Изменяем изображение коробки на открытую в панели подтверждения
        if (confirmationBoxImage != null && boxConfigs != null)
        {
            foreach (var config in boxConfigs)
            {
                if (config.boxId == currentBoxId && config.openedBoxSprite != null)
                {
                    confirmationBoxImage.sprite = config.openedBoxSprite;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Метод, вызываемый ConfirmationPanelController после завершения анимации закрытия.
    /// </summary>
    public void OnConfirmationPanelCloseComplete()
    {
        Debug.Log("Анимация закрытия панели подтверждения завершена.");

        // Скрываем панель
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        // Очистка текста награды
        if (rewardDescriptionText != null)
        {
            rewardDescriptionText.text = "";
        }

        // Включаем кнопку "Back" после завершения анимации
        if (backButton != null)
        {
            backButton.interactable = true;
        }

        // Обновляем состояние всех ящиков (например, обновляем таймеры)
        UpdateAllBoxes();

        // Сохраняем данные после закрытия панели
        GameData.Instance.SaveData();
    }

    /// <summary>
    /// Обработчик нажатия на кнопку "Back".
    /// </summary>
    private void OnBackButtonClicked()
    {
        // Действия при нажатии на кнопку "Back"
        // Например, закрыть панель подтверждения
        if (confirmationPanel != null && confirmationPanel.activeSelf)
        {
            panelAnimator.SetTrigger(closeAnimationTrigger);
            backButton.interactable = false;
        }
    }

    /// <summary>
    /// Обновляет состояние кнопок всех ящиков.
    /// </summary>
    private void UpdateAllBoxes()
    {
        // Предполагается, что у вас есть ссылки на все экземпляры DailyRewardBox
        // Например, через массив или список
        DailyRewardBox[] allBoxes = FindObjectsOfType<DailyRewardBox>();
        foreach (var box in allBoxes)
        {
            box.UpdateButtonState(GameData.Instance.PlayerCoins.CoinCount);
        }
    }

    /// <summary>
    /// Обновляет состояние кнопки в зависимости от времени последнего открытия.
    /// </summary>
    private void UpdateCooldown()
    {
        DateTime lastOpenTime = GameData.Instance.GetLastOpenTime(currentBoxId);
        DateTime now = DateTime.UtcNow;

        TimeSpan timeSinceLastOpen = now - lastOpenTime;

        if (timeSinceLastOpen >= TimeSpan.FromDays(1))
        {
            // Коробка доступна для открытия
            isOnCooldown = false;
            if (openBoxButton != null)
            {
                openBoxButton.interactable = true;
            }
        }
        else
        {
            // Коробка на перезарядке
            isOnCooldown = true;
            if (openBoxButton != null)
            {
                openBoxButton.interactable = false;
            }

            // Запускаем таймер для отключения кнопки после завершения перезарядки
            TimeSpan remaining = TimeSpan.FromDays(1) - timeSinceLastOpen;
            StartCoroutine(UpdateTimer(remaining));
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
            yield return new WaitForSeconds(1f);
            remaining = remaining.Subtract(TimeSpan.FromSeconds(1));

            // Можно добавить обновление UI таймера здесь, если необходимо
        }

        // Перезарядка завершена
        isOnCooldown = false;
        if (openBoxButton != null)
        {
            openBoxButton.interactable = true;
        }

        // Обновляем состояние всех ящиков
        UpdateAllBoxes();

        // Сохраняем данные после завершения перезарядки
        GameData.Instance.SaveData();
    }

    /// <summary>
    /// Метод для получения случайного количества монет в пределах диапазона.
    /// </summary>
    /// <returns>Случайное количество монет между 99 и 2999.</returns>
    public int GetRandomCoinAmount()
    {
        return UnityEngine.Random.Range(99, 3000); // Верхняя граница исключается, поэтому 3000
    }
}
