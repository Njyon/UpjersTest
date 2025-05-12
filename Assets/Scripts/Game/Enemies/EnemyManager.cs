using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Ultra;
using UnityEngine;

public class EnemyManager : MonoSingelton<EnemyManager>
{
    [SerializeField] Transform enemyHolder;
 
    int currentWave = 1;
    public int CurrentWave
    {
        get { return currentWave; }
    }
    int waveDataUsage;

    WaveData currentWaveData;

    [SerializedDictionary("Wave", "WaveData")]
    [SerializeField] SerializedDictionary<int, WaveData> waves;

    Ultra.Timer waveSpawnTimer;
    Wave toBeSpawnedWave;
    int toBeSpawnedWaveIndex;

    List<AEnemy> currentlyAliveEnemies;

    public Action AllEnemiesDied;

    void Start()
    {
        currentlyAliveEnemies = new List<AEnemy>();
        waveSpawnTimer = new Ultra.Timer();
        waveSpawnTimer.onTimerFinished += OnWaveSpawnTimerFinished;
    }

    private void OnDestroy()
    {
        if (waveSpawnTimer != null) waveSpawnTimer.onTimerFinished -= OnWaveSpawnTimerFinished;
    }

    void Update()
    {
        waveSpawnTimer?.Update(Time.deltaTime);
    }

    public void StartSpawningWave()
    {
        waves.TryGetValue(currentWave, out WaveData waveData);
        if (waveData != null)
        {
            waveDataUsage = 0;
            currentWaveData = waveData;
        }
        else
            waveDataUsage++;

        if (currentWaveData == null)
        {
            Ultra.Utilities.Instance.DebugErrorString("EnemyManager", "StartSpawningWave", "CurrentWaveData was null!");
            return;
        }

        toBeSpawnedWave = waveData.spawnLogic.ToSpawn(waveData, currentWave, waveDataUsage);
        toBeSpawnedWaveIndex = 0;

        SpawnEnemey();

        currentWave++;
    }

    void SpawnEnemey()
    {
        if (toBeSpawnedWaveIndex < toBeSpawnedWave.enemiesToSpawn.Count)
        {
            EnemyData toBeSpawnedEnemyData = toBeSpawnedWave.enemiesToSpawn[toBeSpawnedWaveIndex];
            AEnemy enemyToSpawn = toBeSpawnedEnemyData.prefab;
            AEnemy spawnedEnemy = GameObject.Instantiate(enemyToSpawn, enemyHolder);
            spawnedEnemy.Init(toBeSpawnedEnemyData.baseHealth, toBeSpawnedEnemyData.speed, toBeSpawnedEnemyData.damage, OnEnemyDied, OnEnemyReachedTarget);

            currentlyAliveEnemies.Add(spawnedEnemy);
            waveSpawnTimer.Start(toBeSpawnedWave.spawnDelay);
            toBeSpawnedWaveIndex++;
        }
    }

    void OnWaveSpawnTimerFinished()
    {
        SpawnEnemey();
    }

    void OnEnemyDied(AEnemy enemy)
    {
        currentlyAliveEnemies.Remove(enemy);
        CheckIfAllEnemiesAreDead();
    }

    void CheckIfAllEnemiesAreDead()
    {
        if (currentlyAliveEnemies.Count <= 0)
        {
            if (AllEnemiesDied != null) AllEnemiesDied();
        }
    }

    void OnEnemyReachedTarget(AEnemy enemy)
    {
        GameManager.Instance.ResourceAccountant.ChangeCurrentResourceValue(CurrencyType.Health, -enemy.Damage);
        currentlyAliveEnemies.Remove(enemy);
        CheckIfAllEnemiesAreDead();
        Destroy(enemy.gameObject);
    }
}
