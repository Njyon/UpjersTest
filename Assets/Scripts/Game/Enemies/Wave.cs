using System.Collections.Generic;
using UnityEngine;

public class Wave 
{
    public Wave()
    {
        enemiesToSpawn = new List<EnemyData>();
    }

    public List<EnemyData> enemiesToSpawn;
    public float spawnDelay;
}
