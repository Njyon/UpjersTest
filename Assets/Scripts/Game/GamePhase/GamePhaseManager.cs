using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// GamePhaseManager handels all available GamePhases
/// Used like a SingleState state machine (only 1 game phase can be active at the time)
/// </summary>
public class GamePhaseManager : MonoBehaviour
{
    IGamePhase currentPhase;

    void Start()
    {
        StartPhase(GamePhaseType.Building);
    }

    void Update()
    {
        currentPhase.UpdatePhase(Time.deltaTime);
    }

    void OnDestroy()
    {
        if (currentPhase != null)
        {
            currentPhase.ExitPhase();
            currentPhase = null;
        }
    }

    /// <summary>
    /// Handle the new GamePhase,
    /// Exit the old phase, Create new phase, Enter new phase
    /// </summary>
    /// <param name="phaseType"></param>
    void StartPhase(GamePhaseType phaseType)
    {
        currentPhase?.ExitPhase();

        currentPhase = CreatePhase(phaseType);

        if (currentPhase == null) {
            Debug.LogError("currentPhase was null!");
            return;
        }

        currentPhase.OnPhaseComplete += HandlePhaseComplete;
        currentPhase.EnterPhase();
    }

    void HandlePhaseComplete(GamePhaseType nextPhase)
    {
        StartPhase(nextPhase);
    }

    /// <summary>
    /// Creates the GamePhase Class
    /// Every Enum Type needs his own implementation
    /// </summary>
    /// <param name="type"> GamePhase EnumTpye to create </param>
    /// <returns> Returns the GamePhase object </returns>
    IGamePhase CreatePhase(GamePhaseType type)
    {
        IGamePhase phase = null;    
        switch (type)
        {
            case GamePhaseType.Building:
                phase = new BuildingPhase();
                break;
            case GamePhaseType.Combat:
                phase = new CombatPhase();  
                break;
            case GamePhaseType.Defeat:
                phase = new DefeatPhase();
                break;
            case GamePhaseType.Restart:
                phase = new RestartPhase();
                break;
            default:
                Debug.LogError("GamePhaseType was invalid or not Implemented!");
                break;
        }
        return phase;
    }
}
