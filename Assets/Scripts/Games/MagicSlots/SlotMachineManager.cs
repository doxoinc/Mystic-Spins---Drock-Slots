using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    [Header("Reels")]
    public List<Reel> reels;             // Список всех барабанов (должно быть 5)

    [Header("UI Elements")]
    public Text betText;
    public Text coinsText;
    public Text sessionWinningsText;     // Новый UI элемент для отображения выигранных монет в сессии

    [Header("Buttons")]
    public Button addBetButton;
    public Button removeBetButton;
    public Button spinButton;

    [Header("Bet Settings")]
    public int betAmount = 1;            // Текущая ставка
    public int maxBetAmount = 10;        // Максимальная ставка

    [Header("Win Multipliers")]
    [SerializeField] private int verticalWinMultiplier = 10;
    [SerializeField] private int horizontalWinMultiplier = 15;
    [SerializeField] private int bigWinMultiplier = 100;

    [Header("Win Panels")]
    public GameObject verticalWinPanel;
    public GameObject horizontalWinPanel;
    public GameObject bigWinPanel;

    private Animator verticalWinAnimator;
    private Animator horizontalWinAnimator;
    private Animator bigWinAnimator;

    private bool isSpinning = false;
    private int sessionWinnings;          // Выигрыш в текущей сессии

    public enum WinType
    {
        None,
        Vertical,
        Horizontal,
        BigWin
    }

    private void Start()
    {
        // Подписываемся на событие изменения монет для обновления UI
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged += OnCoinsChanged;
            // Инициализируем UI с текущим количеством монет
            OnCoinsChanged(CoinManager.Instance.GetCoinCount());
        }
        else
        {
            Debug.LogError("CoinManager не найден в сцене.");
        }

        // Инициализируем выигрыш сессии
        sessionWinnings = 0;
        UpdateSessionWinningsUI();

        // Назначаем методы-обработчики для кнопок
        if (addBetButton != null)
            addBetButton.onClick.AddListener(AddBet);
        else
            Debug.LogError("AddBetButton не назначена в инспекторе.");

        if (removeBetButton != null)
            removeBetButton.onClick.AddListener(RemoveBet);
        else
            Debug.LogError("RemoveBetButton не назначена в инспекторе.");

        if (spinButton != null)
            spinButton.onClick.AddListener(SpinReels);
        else
            Debug.LogError("SpinButton не назначена в инспекторе.");

        // Инициализируем Animator компоненты
        if (verticalWinPanel != null)
            verticalWinAnimator = verticalWinPanel.GetComponent<Animator>();
        else
            Debug.LogError("VerticalWinPanel не назначена в SlotMachineManager.");

        if (horizontalWinPanel != null)
            horizontalWinAnimator = horizontalWinPanel.GetComponent<Animator>();
        else
            Debug.LogError("HorizontalWinPanel не назначена в SlotMachineManager.");

        if (bigWinPanel != null)
            bigWinAnimator = bigWinPanel.GetComponent<Animator>();
        else
            Debug.LogError("BigWinPanel не назначена в SlotMachineManager.");

        // Обновляем UI ставки
        UpdateUI();
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged -= OnCoinsChanged;
        }
    }

    /// <summary>
    /// Обработчик события изменения монет.
    /// </summary>
    /// <param name="newCoinCount">Новое количество монет.</param>
    private void OnCoinsChanged(int newCoinCount)
    {
        if (coinsText != null)
            coinsText.text = newCoinCount.ToString();
        else
            Debug.LogError("coinsText не назначен в SlotMachineManager.");
    }

    /// <summary>
    /// Увеличивает ставку на 1.
    /// </summary>
    private void AddBet()
    {
        Debug.Log("Нажата кнопка AddBet");
        if (CoinManager.Instance == null)
        {
            Debug.LogError("CoinManager.Instance равен null");
            return;
        }

        if (betAmount < maxBetAmount)
        {
            betAmount += 1;
            UpdateUI();
            Debug.Log("Ставка увеличена до: " + betAmount);
        }
        else
        {
            Debug.Log("Достигнуто максимальное значение ставки: " + maxBetAmount);
        }
    }

    /// <summary>
    /// Уменьшает ставку на 1.
    /// </summary>
    private void RemoveBet()
    {
        Debug.Log("Нажата кнопка RemoveBet");
        if (CoinManager.Instance == null)
        {
            Debug.LogError("CoinManager.Instance равен null");
            return;
        }

        if (betAmount > 1)
        {
            betAmount -= 1;
            UpdateUI();
            Debug.Log("Ставка уменьшена до: " + betAmount);
        }
        else
        {
            Debug.Log("Ставка не может быть меньше 1.");
        }
    }

    /// <summary>
    /// Обновляет UI ставки.
    /// </summary>
    private void UpdateUI()
    {
        if (betText != null)
            betText.text = "BET: " + betAmount;
        else
            Debug.LogError("betText не назначен в SlotMachineManager.");
    }

    /// <summary>
    /// Обновляет UI выигранных монет в сессии.
    /// </summary>
    private void UpdateSessionWinningsUI()
    {
        if (sessionWinningsText != null)
            sessionWinningsText.text = "WIN: " + sessionWinnings;
        else
            Debug.LogError("sessionWinningsText не назначен в SlotMachineManager.");
    }

    /// <summary>
    /// Запускает спин барабанов.
    /// </summary>
    private void SpinReels()
    {
        if (isSpinning)
        {
            Debug.Log("Вращение уже происходит.");
            return;
        }

        if (CoinManager.Instance.GetCoinCount() >= betAmount)
        {
            // Удаляем монеты через CoinManager
            bool coinsRemoved = CoinManager.Instance.RemoveCoins(betAmount);
            if (!coinsRemoved)
            {
                Debug.Log("Не удалось снять монеты для ставки.");
                return;
            }

            ResetHighlighting(); // Сброс подсветки перед новым спином

            bool isWin = Random.Range(0f, 1f) <= 0.15f; // 15% шанс на выигрыш
            if (isWin)
            {
                // Определяем, будет ли это WIN или BIG-WIN (например, 80% WIN, 20% BIG-WIN)
                bool isBigWin = Random.Range(0f, 1f) <= 0.2f;
                StartCoroutine(SpinReelsCoroutine(isWin: true, isBigWin: isBigWin));
            }
            else
            {
                // Обычный спин без выигрыша
                StartCoroutine(SpinReelsCoroutine(isWin: false, isBigWin: false));
            }

            // Деактивируем кнопку на время вращения
            if (spinButton != null)
            {
                spinButton.interactable = false;
            }

            isSpinning = true;
        }
        else
        {
            Debug.Log("Недостаточно монет для ставки.");
        }
    }

    /// <summary>
    /// Корутина для управления спином барабанов и проверкой результатов.
    /// </summary>
    /// <param name="isWin">Является ли спин выигрышным.</param>
    /// <param name="isBigWin">Является ли спин BIG-WIN.</param>
    /// <returns></returns>
    private IEnumerator SpinReelsCoroutine(bool isWin, bool isBigWin)
    {
        // Запускаем вращение всех барабанов
        foreach (var reel in reels)
        {
            reel.Spin();
        }

        // Ждём окончания вращения всех барабанов
        bool spinning = true;
        while (spinning)
        {
            spinning = false;
            foreach (var reel in reels)
            {
                if (reel.IsSpinning)
                {
                    spinning = true;
                    break;
                }
            }
            yield return null;
        }

        // Применяем принудительные выигрыши после вращения
        if (isWin)
        {
            if (isBigWin)
            {
                ForceBigWin();
            }
            else
            {
                ForceWin();
            }
        }

        // Получаем результаты
        List<List<int>> allSymbols = new List<List<int>>();
        foreach (var reel in reels)
        {
            List<int> symbols = reel.GetCurrentSymbols();
            allSymbols.Add(symbols);

            // Выводим символы в консоль для отладки
            foreach (var symbolID in symbols)
            {
                Debug.Log("Reel Symbol ID: " + symbolID);
            }
        }

        // Проверка на выигрыш
        WinType winType = CheckWin(allSymbols);
        int winAmount = 0;

        switch (winType)
        {
            case WinType.Vertical:
                winAmount = betAmount * verticalWinMultiplier;
                CoinManager.Instance.AddCoins(winAmount);
                sessionWinnings += winAmount;
                Debug.Log($"<color=green>VERTICAL WIN! +{winAmount} монет</color>");
                ShowVerticalWinPanel(); // Показываем панель
                break;
            case WinType.Horizontal:
                winAmount = betAmount * horizontalWinMultiplier;
                CoinManager.Instance.AddCoins(winAmount);
                sessionWinnings += winAmount;
                Debug.Log($"<color=green>HORIZONTAL WIN! +{winAmount} монет</color>");
                ShowHorizontalWinPanel(); // Показываем панель
                break;
            case WinType.BigWin:
                winAmount = betAmount * bigWinMultiplier;
                CoinManager.Instance.AddCoins(winAmount);
                sessionWinnings += winAmount;
                Debug.Log($"<color=green>BIG WIN! +{winAmount} монет</color>");
                ShowBigWinPanel(); // Показываем панель
                break;
            case WinType.None:
            default:
                Debug.Log("<color=red>Нет выигрыша.</color>");
                break;
        }

        UpdateSessionWinningsUI();

        // Повторно активируем кнопку после завершения спина
        if (spinButton != null)
        {
            spinButton.interactable = true;
        }

        isSpinning = false;
    }

    /// <summary>
    /// Возвращает тип выигрыша.
    /// </summary>
    /// <param name="allSymbols">Все символы на барабанах.</param>
    /// <returns>Тип выигрыша: None, Vertical, Horizontal, BigWin</returns>
    private WinType CheckWin(List<List<int>> allSymbols)
    {
        bool horizontalWin = false;
        bool verticalWin = false;
        List<int> winningRows = new List<int>();
        List<int> winningReels = new List<int>();

        // Проверка горизонталей
        for (int rowIndex = 0; rowIndex < allSymbols[0].Count; rowIndex++)
        {
            bool allSame = true;
            int firstSymbol = allSymbols[0][rowIndex];
            for (int reelIndex = 1; reelIndex < allSymbols.Count; reelIndex++)
            {
                if (allSymbols[reelIndex][rowIndex] != firstSymbol)
                {
                    allSame = false;
                    break;
                }
            }
            if (allSame)
            {
                horizontalWin = true;
                winningRows.Add(rowIndex);
            }
        }

        // Проверка вертикалей
        for (int reelIndex = 0; reelIndex < allSymbols.Count; reelIndex++)
        {
            bool allSame = true;
            int firstSymbol = allSymbols[reelIndex][0];
            for (int symbolIndex = 1; symbolIndex < allSymbols[reelIndex].Count; symbolIndex++)
            {
                if (allSymbols[reelIndex][symbolIndex] != firstSymbol)
                {
                    allSame = false;
                    break;
                }
            }
            if (allSame)
            {
                verticalWin = true;
                winningReels.Add(reelIndex);
            }
        }

        // Логирование успешных комбинаций
        foreach (int row in winningRows)
        {
            Debug.Log($"<color=blue>Горизонтальный WIN в ряду {row + 1}! symbolID = {allSymbols[0][row]}</color>");
        }

        foreach (int reel in winningReels)
        {
            Debug.Log($"<color=blue>Вертикальный WIN в Reel {reel + 1}! symbolID = {allSymbols[reel][0]}</color>");
        }

        // Выделение выигрышных символов
        HighlightWinningSymbols(allSymbols, winningRows, winningReels);

        if (horizontalWin && verticalWin)
            return WinType.BigWin;
        else if (horizontalWin)
            return WinType.Horizontal;
        else if (verticalWin)
            return WinType.Vertical;
        else
            return WinType.None;
    }

    /// <summary>
    /// Подсвечивает выигрышные символы.
    /// </summary>
    /// <param name="allSymbols">Все символы на барабанах.</param>
    /// <param name="winningRows">Список выигрышных рядов.</param>
    /// <param name="winningReels">Список выигрышных барабанов.</param>
    private void HighlightWinningSymbols(List<List<int>> allSymbols, List<int> winningRows, List<int> winningReels)
    {
        // Подсветить символы в выигрышных рядах
        foreach (int row in winningRows)
        {
            for (int reel = 0; reel < reels.Count; reel++)
            {
                reels[reel].visibleSymbols[row].Highlight(true);
            }
        }

        // Подсветить символы в выигрышных Reels
        foreach (int reelIndex in winningReels)
        {
            foreach (var symbol in reels[reelIndex].visibleSymbols)
            {
                symbol.Highlight(true);
            }
        }
    }

    /// <summary>
    /// Сбрасывает подсветку всех символов.
    /// </summary>
    private void ResetHighlighting()
    {
        foreach (var reel in reels)
        {
            foreach (var symbol in reel.visibleSymbols)
            {
                symbol.Highlight(false);
            }
        }
    }

    /// <summary>
    /// Принудительно создаёт горизонтальный WIN (5 одинаковых символов в ряду).
    /// </summary>
    private void ForceWin()
    {
        // Выбираем случайный ряд для горизонтального выигрыша
        int targetRow = Random.Range(0, reels[0].visibleSymbols.Count);

        // Выбираем случайный символ для выигрыша
        SymbolData winningSymbol = CoinManager.Instance.GetRandomSymbolForWin();

        if (winningSymbol == null)
        {
            Debug.LogError("Не удалось получить символ для выигрыша.");
            return;
        }

        // Устанавливаем одинаковые символы во всех Reels на выбранном ряду
        foreach (var reel in reels)
        {
            var symbol = reel.visibleSymbols[targetRow];
            if (symbol != null)
            {
                symbol.SetSymbol(winningSymbol);
                Debug.Log($"Установлен символID {winningSymbol.symbolID} на {symbol.name}");
            }
            else
            {
                Debug.LogError($"symbol в Reels {reel.name} на ряду {targetRow} равен null.");
            }
        }

        Debug.Log($"Принудительный WIN в ряду {targetRow + 1} с symbolID = {winningSymbol.symbolID}");
    }

    /// <summary>
    /// Принудительно создаёт одновременно горизонтальный и вертикальный BIG-WIN.
    /// </summary>
    private void ForceBigWin()
    {
        // Создаём горизонтальный WIN
        int targetRow = Random.Range(0, reels[0].visibleSymbols.Count);
        SymbolData horizontalWinSymbol = CoinManager.Instance.GetRandomSymbolForWin();

        if (horizontalWinSymbol == null)
        {
            Debug.LogError("Не удалось получить символ для горизонтального выигрыша.");
            return;
        }

        foreach (var reel in reels)
        {
            var symbol = reel.visibleSymbols[targetRow];
            if (symbol != null)
            {
                symbol.SetSymbol(horizontalWinSymbol);
                Debug.Log($"Установлен символID {horizontalWinSymbol.symbolID} на {symbol.name}");
            }
            else
            {
                Debug.LogError($"symbol в Reels {reel.name} на ряду {targetRow} равен null.");
            }
        }

        Debug.Log($"Принудительный горизонтальный WIN в ряду {targetRow + 1} с symbolID = {horizontalWinSymbol.symbolID}");

        // Создаём вертикальный WIN для одного случайного Reel
        int targetReel = Random.Range(0, reels.Count);
        SymbolData verticalWinSymbol = CoinManager.Instance.GetRandomSymbolForWin();

        if (verticalWinSymbol == null)
        {
            Debug.LogError("Не удалось получить символ для вертикального выигрыша.");
            return;
        }

        foreach (var symbol in reels[targetReel].visibleSymbols)
        {
            symbol.SetSymbol(verticalWinSymbol);
            Debug.Log($"Установлен символID {verticalWinSymbol.symbolID} на {symbol.name}");
        }

        Debug.Log($"Принудительный вертикальный WIN в Reel {targetReel + 1} с symbolID = {verticalWinSymbol.symbolID}");
    }

    /// <summary>
    /// Активирует и анимирует панель для вертикального выигрыша.
    /// </summary>
    private void ShowVerticalWinPanel()
    {
        if (verticalWinAnimator != null && verticalWinPanel != null)
        {
            StartCoroutine(AnimatePanel(verticalWinPanel, verticalWinAnimator));
        }
    }

    /// <summary>
    /// Активирует и анимирует панель для горизонтального выигрыша.
    /// </summary>
    private void ShowHorizontalWinPanel()
    {
        if (horizontalWinAnimator != null && horizontalWinPanel != null)
        {
            StartCoroutine(AnimatePanel(horizontalWinPanel, horizontalWinAnimator));
        }
    }

    /// <summary>
    /// Активирует и анимирует панель для BIG WIN.
    /// </summary>
    private void ShowBigWinPanel()
    {
        if (bigWinAnimator != null && bigWinPanel != null)
        {
            StartCoroutine(AnimatePanel(bigWinPanel, bigWinAnimator));
        }
    }

    /// <summary>
    /// Корутина для анимации панели: появление и исчезновение.
    /// </summary>
    /// <param name="panel">Панель для анимации.</param>
    /// <param name="animator">Animator компонент панели.</param>
    /// <returns></returns>
    private IEnumerator AnimatePanel(GameObject panel, Animator animator)
    {
        // Активируем панель
        panel.SetActive(true);

        // Запускаем анимацию появления
        animator.Play("Appear");

        // Ждём окончания анимации появления
        // Предположим, что длительность анимации появления составляет 1.0 секунд
        yield return new WaitForSeconds(1.0f);

        // Ждём немного, чтобы панель оставалась видимой (например, 1 секунду)
        yield return new WaitForSeconds(0.5f);

        // Запускаем анимацию исчезновения
        animator.Play("Disappear");

        // Ждём окончания анимации исчезновения
        // Предположим, что длительность анимации исчезновения составляет 1.0 секунд
        yield return new WaitForSeconds(1.0f);

        // Деактивируем панель
        panel.SetActive(false);
    }
}
