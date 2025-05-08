using System;
using UnityEngine;

public class BuildingPhase : IGamePhase
{
    public event Action<GamePhaseType> OnPhaseComplete;

    public void EnterPhase()
    {
        // Create UI / Enable it
    }

    public void ExitPhase()
    {
        // disable UI
    }

    public void UpdatePhase(float deltaTime)
    {

    }
}
