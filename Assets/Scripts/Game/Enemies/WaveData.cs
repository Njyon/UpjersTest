using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{

    [SerializedDictionary("Enemy", "Amount")]
    public SerializedDictionary<EnemyData, int> enemiesToSpawn;

    [SerializeReference, SubclassSelector]
    public ASpawnWaveLogic spawnLogic;

    public float enemyCountGrowth = 1.2f;

    public float spawnDelay = 1.5f;
}
