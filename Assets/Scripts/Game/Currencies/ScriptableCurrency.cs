using UnityEngine;

public enum CurrencyType
{
    Unknown,
    Gold,
    Health,
}

[CreateAssetMenu(fileName = "ScriptableCurrency", menuName = "Scriptable Objects/Currency/ScriptableCurrency")]
public class ScriptableCurrency : ScriptableObject
{
    public Sprite CurrencySprite;
    public CurrencyType CurrencyTyp;
}
