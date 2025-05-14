using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SRequestCost
{
    public ScriptableCurrency Currency;
    public int Cost;
    public string FormatCostString;

    public string CostString
    {
        get { return Cost.ToString(FormatCostString); }
    }
}

[CreateAssetMenu(fileName = "ScriptableRequest", menuName = "Scriptable Objects/Request/ScriptableRequest")]
public class ScriptableRequest : ScriptableObject
{
    public Sprite RequestSprite;
    public string RequestName;
    public string RequestDescription;

    [SerializeReference, SubclassSelector]
    public List<ARequest> Requests;

    public List<SRequestCost> Costs;
}
