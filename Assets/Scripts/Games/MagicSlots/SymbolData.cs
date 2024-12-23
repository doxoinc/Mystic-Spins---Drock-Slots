using UnityEngine;

[System.Serializable]
public class SymbolData
{
    public int symbolID;           // Уникальный идентификатор символа
    public Sprite symbolSprite;    // Спрайт символа
    public int weight = 1;         // Вес для вероятности выпадения
}
