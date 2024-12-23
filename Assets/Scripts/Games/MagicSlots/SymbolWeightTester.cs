using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolWeightTester : MonoBehaviour
{
    public SymbolManager symbolManager;
    public int testSpins = 10000;

    private void Start()
    {
        StartCoroutine(TestSymbolWeights());
    }

    private IEnumerator TestSymbolWeights()
    {
        Dictionary<int, int> symbolCounts = new Dictionary<int, int>();

        for (int i = 0; i < testSpins; i++)
        {
            SymbolData symbol = symbolManager.GetRandomSymbol();
            if (symbol != null)
            {
                if (symbolCounts.ContainsKey(symbol.symbolID))
                    symbolCounts[symbol.symbolID]++;
                else
                    symbolCounts[symbol.symbolID] = 1;
            }

            // Optional: Log progress every 1000 spins
            if ((i + 1) % 1000 == 0)
            {
                Debug.Log($"Spins Completed: {i + 1}");
                yield return null;
            }
        }

        // Вывод результатов
        foreach (var kvp in symbolCounts)
        {
            SymbolData symbolData = symbolManager.GetSymbolByID(kvp.Key);
            if (symbolData != null)
            {
                float percentage = (float)kvp.Value / testSpins * 100f;
                Debug.Log($"Symbol ID: {kvp.Key}, Count: {kvp.Value}, Percentage: {percentage:F2}%");
            }
        }

        yield break;
    }
}
