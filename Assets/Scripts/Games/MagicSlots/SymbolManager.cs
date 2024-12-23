using System.Collections.Generic;
using UnityEngine;

public class SymbolManager : MonoBehaviour
{
    public SymbolDataList symbolDataList; // Ссылка на Scriptable Object с данными символов

    // Аудио компоненты и клипы
    public AudioSource spinAudioSource;
    public AudioClip spinSound;
    public AudioClip stopSound;

    /// <summary>
    /// Метод для получения случайного SymbolData с учётом весов.
    /// </summary>
    /// <returns></returns>
    public SymbolData GetRandomSymbol()
    {
        if (symbolDataList == null || symbolDataList.symbols.Count == 0)
        {
            Debug.LogError("SymbolDataList не назначен или список символов пуст.");
            return null;
        }

        // Создаём список с учётом весов
        List<SymbolData> weightedSymbols = new List<SymbolData>();
        foreach (var symbol in symbolDataList.symbols)
        {
            for (int i = 0; i < symbol.weight; i++)
            {
                weightedSymbols.Add(symbol);
            }
        }

        if (weightedSymbols.Count == 0)
        {
            Debug.LogError("Список weightedSymbols пуст после применения весов.");
            return null;
        }

        int randomIndex = Random.Range(0, weightedSymbols.Count);
        return weightedSymbols[randomIndex];
    }

    /// <summary>
    /// Метод для получения SymbolData по ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SymbolData GetSymbolByID(int id)
    {
        return symbolDataList.symbols.Find(symbol => symbol.symbolID == id);
    }
}
