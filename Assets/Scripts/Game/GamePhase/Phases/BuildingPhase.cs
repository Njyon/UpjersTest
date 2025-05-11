using System;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BuildingPhase : IGamePhase, IRequestOwner
{
    public event Action<GamePhaseType> OnPhaseComplete;
    SelectorPanelElement selectorPanel;
    UIManager uiManager;
    SelectionManager selectionManager;

    public void EnterPhase()
    {
        uiManager = UIManager.Instance;
        selectionManager = SelectionManager.Instance;

        uiManager?.BuildingPhaseObj.SetActive(true);
        uiManager?.buildingButton.onClick.AddListener(OnBuildButtonClicked);
        uiManager?.startButton.onClick.AddListener(OnStartButtonClicked);
        selectionManager?.selectionEvent.AddListener(OnNewSelect);
    }

    public void ExitPhase()
    {
        selectionManager?.selectionEvent.RemoveListener(OnNewSelect);
        uiManager?.startButton.onClick.RemoveListener(OnStartButtonClicked);
        uiManager?.buildingButton.onClick.RemoveListener(OnBuildButtonClicked);
        uiManager?.BuildingPhaseObj.SetActive(false);
        if (selectorPanel != null) uiManager?.UnselectSelectorPanel(selectorPanel);
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

        if (goldCost > GameManager.Instance.ResourceAccountant.CurrentResourceAmount(CurrencyType.Gold) || healthCost > GameManager.Instance.ResourceAccountant.CurrentResourceAmount(CurrencyType.Health))
        {
            Ultra.Utilities.Instance.DebugLogOnScreen(Ultra.Utilities.Instance.DebugLogString("BuildingPhase", "QueueRequest", "Not enough Currency to request Action!"));
            return;
        }

        GameManager.Instance.ResourceAccountant.AddTransaction(currencyTransaction);

        foreach (ARequest request in requestTransaction.requests)
        {
            request.ExecuteRequest(this);
        }

        uiManager?.UnselectSelectorPanel(selectorPanel);
    }

    public void OnNewSelect(List<ISelectable> newSelection, List<ISelectable> oldSelection)
    {
        uiManager?.UnselectSelectorPanel(selectorPanel);
    }
}
