using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textCoins;
    [SerializeField] private int coins;

    public void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (coins / 1000000000 >= 1)
            textCoins.text = (((int)(coins / 1000000000f * 100)) / 10f).ToString() + "B";
        else if (coins / 1000000 >= 1)
            textCoins.text = (((int)(coins / 1000000f * 10)) / 10f).ToString() + "M";
        else if (coins / 10000 >= 1)
            textCoins.text = (((int)(coins / 10000f * 100)) / 10f).ToString() + "k";
        else
            textCoins.text = coins.ToString();
    }

    public void SetCoins(int newValue)
    {
        coins = newValue;
        UpdateText();
    }

    public void AddCoins(int newValue)
    {
        coins += newValue;
        UpdateText();
    }

    public int GetCoins()
    {
        return coins;
    }
}
