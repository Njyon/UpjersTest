using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CurrencyTransaction
{
    public CurrencyTransaction()
    {
        Costs = new List<SRequestCost>();
    }

    public List<SRequestCost> Costs;
}

public class ResourceAccountent
{
    TransactionHistory<CurrencyTransaction> history;
    Func<CurrencyType, ResourceBase> resourceGetter;
    SerializedDictionary<CurrencyType, ScriptableCurrency> scriptableCurrencys;
    public ResourceAccountent(int transaktionHistorySize, Func<CurrencyType, ResourceBase> resourceGetter, SerializedDictionary<CurrencyType, ScriptableCurrency> scriptableCurrencys)
    {
        this.history = new TransactionHistory<CurrencyTransaction>(transaktionHistorySize, CreateEmptyCurrencyTransaction);
        history.OnAddedTransAction += OnAddedTransaction;
        history.OnRemovedTransAction += OnRemovedTransaction;

        this.resourceGetter = resourceGetter;
        this.scriptableCurrencys = scriptableCurrencys;
    }

    ~ResourceAccountent()
    {
        if (history != null)
        {
            history.OnAddedTransAction -= OnAddedTransaction;
            history.OnRemovedTransAction -= OnRemovedTransaction;
        }
    }

    public void ChangeCurrentResourceValue(CurrencyType currencyType, float value) 
    {
        Dictionary<CurrencyType, float> dic = new Dictionary<CurrencyType, float>();   
        dic.Add(currencyType, value);
        CreateCurrencyTransactions(dic);
    }
    public float CurrentResourceAmount(CurrencyType currencyType)
    {
        ResourceBase r = resourceGetter(currencyType);
        return r.CurrentValue;
    }

    public void CreateCurrencyTransactions(Dictionary<CurrencyType, float> transactions)
    {
        CurrencyTransaction ct = new();
        foreach (KeyValuePair<CurrencyType, float> pair in transactions)
        {
            SRequestCost rc = new();
            rc.Cost = Mathf.FloorToInt(pair.Value);
            ScriptableCurrency sc = null;
            scriptableCurrencys.TryGetValue(pair.Key, out sc);
            if (sc == null)
            {
                Ultra.Utilities.Instance.DebugErrorString("ResourceAccountant", "CreateCurrencyTransactions", "ScriptableCurrency was null!");
            }
            rc.Currency = sc;

            ct.Costs.Add(rc);
        }

        history.Add(ct);
    }

    public void AddTransaction(CurrencyTransaction ct)
    {
        history.Add(ct);
    }

    CurrencyTransaction CreateEmptyCurrencyTransaction()
    {
        return new CurrencyTransaction();
    }


    void OnAddedTransaction(CurrencyTransaction ct)
    {
        foreach (SRequestCost cost in ct.Costs)
        {
            ResourceBase r = resourceGetter(cost.Currency.CurrencyTyp);
            r.AddCurrentValue(cost.Cost);
        }
    }

    void OnRemovedTransaction(CurrencyTransaction ct)
    {
        foreach (SRequestCost cost in ct.Costs)
        {
            ResourceBase r = resourceGetter(cost.Currency.CurrencyTyp);
            // invert value
            r.AddCurrentValue(cost.Cost * -1);
        }
    }
}
