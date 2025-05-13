using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Ultra;
using UnityEngine;

public class GameManager : MonoSingelton<GameManager>
{
    [Header("PlayerHealth")]
    [SerializeField] int maxHealth;
    public ResourceBase health;

    [Header("PlayerGold")]
    [SerializeField] int startGold;
    public ResourceBase gold;

    [Header("TowerOptions")]
    public List<ScriptableRequest> possibleBuildingRequests = new List<ScriptableRequest>();

    [Header("ScriptableCurrencys")]
    [SerializedDictionary("CurrencyType", "ScriptableCurrency")]
    [SerializeField] SerializedDictionary<CurrencyType, ScriptableCurrency> currencys;

    [Header("FastForwardToggle")]
    public float fastForwardSpeed = 2;
     

    ResourceAccountent resourceAccountant;
    public ResourceAccountent ResourceAccountant
    {
        get { return resourceAccountant; }
    }

    void Start()
    {
        resourceAccountant = new ResourceAccountent(50, ResourceGetter, currencys);

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

    ResourceBase ResourceGetter(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Gold:
                return gold;
            case CurrencyType.Health:
                return health;
            default:
                Ultra.Utilities.Instance.DebugErrorString("GameManager", "ResourceGetter", "CurrencyType not Implemented!");
                return null;
        }
    }
}
