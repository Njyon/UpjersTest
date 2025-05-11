using System;
using UnityEngine;

public enum GamePhaseType
{
    Unknown,
    Building,
    Combat,
    Defeat
}

public interface IGamePhase
{
    public event Action<GamePhaseType> OnPhaseComplete;
    public void EnterPhase();

    public void ExitPhase();

    public void UpdatePhase(float deltaTime);
}
