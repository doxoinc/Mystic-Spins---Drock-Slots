// BallSpawner.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BallSpawner : MonoBehaviour
{
    public static BallSpawner Instance { get; private set; }

    public GameObject ballPrefab;
    public float initialDelay = 1f;
    public float spawnInterval = 1.5f;
    public BoxCollider2D spawnZone;
    public Transform ballParent;

    public Button spawnButton;
    public Button spawnOnceButton; // Новая кнопка для однократного спавна
    public Text ballQuantityText; // Displays the quantity of balls
    public Text winningsText;    // Displays the чистый выигрыш
    public Text betPerBallText;  // Displays the bet per ball
    public Button incrementButton;   // + Button for ball quantity
    public Button decrementButton;   // - Button for ball quantity
    public Button incrementBetButton; // + Button for bet per ball
    public Button decrementBetButton; // - Button for bet per ball
    public GameObject bucket;        // Reference to the bucket GameObject
    public bool autoSpawn = true;

    private int ballQuantity = 1; // Default number of balls to spawn
    private const int maxBalls = 10; // Maximum number of balls
    private int betPerBall = 10; // Default bet amount per ball
    private const int maxBet = 100; // Maximum bet per ball
    private const int minBet = 1;   // Minimum bet per ball

    private int totalSpent = 0; // Общая сумма ставок
    private int totalWon = 0;    // Общая сумма выигрышей
    private int netWinnings = 0; // Чистый выигрыш: totalWon - totalSpent

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем объект при смене сцен
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("Ball prefab is not assigned!");
            return;
        }

        spawnZone = GetComponent<BoxCollider2D>();
        if (spawnZone == null)
        {
            Debug.LogError("BoxCollider2D is missing on BallSpawner!");
            return;
        }

        // Настройка слушателей кнопок
        if (spawnButton != null)
            spawnButton.onClick.AddListener(ToggleSpawning);

        if (spawnOnceButton != null)
            spawnOnceButton.onClick.AddListener(SpawnOnce);

        if (incrementButton != null)
            incrementButton.onClick.AddListener(IncrementBallQuantity);

        if (decrementButton != null)
            decrementButton.onClick.AddListener(DecrementBallQuantity);

        if (incrementBetButton != null)
            incrementBetButton.onClick.AddListener(IncrementBet);

        if (decrementBetButton != null)
            decrementBetButton.onClick.AddListener(DecrementBet);

        UpdateBallQuantityText(); // Инициализация текста количества шариков
        UpdateWinnings();         // Инициализация текста чистого выигрыша
        UpdateBetPerBallText();   // Инициализация текста ставки за шарик

        // Запускаем автоспавн, если он включён
        if (autoSpawn)
            InvokeRepeating(nameof(SpawnBall), initialDelay, spawnInterval);
    }

    void SpawnBall()
    {
        if (!autoSpawn || spawnZone == null)
        {
            return;
        }
        else
        {
            int totalBet = ballQuantity * betPerBall;

            // Проверяем, достаточно ли монет
            if (CoinManager.Instance.GetCoinCount() >= totalBet)
            {
                // Списываем монеты
                bool success = CoinManager.Instance.RemoveCoins(totalBet);
                if (success)
                {
                    totalSpent += totalBet; // Увеличиваем общую сумму ставок
                    UpdateWinnings(); // Обновляем чистый выигрыш

                    for (int i = 0; i < ballQuantity; i++)
                    {
                        Vector2 spawnPosition = GetRandomPointInZone();
                        GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

                        if (ballParent != null)
                            ball.transform.SetParent(ballParent);

                        var ballScript = ball.GetComponent<Ball>();
                        if (ballScript != null) 
                            ballScript.SetBet(betPerBall); // Устанавливаем ставку за шарик
                    }
                }
                else
                {
                    Debug.LogWarning("Не удалось списать монеты для автоспавна.");
                    // Останавливаем автоспавн, если монет недостаточно
                    autoSpawn = false;
                    if (spawnButton != null)
                    {
                        Text buttonTextComponent = spawnButton.GetComponentInChildren<Text>();
                        if (buttonTextComponent != null)
                            buttonTextComponent.text = "Start Spawning";
                    }
                }
            }
            else
            {
                Debug.LogWarning("Недостаточно монет для автоспавна.");
                // Останавливаем автоспавн, если монет недостаточно
                autoSpawn = false;
                if (spawnButton != null)
                {
                    Text buttonTextComponent = spawnButton.GetComponentInChildren<Text>();
                    if (buttonTextComponent != null)
                        buttonTextComponent.text = "Start Spawning";
                }
            }
        }
    }

    public void OnlyOneTimeSpawn()
    {
        for (int i = 0; i < ballQuantity; i++)
        {
            Vector2 spawnPosition = GetRandomPointInZone();
            GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

            if (ballParent != null)
                ball.transform.SetParent(ballParent);

            var ballScript = ball.GetComponent<Ball>();
            if (ballScript != null)
                ballScript.SetBet(betPerBall); // Устанавливаем ставку за шарик
        }
    }

    Vector2 GetRandomPointInZone()
    {
        if (spawnZone == null)
        {
            Debug.LogWarning("SpawnZone is null when calculating a random point.");
            return Vector2.zero;
        }

        Vector2 zoneSize = spawnZone.size;
        Vector2 zoneCenter = (Vector2)spawnZone.transform.position + spawnZone.offset;

        float x = Random.Range(zoneCenter.x - zoneSize.x / 2, zoneCenter.x + zoneSize.x / 2);
        float y = Random.Range(zoneCenter.y - zoneSize.y / 2, zoneCenter.y + zoneSize.y / 2);

        return new Vector2(x, y);
    }

    public void ToggleSpawning()
    {
        autoSpawn = !autoSpawn;
        string buttonText = autoSpawn ? "Stop Spawning" : "Start Spawning";
        if (spawnButton != null)
        {
            Text buttonTextComponent = spawnButton.GetComponentInChildren<Text>();
            if (buttonTextComponent != null)
                buttonTextComponent.text = buttonText;
        }

        if (autoSpawn)
        {
            InvokeRepeating(nameof(SpawnBall), initialDelay, spawnInterval);
        }
        else
        {
            CancelInvoke(nameof(SpawnBall));
        }
    }

    public void SpawnOnce()
    {
        int totalBet = ballQuantity * betPerBall;

        // Проверяем, достаточно ли монет у игрока
        if (CoinManager.Instance.GetCoinCount() >= totalBet)
        {
            // Списываем монеты
            bool success = CoinManager.Instance.RemoveCoins(totalBet);
            if (success)
            {
                totalSpent += totalBet; // Увеличиваем общую сумму ставок
                UpdateWinnings(); // Обновляем чистый выигрыш

                // Спавним шарики
                OnlyOneTimeSpawn();

                // Обновляем UI монет
                // Предполагается, что CoinUI обновляет количество монет автоматически через событие CoinsChanged
            }
            else
            {
                Debug.LogWarning("Не удалось списать монеты. Возможно, монеты были списаны в другом месте.");
            }
        }
        else
        {
            Debug.LogWarning("Недостаточно монет для выполнения ставки.");
            // Здесь можно добавить отображение сообщения пользователю о недостатке монет
        }
    }

    public void GoHome()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void IncrementBallQuantity()
    {
        if (ballQuantity < maxBalls)
        {
            ballQuantity++;
            UpdateBallQuantityText();
        }
    }

    private void DecrementBallQuantity()
    {
        if (ballQuantity > 1)
        {
            ballQuantity--;
            UpdateBallQuantityText();
        }
    }

    private void IncrementBet()
    {
        if (betPerBall < maxBet)
        {
            betPerBall++;
            UpdateBetPerBallText();
        }
    }

    private void DecrementBet()
    {
        if (betPerBall > minBet)
        {
            betPerBall--;
            UpdateBetPerBallText();
        }
    }

    private void UpdateBallQuantityText()
    {
        if (ballQuantityText != null)
        {
            ballQuantityText.text = $"{ballQuantity}";
        }
    }

    private void UpdateBetPerBallText()
    {
        if (betPerBallText != null)
        {
            betPerBallText.text = $"{betPerBall}";
        }
    }

    public void AddToWinnings(float amount)
    {
        int _amount = Mathf.RoundToInt(amount);
        totalWon += _amount;
        UpdateWinnings();
    }

    private void UpdateWinnings()
    {
        netWinnings = totalWon - totalSpent;
        if (winningsText != null)
        {
            if (netWinnings > 0)
            {
                winningsText.text = $"{netWinnings}";
            }
            else
            {
                winningsText.text = "0";
            }
        }
    }

    private void Update()
    {
        // Блокировка кнопки SpawnOnce, если монет недостаточно
        if (spawnOnceButton != null)
        {
            int totalBet = ballQuantity * betPerBall;
            bool canSpawn = CoinManager.Instance.GetCoinCount() >= totalBet;
            spawnOnceButton.interactable = canSpawn;
        }
    }
}
