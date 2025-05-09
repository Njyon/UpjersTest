using UnityEngine;
using UnityEngine.Rendering;

public class GamePhaseManager : MonoBehaviour
{
    IGamePhase currentPhase;

    void Start()
    {
        StartPhase(GamePhaseType.Build);
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

    public void StartPhase(GamePhaseType phaseType)
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

    IGamePhase CreatePhase(GamePhaseType type)
    {
        IGamePhase phase = null;    
        switch (type)
        {
            case GamePhaseType.Build:
                phase = new BuildingPhase();
                break;
            case GamePhaseType.Combat:
                phase = new CombatPhase();  
                break;
            case GamePhaseType.Defeat:
                phase = new DefeatPhase();
                break;
            default:
                Debug.LogError("GamePhaseType was invalid or not Implemented!");
                break;
        }
        return phase;
    }
}
