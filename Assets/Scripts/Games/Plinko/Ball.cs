// Ball.cs
using UnityEngine;

public class Ball : MonoBehaviour
{
    private int bet;
    private bool isCollected = false; // Флаг, указывающий, был ли шарик собран

    public void SetBet(int amount)
    {
        bet = amount;
    }

    public int GetBet()
    {
        return bet;
    }

    // Метод для проверки и установки флага
    public bool Collect()
    {
        if (!isCollected)
        {
            isCollected = true;
            return true;
        }
        return false;
    }
}
