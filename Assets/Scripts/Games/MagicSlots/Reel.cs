using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reel : MonoBehaviour
{
    public SymbolManager symbolManager; // Ссылка на SymbolManager
    public float spinDuration = 2f;     // Общая длительность прокрутки (секунды)
    public float spinInterval = 0.1f;   // Интервал между заменами символов (секунды)
    public List<Symbol> visibleSymbols; // Видимые символы на барабане (от верхнего к нижнему)

    private Coroutine spinCoroutine;

    public bool IsSpinning { get; private set; }

    private void Start()
    {
        InitializeSymbols();
    }

    /// <summary>
    /// Инициализирует все видимые символы случайными значениями.
    /// </summary>
    private void InitializeSymbols()
    {
        foreach (var symbol in visibleSymbols)
        {
            symbol.SetSymbol(symbolManager.GetRandomSymbol());
        }
    }

    /// <summary>
    /// Запускает вращение барабана.
    /// </summary>
    public void Spin()
    {
        if (IsSpinning)
            return;

        spinCoroutine = StartCoroutine(SpinReel());
    }

    /// <summary>
    /// Корутин для вращения барабана.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpinReel()
    {
        IsSpinning = true;
        float elapsed = 0f;

        // Воспроизводим звук прокрутки (если установлен)
        if (symbolManager != null && symbolManager.spinAudioSource != null && symbolManager.spinSound != null)
        {
            symbolManager.spinAudioSource.PlayOneShot(symbolManager.spinSound);
        }

        while (elapsed < spinDuration)
        {
            ShiftSymbolsUp();
            yield return new WaitForSeconds(spinInterval);
            elapsed += spinInterval;
        }

        // Воспроизводим звук остановки (если установлен)
        if (symbolManager != null && symbolManager.spinAudioSource != null && symbolManager.stopSound != null)
        {
            symbolManager.spinAudioSource.PlayOneShot(symbolManager.stopSound);
        }

        IsSpinning = false;
    }

    /// <summary>
    /// Сдвигает символы вверх и заменяет нижний символ новым.
    /// </summary>
    private void ShiftSymbolsUp()
    {
        // Перебираем символы от верхнего к предпоследнему и заменяем их значениями следующего символа
        for (int i = 0; i < visibleSymbols.Count - 1; i++)
        {
            visibleSymbols[i].SetSymbol(visibleSymbols[i + 1].symbolData);
        }

        // Получаем новый символ для нижнего места
        SymbolData newSymbolData = symbolManager.GetRandomSymbol();
        if (newSymbolData != null)
        {
            visibleSymbols[visibleSymbols.Count - 1].SetSymbol(newSymbolData);
        }
    }

    /// <summary>
    /// Получает текущие символы на барабане для проверки выигрыша.
    /// </summary>
    /// <returns></returns>
    public List<int> GetCurrentSymbols()
    {
        List<int> currentSymbols = new List<int>();
        foreach (var symbol in visibleSymbols)
        {
            if (symbol.symbolData != null)
            {
                currentSymbols.Add(symbol.symbolData.symbolID);
            }
        }
        return currentSymbols;
    }
}
