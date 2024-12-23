using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Добавлено для работы со сценами
// Если используете TextMeshPro:
// using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject wheelPanel;
    public GameObject dailyBonusPanel;
    public GameObject achievementsPanel;
    public GameObject leaderboardsPanel;
    public GameObject settingsPanel;
    public GameObject informationPanel;
    public GameObject gameModePanel; // Добавлено: Панель GameModeI

    [Header("Loading")]
    public GameObject loadingPanel; // Панель загрузки
    public Slider loadingSlider;     // Прогресс-бар загрузки
    public Text loadingPercentageText; // Текст прогресса загрузки (для стандартного Text)
    // Или, если используете TextMeshPro:
    // public TextMeshProUGUI loadingPercentageText;

    // Словарь для хранения Animator'ов панелей
    private Dictionary<GameObject, Animator> panelAnimators;

    // Переменная для хранения текущей активной панели
    private GameObject currentPanel;

    // Переменная для хранения Animator главного меню
    public Animator mainMenuAnimator;

    // Переменная для хранения счета (замени на свою систему)
    private int playerScore = 0;

    void Start()
    {
        // Инициализация словаря
        panelAnimators = new Dictionary<GameObject, Animator>
        {
            { wheelPanel, wheelPanel.GetComponent<Animator>() },
            { dailyBonusPanel, dailyBonusPanel.GetComponent<Animator>() },
            { achievementsPanel, achievementsPanel.GetComponent<Animator>() },
            { leaderboardsPanel, leaderboardsPanel.GetComponent<Animator>() },
            { settingsPanel, settingsPanel.GetComponent<Animator>() },
            { informationPanel, informationPanel.GetComponent<Animator>() },
            { gameModePanel, gameModePanel.GetComponent<Animator>() }, // Добавлено: Animator для GameModePanel
            { mainMenuPanel, mainMenuPanel.GetComponent<Animator>() }
        };

        // Убедись, что все панели скрыты кроме главного меню и LoadingPanel
        HideAllPanelsImmediate();
        mainMenuPanel.SetActive(true);
        currentPanel = mainMenuPanel;
        loadingPanel.SetActive(false); // Убедитесь, что LoadingPanel скрыта
    }

    // Методы для открытия панелей с анимацией
    public void ShowWheelPanel()
    {
        StartCoroutine(SwitchPanel(wheelPanel));
    }

    public void ShowDailyBonusPanel()
    {
        StartCoroutine(SwitchPanel(dailyBonusPanel));
    }

    public void ShowAchievementsPanel()
    {
        StartCoroutine(SwitchPanel(achievementsPanel));
    }

    public void ShowLeaderboardsPanel()
    {
        StartCoroutine(SwitchPanel(leaderboardsPanel));
    }

    public void ShowSettingsPanel()
    {
        StartCoroutine(SwitchPanel(settingsPanel));
    }

    public void ShowInformationPanel()
    {
        StartCoroutine(SwitchPanel(informationPanel));
    }

    public void ShowGameModePanel() // Добавлено: Метод для открытия GameModePanel
    {
        StartCoroutine(SwitchPanel(gameModePanel));
    }

    public void ShowMainMenu()
    {
        StartCoroutine(SwitchPanel(mainMenuPanel));
    }

    // Новый метод для возврата к главному меню
    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    // Метод для начала игры (теперь открывает GameModePanel)
    public void StartGame()
    {
        ShowGameModePanel();
    }

    // Метод для загрузки сцены асинхронно
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Проверка существования сцены
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("Сцена " + sceneName + " не найдена в Build Settings.");
            yield break;
        }

        // Показываем панель загрузки
        loadingPanel.SetActive(true);
        Animator loadingAnimator = loadingPanel.GetComponent<Animator>();
        loadingAnimator.SetTrigger("OpenTrigger");
        loadingSlider.value = 0f;
        loadingPercentageText.text = "0%"; // Инициализация текста

        // Закрываем текущую панель (GameModePanel)
        if (currentPanel != null)
        {
            Animator currentAnimator = panelAnimators[currentPanel];
            currentAnimator.SetTrigger("CloseTrigger");

            // Анимировать главное меню, если текущая панель - главное меню
            if (currentPanel == mainMenuPanel)
            {
                Debug.Log("Закрытие главного меню");
                mainMenuAnimator.SetTrigger("MainMenuClose");
            }

            // Ждать окончания анимации закрытия
            yield return new WaitForSeconds(GetAnimationLength(currentAnimator, "Close"));
            currentPanel.SetActive(false);
        }

        // Запускаем асинхронную загрузку сцены
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // Позволяет контролировать момент активации сцены

        // Обновляем прогресс-бар и текст
        while (!asyncLoad.isDone)
        {
            // Асинхронная загрузка достигает 0.9, после чего сцена готова к активации
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingSlider.value = progress;

            // Обновляем текст прогресса
            int percentage = Mathf.RoundToInt(progress * 100f);
            loadingPercentageText.text = percentage.ToString() + "%";

            // Когда загрузка почти завершена
            if (asyncLoad.progress >= 0.9f)
            {
                // Автоматически активируем сцену
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Закрываем панель загрузки с анимацией
        loadingAnimator.SetTrigger("CloseTrigger");
        yield return new WaitForSeconds(GetAnimationLength(loadingAnimator, "Close"));
        loadingPanel.SetActive(false);
    }

    // Метод для переключения панелей с анимацией
    private IEnumerator SwitchPanel(GameObject targetPanel)
    {
        if (currentPanel == targetPanel)
            yield break;

        // Анимировать закрытие текущей панели
        if (currentPanel != null)
        {
            Animator currentAnimator = panelAnimators[currentPanel];
            currentAnimator.SetTrigger("CloseTrigger");

            // Анимировать главное меню, если текущая панель - главное меню
            if (currentPanel == mainMenuPanel)
            {
                Debug.Log("Закрытие главного меню");
                mainMenuAnimator.SetTrigger("MainMenuClose");
            }

            // Ждать окончания анимации закрытия
            yield return new WaitForSeconds(GetAnimationLength(currentAnimator, "Close"));
            currentPanel.SetActive(false);
        }

        // Анимировать открытие целевой панели
        targetPanel.SetActive(true);
        Animator targetAnimatorOpen = panelAnimators[targetPanel];
        targetAnimatorOpen.SetTrigger("OpenTrigger");

        // Анимировать главное меню, если целевая панель не главное меню
        if (targetPanel != mainMenuPanel)
        {
            Debug.Log("Открытие главного меню");
            mainMenuAnimator.SetTrigger("MainMenuOpen");
        }

        // Ждать окончания анимации открытия
        yield return new WaitForSeconds(GetAnimationLength(targetAnimatorOpen, "Open"));

        currentPanel = targetPanel;
    }

    // Метод для получения длины анимации по названию состояния
    private float GetAnimationLength(Animator animator, string stateName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == stateName)
                return clip.length;
        }
        return 0.5f; // Значение по умолчанию
    }

    // Метод для немедленного скрытия всех панелей без анимации (используется при старте)
    private void HideAllPanelsImmediate()
    {
        foreach (var panel in panelAnimators.Keys)
        {
            if (panel != mainMenuPanel)
                panel.SetActive(false);
        }

        loadingPanel.SetActive(false); // Убедитесь, что LoadingPanel скрыта
    }
}
