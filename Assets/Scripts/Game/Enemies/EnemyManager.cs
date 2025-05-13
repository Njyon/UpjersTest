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
        set
        {
            int oldValue = currentWave;
            currentWave = value;
            CurrentWaveAmountChanged(currentWave, oldValue);   
        }
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

        // Init the Text Data
        CurrentWaveAmountChanged(currentWave, 0);
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

        toBeSpawnedWave = currentWaveData.spawnLogic.ToSpawn(currentWaveData, currentWave, waveDataUsage);
        toBeSpawnedWaveIndex = 0;

        SpawnEnemey();
    }

    void SpawnEnemey()
    {
        if (toBeSpawnedWaveIndex < toBeSpawnedWave.enemiesToSpawn.Count)
        {
            EnemyData toBeSpawnedEnemyData = toBeSpawnedWave.enemiesToSpawn[toBeSpawnedWaveIndex];
            AEnemy enemyToSpawn = toBeSpawnedEnemyData.prefab;
            AEnemy spawnedEnemy = GameObject.Instantiate(enemyToSpawn, enemyHolder);
            spawnedEnemy.Init(toBeSpawnedEnemyData.baseHealth, toBeSpawnedEnemyData.speed, toBeSpawnedEnemyData.damage, toBeSpawnedEnemyData.reward);
            spawnedEnemy.onEnemyDied += OnEnemyDied;
            spawnedEnemy.reachedTarget += OnEnemyReachedTarget;

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
        enemy.onEnemyDied -= OnEnemyDied;
        enemy.reachedTarget -= OnEnemyReachedTarget;
        currentlyAliveEnemies.Remove(enemy);
        Destroy(enemy.gameObject);
        CheckIfAllEnemiesAreDead();
    }

    /// <summary>
    /// Check if All Enemys are dead and if no enemy is supposed to spawn
    /// </summary>
    void CheckIfAllEnemiesAreDead()
    {
        if (currentlyAliveEnemies.Count <= 0 && toBeSpawnedWaveIndex >= toBeSpawnedWave.enemiesToSpawn.Count)
        {
            if (AllEnemiesDied != null)
                AllEnemiesDied();
            if (GameManager.Instance.ResourceAccountant.CurrentResourceAmount(CurrencyType.Health) > 0) CurrentWave++;
        }
    }

    void OnEnemyReachedTarget(AEnemy enemy)
    {
        GameManager.Instance.ResourceAccountant.AddCurrentResourceValue(CurrencyType.Health, -enemy.Damage);
        enemy.onEnemyDied -= OnEnemyDied;
        enemy.reachedTarget -= OnEnemyReachedTarget;
        currentlyAliveEnemies.Remove(enemy);
        CheckIfAllEnemiesAreDead();
        Destroy(enemy.gameObject);
    }

    void CurrentWaveAmountChanged(int newValue, int oldValue)
    {
        UIManager.Instance.currentWaveCount.text = newValue.ToString();
    }
}
