using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    public SymbolData symbolData; // Ссылка на данные символа
    public Image symbolImage;

    // Добавьте компонент Outline или другой визуальный компонент для подсветки
    private Outline outline;

    private void Awake()
    {
        // Получаем компонент Outline, если он есть
        outline = GetComponent<Outline>();
    }

    /// <summary>
    /// Метод для установки символа.
    /// </summary>
    /// <param name="data"></param>
    public void SetSymbol(SymbolData data)
    {
        symbolData = data;
        if (symbolImage != null && symbolData.symbolSprite != null)
        {
            symbolImage.sprite = symbolData.symbolSprite;
            Debug.Log($"Symbol {name} установлен на sprite {symbolData.symbolSprite.name}");
        }
        else
        {
            Debug.LogWarning("symbolImage или symbolSprite не назначены.");
        }
    }

    /// <summary>
    /// Метод для подсветки символа.
    /// </summary>
    /// <param name="highlight"></param>
    public void Highlight(bool highlight)
    {
        if (outline != null)
        {
            outline.enabled = highlight;
        }
    }
}
