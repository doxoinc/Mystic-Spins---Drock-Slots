// GameData.cs
using UnityEngine;
using System;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public Coin PlayerCoins { get; private set; }

    private DateTime lastSpinTime;
    private const string LastSpinTimeKey = "PlayerLastSpinTime";

    // Словарь для хранения времени последнего открытия каждой коробки
    private Dictionary<string, DateTime> lastOpenTimes;
    private const string LastOpenTimePrefix = "PlayerLastOpenTime_";

    public DateTime LastSpinTime
    {
        get { return lastSpinTime; }
        private set { lastSpinTime = value; }
    }

    private void Awake()
    {
        // Реализация паттерна Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();

            // Инициализация PlayerCoins в Awake
            if (PlayerCoins == null)
            {
                PlayerCoins = new Coin();
                Debug.Log("PlayerCoins инициализирован в Awake.");
            }

            Debug.Log("GameData Awake. PlayerCoins initialized with " + PlayerCoins.CoinCount + " coins.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Метод для загрузки данных
    public void LoadData()
    {
        // Загрузка монет
        int coins = PlayerPrefs.GetInt("PlayerCoins", 0);
        PlayerCoins = new Coin(coins);
        Debug.Log("Монеты загружены: " + PlayerCoins.CoinCount);

        // Загрузка времени последней прокрутки
        string lastSpinTimeStr = PlayerPrefs.GetString(LastSpinTimeKey, "");
        if (!string.IsNullOrEmpty(lastSpinTimeStr))
        {
            if (DateTime.TryParse(lastSpinTimeStr, out DateTime parsedTime))
            {
                lastSpinTime = parsedTime;
                Debug.Log("LastSpinTime загружено: " + lastSpinTime);
            }
            else
            {
                lastSpinTime = DateTime.MinValue;
                Debug.LogWarning("Не удалось разобрать LastSpinTime. Установлено в DateTime.MinValue.");
            }
        }
        else
        {
            lastSpinTime = DateTime.MinValue;
            Debug.Log("LastSpinTime отсутствует. Установлено в DateTime.MinValue.");
        }

        // Загрузка времени последнего открытия коробок
        lastOpenTimes = new Dictionary<string, DateTime>();

        string[] boxIds = { "Red", "Green", "Blue" };
        foreach (var boxId in boxIds)
        {
            string key = LastOpenTimePrefix + boxId;
            string lastOpenTimeStr = PlayerPrefs.GetString(key, "");
            if (!string.IsNullOrEmpty(lastOpenTimeStr))
            {
                if (DateTime.TryParse(lastOpenTimeStr, out DateTime parsedTime))
                {
                    lastOpenTimes[boxId] = parsedTime;
                    Debug.Log($"LastOpenTime для {boxId} загружено: {parsedTime}");
                }
                else
                {
                    lastOpenTimes[boxId] = DateTime.MinValue;
                    Debug.LogWarning($"Не удалось разобрать LastOpenTime для {boxId}. Установлено в DateTime.MinValue.");
                }
            }
            else
            {
                lastOpenTimes[boxId] = DateTime.MinValue;
                Debug.Log($"LastOpenTime для {boxId} отсутствует. Установлено в DateTime.MinValue.");
            }
        }

        // Загрузка других настроек (при необходимости)
        // string setting = PlayerPrefs.GetString("SomeSetting", "DefaultValue");
        // PlayerSettings = new Settings(setting);
    }

    // Метод для сохранения данных
    public void SaveData()
    {
        // Сохранение монет
        PlayerPrefs.SetInt("PlayerCoins", PlayerCoins.CoinCount);
        Debug.Log("Монеты сохранены: " + PlayerCoins.CoinCount);

        // Сохранение времени последней прокрутки
        PlayerPrefs.SetString(LastSpinTimeKey, lastSpinTime.ToString());
        Debug.Log("LastSpinTime сохранено: " + lastSpinTime);

        // Сохранение времени последнего открытия коробок
        foreach (var kvp in lastOpenTimes)
        {
            string key = LastOpenTimePrefix + kvp.Key;
            PlayerPrefs.SetString(key, kvp.Value.ToString());
            Debug.Log($"LastOpenTime для {kvp.Key} сохранено: {kvp.Value}");
        }

        // Сохранение других настроек (при необходимости)
        // PlayerPrefs.SetString("SomeSetting", PlayerSettings.SomeValue);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Сбрасывает все данные игрока до начального состояния.
    /// </summary>
    [ContextMenu("Reset All Player Data")]
    public void ResetData()
    {
        Debug.Log("ResetData вызван.");

        // Проверка на null и инициализация PlayerCoins при необходимости
        if (PlayerCoins == null)
        {
            PlayerCoins = new Coin();
            Debug.LogWarning("PlayerCoins был null. Инициализирован новый экземпляр.");
        }

        // Сброс монет
        PlayerCoins.ResetCoins();
        PlayerPrefs.DeleteKey("PlayerCoins");
        Debug.Log("Монеты сброшены.");

        // Сброс времени последней прокрутки
        PlayerPrefs.DeleteKey(LastSpinTimeKey);
        lastSpinTime = DateTime.MinValue;
        Debug.Log("LastSpinTime сброшено.");

        // Сброс времени последнего открытия коробок
        string[] boxIds = { "Red", "Green", "Blue" };
        foreach (var boxId in boxIds)
        {
            string key = LastOpenTimePrefix + boxId;
            PlayerPrefs.DeleteKey(key);
            lastOpenTimes[boxId] = DateTime.MinValue;
            Debug.Log($"LastOpenTime для {boxId} сброшено.");
        }

        // Сброс других данных (при необходимости)
        // PlayerSettings.ResetSettings();
        // PlayerPrefs.DeleteKey("SomeSetting");
        // Debug.Log("Настройки сброшены.");

        // Сброс других необходимых ключей PlayerPrefs (при необходимости)
        // PlayerPrefs.DeleteKey("AnotherKey");

        // Если у вас есть другие компоненты, которые хранят свои данные через PlayerPrefs,
        // добавьте соответствующие строки для их сброса.

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Устанавливает время последней прокрутки колеса.
    /// </summary>
    /// <param name="time">Время последней прокрутки.</param>
    public void SetLastSpinTime(DateTime time)
    {
        lastSpinTime = time;
        PlayerPrefs.SetString(LastSpinTimeKey, lastSpinTime.ToString());
        PlayerPrefs.Save();
        Debug.Log("SetLastSpinTime вызван: " + lastSpinTime);
    }

    /// <summary>
    /// Получает время последней прокрутки колеса.
    /// </summary>
    /// <returns>Время последней прокрутки.</returns>
    public DateTime GetLastSpinTime()
    {
        return lastSpinTime;
    }

    /// <summary>
    /// Устанавливает время последнего открытия коробки.
    /// </summary>
    /// <param name="boxId">Идентификатор коробки.</param>
    /// <param name="time">Время открытия.</param>
    public void SetLastOpenTime(string boxId, DateTime time)
    {
        if (lastOpenTimes.ContainsKey(boxId))
        {
            lastOpenTimes[boxId] = time;
        }
        else
        {
            lastOpenTimes.Add(boxId, time);
        }

        string key = LastOpenTimePrefix + boxId;
        PlayerPrefs.SetString(key, time.ToString());
        PlayerPrefs.Save();
        Debug.Log($"SetLastOpenTime вызван для {boxId}: {time}");
    }

    /// <summary>
    /// Получает время последнего открытия коробки.
    /// </summary>
    /// <param name="boxId">Идентификатор коробки.</param>
    /// <returns>Время последнего открытия.</returns>
    public DateTime GetLastOpenTime(string boxId)
    {
        if (lastOpenTimes.ContainsKey(boxId))
        {
            return lastOpenTimes[boxId];
        }
        else
        {
            Debug.LogWarning($"GetLastOpenTime: Коробка с ID {boxId} не найдена. Возвращается DateTime.MinValue.");
            return DateTime.MinValue;
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}
