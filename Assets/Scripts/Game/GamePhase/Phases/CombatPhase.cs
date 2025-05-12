using System;
using UnityEngine;

public class CombatPhase : IGamePhase
{
    public event Action<GamePhaseType> OnPhaseComplete;
    EnemyManager enemyManager;
    UIManager uiManager;

    public void EnterPhase()
    {
        enemyManager = EnemyManager.Instance;
        uiManager = UIManager.Instance;

        enemyManager.StartSpawningWave();
        enemyManager.AllEnemiesDied += AllEnemiesDied;

        uiManager.combatPhaseObj.SetActive(true);
        uiManager.fastForwardButton.onClick.AddListener(FastForwardToggle);

    }

    public void ExitPhase()
    {
        if (enemyManager != null)
        {
            enemyManager.AllEnemiesDied -= AllEnemiesDied;
        }

        if (uiManager != null)
        {
            if (uiManager.fastForwardButton != null) uiManager.fastForwardButton.onClick.RemoveListener(FastForwardToggle);
            if (uiManager.combatPhaseObj != null) uiManager.combatPhaseObj.SetActive(false);
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

    void FastForwardToggle()
    {
        GameTimeManager.Instance.ToggleTimeManipulation("fastForward", GameManager.Instance.fastForwardSpeed);
    }
}
