using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    // Событие для уведомления UI или других систем о изменении количества монет
    public delegate void OnCoinsChanged(int newCoinCount);
    public event OnCoinsChanged CoinsChanged;

    private void Awake()
    {
        // Реализация паттерна Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Убедитесь, что GameData присутствует
            if (GameData.Instance == null)
            {
                Debug.LogError("GameData.Instance не найден в сцене. Пожалуйста, добавьте объект с компонентом GameData.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Метод для добавления монет
    public void AddCoins(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("Нельзя добавлять отрицательное количество монет.");
            return;
        }

        GameData.Instance.PlayerCoins.AddCoins(amount);
        CoinsChanged?.Invoke(GameData.Instance.PlayerCoins.CoinCount);
        
        // Сохраняем данные после добавления монет
        GameData.Instance.SaveData();
        Debug.Log($"Добавлено {amount} монет. Всего монет: {GameData.Instance.PlayerCoins.CoinCount}");
    }

    // Метод для удаления монет
    public bool RemoveCoins(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("Нельзя удалять отрицательное количество монет.");
            return false;
        }

        bool success = GameData.Instance.PlayerCoins.RemoveCoins(amount);
        if (success)
        {
            CoinsChanged?.Invoke(GameData.Instance.PlayerCoins.CoinCount);
            
            // Сохраняем данные после удаления монет
            GameData.Instance.SaveData();
            Debug.Log($"Удалено {amount} монет. Осталось монет: {GameData.Instance.PlayerCoins.CoinCount}");
        }
        else
        {
            Debug.LogWarning("Недостаточно монет для удаления.");
        }
        return success;
    }

    // Метод для получения текущего количества монет
    public int GetCoinCount()
    {
        return GameData.Instance.PlayerCoins.CoinCount;
    }

    // Метод для сброса монет
    public void ResetCoins()
    {
        GameData.Instance.PlayerCoins.ResetCoins();
        CoinsChanged?.Invoke(GameData.Instance.PlayerCoins.CoinCount);
        
        // Сохраняем данные после сброса монет
        GameData.Instance.SaveData();
        Debug.Log("Монеты сброшены.");
    }

    /// <summary>
    /// Получает случайный SymbolData для создания выигрыша.
    /// </summary>
    /// <returns></returns>
    public SymbolData GetRandomSymbolForWin()
    {
        // Для создания выигрыша можно выбирать определённые символы или применять особые правила
        // В данном примере просто используем существующий метод
        SymbolManager symbolManager = FindObjectOfType<SymbolManager>();
        if (symbolManager != null)
        {
            return symbolManager.GetRandomSymbol();
        }
        else
        {
            Debug.LogError("SymbolManager не найден в сцене.");
            return null;
        }
    }
}
