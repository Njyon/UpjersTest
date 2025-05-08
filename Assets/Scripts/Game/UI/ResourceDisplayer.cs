using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplayer : MonoBehaviour
{
    public Image image;
    public TMPro.TextMeshProUGUI currencyAmount;


    [Header("Currency")]
    public ScriptableCurrency currencyType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image.sprite = currencyType.CurrencySprite;
    }

    public void CurrencyAmountChanged(int oldAmount, int newAmount)
    {
        currencyAmount.text = newAmount.ToString();
    }

}
