using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UltEvents;
using UnityEngine;

public class BuildingPhase : IGamePhase, IRequestOwner
{
    public event Action<GamePhaseType> OnPhaseComplete;
    SelectorPanelElement selectorPanel;
    UIManager uiManager;
    SelectionManager selectionManager;
    BuildingManager buildingManager;

    public void EnterPhase()
    {
        uiManager = UIManager.Instance;
        selectionManager = SelectionManager.Instance;
        buildingManager = BuildingManager.Instance;

        uiManager.buildingPhaseObj.SetActive(true);
        uiManager.buildingButton.onClick.AddListener(OnBuildButtonClicked);
        uiManager.startButton.onClick.AddListener(OnStartButtonClicked);
        selectionManager.selectionEvent.AddListener(OnNewSelect);
        buildingManager.cancleBuild += OnCancleBuild;
        buildingManager.onTowerReplace += OnTowerReplace;
    }

    public void ExitPhase()
    {
        if (buildingManager != null)
        {
            buildingManager.cancleBuild -= OnCancleBuild;
            buildingManager.onTowerReplace -= OnTowerReplace;
        }

        if (selectionManager != null)
        {
            if (selectionManager.contextEvent != null) selectionManager.contextEvent.RemoveListener(OnOpenContext);
            if (selectionManager.selectionEvent != null) selectionManager.selectionEvent.RemoveListener(OnNewSelect);
        }
        if (uiManager != null)
        {
            if (uiManager.startButton != null) uiManager.startButton.onClick.RemoveListener(OnStartButtonClicked);
            if (uiManager.buildingButton != null) uiManager.buildingButton.onClick.RemoveListener(OnBuildButtonClicked);
            if (uiManager.buildingPhaseObj != null) uiManager.buildingPhaseObj.SetActive(false);
        }
     
        if (selectorPanel != null) uiManager?.UnselectSelectorPanel(selectorPanel);
    }   

    public void UpdatePhase(float deltaTime)
    {

    }

    void OnBuildButtonClicked()
    {
        OpenPanel();
    }

    void OpenPanel()
    {
        uiManager?.buildingPhaseObj.SetActive(false);
        UIManager.Instance.CreateSelectorPanelForRequests(GameManager.Instance.possibleBuildingRequests, out selectorPanel, this);
        //await new WaitForEndOfFrame();  // Fast Fix but iam to lasy to implement a correct UI Pattern
        selectionManager.contextEvent.AddListener(OnOpenContext);
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
                    {
                        goldCost += cost.Cost;
                        SRequestCost rc = CreateCostWithNegativeAmount(cost);
                        currencyTransaction.Costs.Add(rc);
                    } break;
                case CurrencyType.Health:
                    {
                        healthCost += cost.Cost;
                        SRequestCost rc = CreateCostWithNegativeAmount(cost);
                        currencyTransaction.Costs.Add(rc);
                    } break;
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

        DeselectSelectorPanel();

        buildingManager.onTowerBuild += OnTowerBuild;
    }

    private static SRequestCost CreateCostWithNegativeAmount(SRequestCost cost)
    {
        SRequestCost rc = new SRequestCost();
        rc.Cost = -cost.Cost;
        rc.Currency = cost.Currency;
        rc.FormatCostString = cost.FormatCostString;
        return rc;
    }

    public void OnNewSelect(List<ISelectable> newSelection, List<ISelectable> oldSelection)
    {
        DeselectSelectorPanel();
        uiManager?.buildingPhaseObj.SetActive(true);
    }

    void DeselectSelectorPanel()
    {
        if (selectionManager != null && selectorPanel != null)
        {
            if (selectionManager.contextEvent != null) selectionManager.contextEvent.RemoveListener(OnOpenContext);
            uiManager?.UnselectSelectorPanel(selectorPanel);
            selectorPanel = null;
        }
    }

    void OnCancleBuild()
    {
        OpenPanel();
    }

    void OnOpenContext(IContextAction newContext, IContextAction oldContext)
    {
        DeselectSelectorPanel();
        uiManager?.buildingPhaseObj.SetActive(true);
    }

    void OnTowerBuild()
    {
        buildingManager.onTowerBuild -= OnTowerBuild;
        uiManager?.buildingPhaseObj.SetActive(true);
    }

    void OnTowerReplace()
    {
        buildingManager.onTowerReplaceCanceled += OnTowerReplacedCanceled;
        buildingManager.onTowerBuild += OnTowerBuild;
        uiManager?.buildingPhaseObj.SetActive(false);
    }

    void OnTowerReplacedCanceled()
    {
        buildingManager.onTowerReplaceCanceled -= OnTowerReplacedCanceled;
        uiManager?.buildingPhaseObj.SetActive(true);
    }
}
