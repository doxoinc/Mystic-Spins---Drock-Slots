using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BetSystem : MonoBehaviour
{
    [Header("UI Элементы")]
    public TextMeshProUGUI betAmountText;
    public TextMeshProUGUI playerCoinsText;
    public TextMeshProUGUI totalWinningsText;
    public Button betButton;
    public Button increaseButton;
    public Button decreaseButton;

    [Header("Настройки Ставки")]
    public int currentBet = 1;
    private int totalWinnings = 0;

    private ThimbleController thimbleController;

    void Start()
    {
        thimbleController = FindObjectOfType<ThimbleController>();
        
        if (thimbleController == null)
        {
            Debug.LogError("ThimbleController не найден в сцене!");
        }

        // Подписка на событие изменения монет
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged += OnCoinsChanged;
            UpdateCoinDisplay(); // Инициализация отображения монет
        }
        else
        {
            Debug.LogError("CoinManager.Instance не найден!");
        }

        betButton.onClick.AddListener(PlaceBet);
        increaseButton.onClick.AddListener(IncreaseBet);
        decreaseButton.onClick.AddListener(DecreaseBet);

        UpdateBetAmountText();
        UpdateTotalWinningsText();
    }

    void OnDestroy()
    {
        // Отписка от события изменения монет
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged -= OnCoinsChanged;
        }
    }

    /// <summary>
    /// Обработчик события изменения количества монет.
    /// </summary>
    /// <param name="newCoinCount">Новое количество монет.</param>
    void OnCoinsChanged(int newCoinCount)
    {
        UpdateCoinDisplay();
    }

    /// <summary>
    /// Размещает ставку, списывая монеты через CoinManager.
    /// </summary>
    void PlaceBet()
    {
        int playerCoins = CoinManager.Instance.GetCoinCount();

        if (currentBet <= playerCoins && currentBet >= 1)
        {
            bool success = CoinManager.Instance.RemoveCoins(currentBet);
            if (success)
            {
                thimbleController.startButton.interactable = true;
                betButton.interactable = false;
            }
            else
            {
                Debug.Log("Не удалось списать монеты.");
            }
        }
        else
        {
            Debug.Log("Недопустимая сумма ставки.");
        }
    }

    /// <summary>
    /// Увеличивает ставку на 1, если это возможно.
    /// </summary>
    void IncreaseBet()
    {
        int playerCoins = CoinManager.Instance.GetCoinCount();

        if (currentBet < playerCoins)
        {
            currentBet++;
            UpdateBetAmountText();
        }
    }

    /// <summary>
    /// Уменьшает ставку на 1, если это возможно.
    /// </summary>
    void DecreaseBet()
    {
        if (currentBet > 1)
        {
            currentBet--;
            UpdateBetAmountText();
        }
    }

    /// <summary>
    /// Разрешает ставку, добавляя монеты при выигрыше.
    /// </summary>
    /// <param name="hasWon">Победа или проигрыш.</param>
    public void ResolveBet(bool hasWon)
    {
        if (hasWon)
        {
            int winnings = currentBet * 2;
            CoinManager.Instance.AddCoins(winnings);
            totalWinnings += currentBet;
            Debug.Log("Вы выиграли! Монет: " + CoinManager.Instance.GetCoinCount());
        }
        else
        {
            totalWinnings -= currentBet;
            Debug.Log("Вы проиграли! Монет: " + CoinManager.Instance.GetCoinCount());
        }

        UpdateTotalWinningsText();
        currentBet = 1;
        UpdateBetAmountText();
        betButton.interactable = true;
    }

    /// <summary>
    /// Обновляет отображение количества монет игрока.
    /// </summary>
    void UpdateCoinDisplay()
    {
        if (playerCoinsText != null && CoinManager.Instance != null)
        {
            playerCoinsText.text = $"{CoinManager.Instance.GetCoinCount()}";
        }
    }

    /// <summary>
    /// Обновляет отображение текущей ставки.
    /// </summary>
    void UpdateBetAmountText()
    {
        if (betAmountText != null)
        {
            betAmountText.text = $"{currentBet}";
        }
    }

    /// <summary>
    /// Обновляет отображение общего выигрыша.
    /// </summary>
    void UpdateTotalWinningsText()
    {
        if (totalWinningsText != null)
        {
            totalWinningsText.text = $"{totalWinnings}";
        }
    }
}
