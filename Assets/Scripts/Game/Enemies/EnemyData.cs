using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public AEnemy prefab;
    public float baseHealth;
    public float speed;
    public int reward;
    public int damage;
    public Sprite icon;
}
