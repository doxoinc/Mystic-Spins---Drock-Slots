public class Coin
{
    private int _coinCount;

    public int CoinCount
    {
        get { return _coinCount; }
        private set { _coinCount = value; }
    }

    public Coin(int initialCoins = 0)
    {
        _coinCount = initialCoins;
    }

    // Метод для добавления монет
    public void AddCoins(int amount)
    {
        if (amount < 0)
            throw new System.ArgumentException("Количество монет для добавления не может быть отрицательным.");
        _coinCount += amount;
    }

    // Метод для удаления монет
    public bool RemoveCoins(int amount)
    {
        if (amount < 0)
            throw new System.ArgumentException("Количество монет для удаления не может быть отрицательным.");

        if (_coinCount >= amount)
        {
            _coinCount -= amount;
            return true;
        }
        else
        {
            // Недостаточно монет
            return false;
        }
    }

    // Метод для сброса монет
    public void ResetCoins()
    {
        _coinCount = 0;
    }
}
