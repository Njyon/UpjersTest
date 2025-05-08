using Ultra;
using UnityEngine;

public class GameManager : MonoSingelton<GameManager>
{
    [Header("PlayerHealth")]
    [SerializeField] int maxHealth;
    ResourceBase health;
    public ResourceBase Health
    {
        get {  return health; }
    }

    [Header("PlayerGold")]
    [SerializeField] int startGold;
    ResourceBase gold;
    public ResourceBase Gold
    {
        get { return gold; }
    }


    void Start()
    {
        health = new ResourceBase(maxHealth, maxHealth);
        health.onCurrentValueChange += HealthValueChanged;
        HealthValueChanged(health.CurrentValue, 0);

        gold = new ResourceBase(startGold);
        gold.onCurrentValueChange += GoldValueChanged;
        GoldValueChanged(gold.CurrentValue, 0);
    }

    private void OnDestroy()
    {
        if(health != null)
        {
            health.onCurrentValueChange -= HealthValueChanged;
        }
        if (gold != null)
        {
            gold.onCurrentValueChange -= GoldValueChanged;
        }
    }

    void HealthValueChanged(float newValue, float oldValue)
    {
        UIManager.Instance.healthVisulizer.CurrencyAmountChanged(Mathf.FloorToInt(oldValue), Mathf.FloorToInt(newValue));
    }

    void GoldValueChanged(float newValue, float oldValue)
    {
        UIManager.Instance.goldVisulizer.CurrencyAmountChanged(Mathf.FloorToInt(oldValue), Mathf.FloorToInt(newValue));
    }
}
