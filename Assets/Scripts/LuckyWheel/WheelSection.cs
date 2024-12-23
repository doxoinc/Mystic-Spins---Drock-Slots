// WheelSection.cs
using UnityEngine;

[System.Serializable]
public class WheelSection
{
    public string itemName;      // Название предмета
    public int coins;            // Количество монет за выпадение
    [Range(1, 100)]
    public int weight;           // Вес секции для выпадения
    public Sprite sectionImage;  // Изображение секции (если необходимо)
}
