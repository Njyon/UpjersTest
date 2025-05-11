using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using System.Linq;

[Serializable]
public class FlipFlopSpawnLogic : ASpawnWaveLogic
{
    public override Wave ToSpawn(WaveData waveData, int currentWave, int waveAmountUsage)
    {
        Wave waveToBeConstructed = new Wave();
        waveToBeConstructed.spawnDelay = waveData.spawnDelay;  

        int totalSpawnAmount = 0;
        foreach (int spawnAmount in waveData.enemiesToSpawn.Values) 
        {
            // Generate the toalSpawn Amount times the growth % to calculate the growing wave
            totalSpawnAmount += Mathf.FloorToInt(spawnAmount * (1 + (waveData.enemyCountGrowth - 1f) * waveAmountUsage));
        }

        EnemyData[] enemyArray = waveData.enemiesToSpawn.Keys.ToArray();

        for (int i = 0; i < totalSpawnAmount; i++)
        {
            // iterate through keys to get the spawn pattern (spawn 1 of each kind and wrap around)
            int keyIndex = i % waveData.enemiesToSpawn.Keys.Count;
            waveToBeConstructed.enemiesToSpawn.Add(enemyArray[keyIndex]);
        }

        return waveToBeConstructed;
    }
}
