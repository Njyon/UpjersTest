using System;
using UnityEngine;

public class CombatPhase : IGamePhase
{
    public event Action<GamePhaseType> OnPhaseComplete;
    EnemyManager enemyManager;
    UIManager uiManager;
    GameManager gameManager;

    public void EnterPhase()
    {
        enemyManager = EnemyManager.Instance;
        uiManager = UIManager.Instance;
        gameManager = GameManager.Instance;

        enemyManager.StartSpawningWave();
        enemyManager.AllEnemiesDied += AllEnemiesDied;

        uiManager.combatPhaseObj.SetActive(true);
        uiManager.fastForwardButton.onClick.AddListener(FastForwardToggle);
        gameManager.health.onCurrentValueChange += OnHealthValueChange;

    }

    public void ExitPhase()
    {
        if (gameManager != null)
        {
            gameManager.health.onCurrentValueChange -= OnHealthValueChange;
        }

        if (uiManager != null)
        {
            if (uiManager.fastForwardButton != null) uiManager.fastForwardButton.onClick.RemoveListener(FastForwardToggle);
            if (uiManager.combatPhaseObj != null) uiManager.combatPhaseObj.SetActive(false);
        }

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

    void FastForwardToggle()
    {
        GameTimeManager.Instance.ToggleTimeManipulation("fastForward", GameManager.Instance.fastForwardSpeed);
    }

    void OnHealthValueChange(float newValue, float oldValue)
    {
        if (newValue <= 0)
        {
            if (OnPhaseComplete != null)
                OnPhaseComplete.Invoke(GamePhaseType.Defeat);
        }
    }
}
