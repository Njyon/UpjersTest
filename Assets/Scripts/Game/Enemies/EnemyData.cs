using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
{
    public AEnemy prefab;
    public float baseHealth;
    public float speed;
    public int reward;
    public Sprite icon;
}
