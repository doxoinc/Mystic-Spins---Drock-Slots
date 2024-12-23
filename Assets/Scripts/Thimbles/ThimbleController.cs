using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ThimbleController : MonoBehaviour
{
    [Header("UI Элементы")]
    public GameObject[] thimbles; // Массив ящиков
    public GameObject ball; // Объект мяча
    public TextMeshProUGUI resultText; // Текст для отображения результата
    public RectTransform canvas; // Canvas для позиционирования мяча
    public float liftHeight = 100.0f; // Высота подъёма ящика
    public float liftDuration = 0.5f; // Длительность подъёма/опускания
    public float shuffleSpeed = 0.5f; // Скорость перемешивания
    public Sprite thimbleSelectedSprite; // Спрайт выбранного ящика
    public Sprite thimbleBallSprite; // Спрайт ящика с мячом
    public float delayAfterShuffle = 1.0f; // Задержка после перемешивания
    public Button startButton; // Кнопка начала игры

    [Header("Настройки Цветов Текста")]
    public Color winTextColor = Color.yellow; // Цвет текста при выигрыше
    public Color loseTextColor = Color.red;   // Цвет текста при проигрыше

    private int ballPosition; // Текущая позиция мяча
    private bool hasChosen = false; // Флаг выбора игрока
    private bool isShuffling = false; // Флаг перемешивания
    private bool isAnimating = false; // Флаг анимации

    private Sprite[] originalThimbleSprites; // Оригинальные спрайты ящиков
    
    private ThimbleController thimbleController; // Ссылка на этот же контроллер
    
    public BetSystem betSystem; // Ссылка на систему ставок

    void Start()
    {
        thimbleController = FindObjectOfType<ThimbleController>();
        originalThimbleSprites = new Sprite[thimbles.Length];
        for (int i = 0; i < thimbles.Length; i++)
        {
            Image thimbleImage = thimbles[i].GetComponent<Image>();
            if (thimbleImage != null)
            {
                originalThimbleSprites[i] = thimbleImage.sprite;
            }
        }

        startButton.onClick.AddListener(StartThimble);
        startButton.interactable = true; 
        DisableThimbleButtons(true);    
    }

    public void StartThimble()
    {
        if (isAnimating || isShuffling) return; 

        hasChosen = false;
        resultText.text = "";
        resultText.color = Color.white;
        ball.SetActive(false);

        for (int i = 0; i < thimbles.Length; i++)
        {
            Image thimbleImage = thimbles[i].GetComponent<Image>();
            if (thimbleImage != null)
            {
                thimbleImage.sprite = originalThimbleSprites[i];
            }
            Button thimbleButton = thimbles[i].GetComponent<Button>();
            if (thimbleButton != null)
            {
                thimbleButton.interactable = false;
            }
        }

        PlaceBall();
        Debug.Log($"Ball placed under thimble: {ballPosition}");

        StartCoroutine(ShuffleThimbles());
        startButton.interactable = false; 
    }

    void PlaceBall()
    {
        ballPosition = Random.Range(0, thimbles.Length);
        UpdateBallPosition();
    }

    void UpdateBallPosition()
    {
        ball.transform.SetParent(canvas);
        RectTransform thimbleRect = thimbles[ballPosition].GetComponent<RectTransform>();
        RectTransform ballRect = ball.GetComponent<RectTransform>();

        ballRect.anchoredPosition = thimbleRect.anchoredPosition - new Vector2(0, 50);
    }

    IEnumerator ShuffleThimbles()
    {
        isShuffling = true;
        isAnimating = true;

        resultText.text = "Shuffling...";
        resultText.color = Color.white;

        DisableThimbleButtons(true);

        for (int i = 0; i < 10; i++)
        {
            int index1 = Random.Range(0, thimbles.Length);
            int index2 = Random.Range(0, thimbles.Length);

            if (index1 != index2)
            {
                yield return StartCoroutine(AnimateSwap(thimbles[index1], thimbles[index2]));

                if (ballPosition == index1)
                {
                    ballPosition = index2;
                }
                else if (ballPosition == index2)
                {
                    ballPosition = index1;
                }
                UpdateBallPosition();
            }
        }

        resultText.text = "Pick where the ball is";
        resultText.color = Color.white;
        yield return new WaitForSeconds(delayAfterShuffle);

        DisableThimbleButtons(false);
        isShuffling = false;
        yield return new WaitForSeconds(0.5f);
        isAnimating = false;
    }

    IEnumerator AnimateSwap(GameObject obj1, GameObject obj2)
    {
        RectTransform rect1 = obj1.GetComponent<RectTransform>();
        RectTransform rect2 = obj2.GetComponent<RectTransform>();

        Vector2 position1 = rect1.anchoredPosition;
        Vector2 position2 = rect2.anchoredPosition;

        float elapsedTime = 0;
        while (elapsedTime < shuffleSpeed)
        {
            rect1.anchoredPosition = Vector2.Lerp(position1, position2, elapsedTime / shuffleSpeed);
            rect2.anchoredPosition = Vector2.Lerp(position2, position1, elapsedTime / shuffleSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rect1.anchoredPosition = position2;
        rect2.anchoredPosition = position1;
    }

    public void CheckThimble(int thimbleIndex)
    {
        if (hasChosen || isShuffling || isAnimating) return;

        hasChosen = true;
        DisableOtherThimbles();

        Debug.Log($"Player chose thimble: {thimbleIndex}");
        StartCoroutine(LiftThimble(thimbles[thimbleIndex], thimbleIndex == ballPosition));

        Image thimbleImage = thimbles[thimbleIndex].GetComponent<Image>();
        if (thimbleImage != null)
        {
            thimbleImage.sprite = thimbleSelectedSprite;
        }
    }

    void DisableOtherThimbles()
    {
        for (int i = 0; i < thimbles.Length; i++)
        {
            Button thimbleButton = thimbles[i].GetComponent<Button>();
            if (thimbleButton != null)
            {
                thimbleButton.interactable = false;
            }
        }
    }

    void DisableThimbleButtons(bool disable)
    {
        foreach (var thimble in thimbles)
        {
            Button thimbleButton = thimble.GetComponent<Button>();
            if (thimbleButton != null)
            {
                thimbleButton.interactable = !disable;
            }
        }
    }

    IEnumerator LiftThimble(GameObject thimble, bool hasBall)
    {
        RectTransform thimbleRect = thimble.GetComponent<RectTransform>();
        Vector2 originalPosition = thimbleRect.anchoredPosition;
        Vector2 liftedPosition = originalPosition + new Vector2(0, liftHeight);

        float elapsedTime = 0;
        while (elapsedTime < liftDuration)
        {
            thimbleRect.anchoredPosition = Vector2.Lerp(originalPosition, liftedPosition, elapsedTime / liftDuration);
            elapsedTime += Time.deltaTime;

            if (hasBall)
            {
                ball.SetActive(true);
                Image thimbleImage = thimble.GetComponent<Image>();
                if (thimbleImage != null)
                {
                    thimbleImage.sprite = thimbleBallSprite;
                }
            }
            yield return null;
        }

        thimbleRect.anchoredPosition = liftedPosition;

        yield return new WaitForSeconds(0.2f);

        elapsedTime = 0;
        while (elapsedTime < liftDuration)
        {
            thimbleRect.anchoredPosition = Vector2.Lerp(liftedPosition, originalPosition, elapsedTime / liftDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        thimbleRect.anchoredPosition = originalPosition;

        if (hasBall)
        {
            resultText.text = "You win: +" + betSystem.currentBet;
            resultText.color = winTextColor;
        }
        else
        {
            resultText.text = "You lose: -" + betSystem.currentBet; 
            resultText.color = loseTextColor;
        }

        thimbleController.GetComponent<BetSystem>().ResolveBet(hasBall);

        startButton.interactable = true;
    }

    public void GoHome()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
