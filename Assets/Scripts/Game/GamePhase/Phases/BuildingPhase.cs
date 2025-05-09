using System;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BuildingPhase : IGamePhase, IRequestOwner
{
    public event Action<GamePhaseType> OnPhaseComplete;
    SelectorPanelElement selectorPanel;

    public void EnterPhase()
    {
        UIManager.Instance.BuildingPhaseObj.SetActive(true);
        UIManager.Instance.buildingButton.onClick.AddListener(OnBuildButtonClicked);
        UIManager.Instance.startButton.onClick.AddListener(OnStartButtonClicked);
        SelectionManager.Instance.selectionEvent.AddListener(OnNewSelect);
    }

    public void ExitPhase()
    {
        SelectionManager.Instance.selectionEvent.RemoveListener(OnNewSelect);
        UIManager.Instance.startButton.onClick.RemoveListener(OnStartButtonClicked);
        UIManager.Instance.buildingButton.onClick.RemoveListener(OnBuildButtonClicked);
        UIManager.Instance.BuildingPhaseObj.SetActive(false);
    }

    public void UpdatePhase(float deltaTime)
    {

    }

    void OnBuildButtonClicked()
    {
        UIManager.Instance.CreateSelectorPanelForRequests(GameManager.Instance.possibleBuildingRequests, out selectorPanel, this);
    }

    void OnStartButtonClicked() 
    { 
        if (OnPhaseComplete != null) 
            OnPhaseComplete.Invoke(GamePhaseType.Combat);
    }

    public void QueueRequest(RequestTransaction requestTransaction)
    {
        int goldCost = 0;
        int healthCost = 0;
        CurrencyTransaction currencyTransaction = new();
        foreach (SRequestCost cost in requestTransaction.costs)
        {
            switch (cost.Currency.CurrencyTyp)
            {
                case CurrencyType.Gold:
                    goldCost += cost.Cost;
                    currencyTransaction.Costs.Add(cost);
                    break;
                case CurrencyType.Health:
                    healthCost += cost.Cost;
                    currencyTransaction.Costs.Add(cost);
                    break;
                default:
                    Ultra.Utilities.Instance.DebugErrorString("BuildingPhase", "QueueRequest", "CurrencyType not implemented or UNKNOWN!");
                    break;
            }
        }

        if (goldCost > GameManager.Instance.ResourceAccountent.CurrentResourceAmount(CurrencyType.Gold) || healthCost > GameManager.Instance.ResourceAccountent.CurrentResourceAmount(CurrencyType.Health))
        {
            Ultra.Utilities.Instance.DebugLogOnScreen(Ultra.Utilities.Instance.DebugLogString("BuildingPhase", "QueueRequest", "Not enough Currency to request Action!"));
            return;
        }

        GameManager.Instance.ResourceAccountent.AddTransaction(currencyTransaction);

        foreach (ARequest request in requestTransaction.requests)
        {
            request.ExecuteRequest(this);
        }

        UIManager.Instance.UnselectSelectorPanel(selectorPanel);
    }

    public void OnNewSelect(List<ISelectable> newSelection, List<ISelectable> oldSelection)
    {
        UIManager.Instance.UnselectSelectorPanel(selectorPanel);
    }
}
