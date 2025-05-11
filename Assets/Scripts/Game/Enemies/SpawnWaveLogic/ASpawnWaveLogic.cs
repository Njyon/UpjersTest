using AYellowpaper.SerializedCollections;
using UnityEngine;

public abstract class ASpawnWaveLogic
{
    public abstract Wave ToSpawn(WaveData waveData, int waveNumer, int waveDataUsage);
}
