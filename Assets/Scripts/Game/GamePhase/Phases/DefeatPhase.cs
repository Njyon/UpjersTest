using System;
using UnityEngine;

public class DefeatPhase : IGamePhase
{
    public event Action<GamePhaseType> OnPhaseComplete;

    UIManager uiManager;

    public void EnterPhase()
    {
        uiManager = UIManager.Instance;
        uiManager.gameOverButtonObj.SetActive(true);
        uiManager.gameOverTextObj.SetActive(true);
        uiManager.gameOverButton.onClick.AddListener(OnRestartButtonPressed);
    }

    public void ExitPhase()
    {
        if (uiManager != null)
        {
            uiManager.gameOverButton.onClick.RemoveListener(OnRestartButtonPressed);
            uiManager.gameOverTextObj.SetActive(false);
            uiManager.gameOverButtonObj.SetActive(false);
        }
    }

    public void UpdatePhase(float deltaTime)
    {

    }

    void OnRestartButtonPressed()
    {
        if (OnPhaseComplete != null) OnPhaseComplete.Invoke(GamePhaseType.Restart);
    }
}
