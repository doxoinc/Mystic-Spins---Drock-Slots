// CoinUI.cs
using UnityEngine;
using UnityEngine.UI;
// Если используете TextMeshPro, раскомментируйте следующую строку и закомментируйте строку выше
// using TMPro;

public class CoinUI : MonoBehaviour
{
    // Для стандартного Text:
    public Text coinText;
    // Для TextMeshProUGUI:
    // public TextMeshProUGUI coinText;

    private void OnEnable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged += UpdateCoinText;
            UpdateCoinText(CoinManager.Instance.GetCoinCount()); // Инициализация текста
        }
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.CoinsChanged -= UpdateCoinText;
    }

    private void UpdateCoinText(int newCoinCount)
    {
        coinText.text = newCoinCount.ToString();
        // Если хотите отображать без процента, используйте:
        // coinText.text = "Coins: " + newCoinCount.ToString();
    }
}
