using System;
using UnityEngine;

public class CombatPhase : IGamePhase
{
    public event Action<GamePhaseType> OnPhaseComplete;
    EnemyManager enemyManager;

    public void EnterPhase()
    {
        enemyManager = EnemyManager.Instance;

        enemyManager.StartSpawningWave();
        enemyManager.AllEnemiesDied += AllEnemiesDied;
    }

    public void ExitPhase()
    {
        if (enemyManager != null)
        {
            enemyManager.AllEnemiesDied -= AllEnemiesDied;
        }
    }

    public void UpdatePhase(float deltaTime)
    {

    }

    void AllEnemiesDied()
    {
        if (OnPhaseComplete != null)
            OnPhaseComplete.Invoke(GamePhaseType.Building);
    }
}
