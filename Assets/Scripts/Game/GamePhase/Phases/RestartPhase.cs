using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartPhase : IGamePhase
{
    public event Action<GamePhaseType> OnPhaseComplete;

    public void EnterPhase()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitPhase()
    {

    }

    public void UpdatePhase(float deltaTime)
    {

    }
}
