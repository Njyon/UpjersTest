using System.Collections.Generic;
using Ultra;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingelton<UIManager>
{
    // there is probably a better solution than this but good enough for now
    public GraphicRaycaster graphicRaycaster;

    public ResourceDisplayer healthVisulizer;
    public ResourceDisplayer goldVisulizer;
    public TMPro.TextMeshProUGUI currentWaveCount;

    [SerializeField] GameObject selectionHolder;

    [Header("BuildingPhase")]
    public GameObject buildingPhaseObj;
    public Button buildingButton;
    public Button startButton;


    [Header("CombatPhase")]
    public GameObject combatPhaseObj;
    public Button fastForwardButton;

    [Header("GameOverPhase")]
    public GameObject gameOverTextObj;
    public GameObject gameOverButtonObj;
    public Button gameOverButton;

    public void CreateSelectorPanelForRequests(List<ScriptableRequest> requests, out SelectorPanelElement selectorPanelElement, IRequestOwner requestOwner)
    {
        selectorPanelElement = CreateSelectionPanel();
        selectorPanelElement.SetupSelectorPanelElement();
        if (selectorPanelElement == null || selectorPanelElement.ContentHolder == null)
        {
            Debug.LogError(Ultra.Utilities.Instance.DebugErrorString("UIManager", "CreateSelectorPanelForRequests", "Panel or ContentTransform was NULL!"));
            return;
        }
        foreach (var request in requests)
        {
            RequestUIElement requestUIElement = CreateRequestUIElement(selectorPanelElement.ContentHolder);
            requestUIElement.SetupRequestUIElement(request, requestOwner);
            foreach (var cost in request.Costs)
            {
                ResourceUIElement resourceElement = CreateResourceUIElement(requestUIElement.ResourceCostHolder);
                resourceElement.SetupResourceElement(cost.Currency.CurrencySprite, cost.CostString);
                requestUIElement.ResourceCosts.Add(resourceElement);
            }
            selectorPanelElement.UIElements.Add(requestUIElement);
        }
    }

    public void UnselectSelectorPanel(SelectorPanelElement selectorPanelElement)
    {
        RemoveSelectorPanelElement(selectorPanelElement);
    }

    ResourceUIElement CreateResourceUIElement(Transform parent)
    {
        if (parent == null)
        {
            Debug.LogError(Ultra.Utilities.Instance.DebugErrorString("UIManager", "CreateResourceUIElement", "Parent for ResourceUIElement was NULL!"));
            return null;
        }
        ResourceUIElement resourceElement = GameAssets.Instance.UI.ResourceUIElementPool.GetValue();
        resourceElement.transform.SetParent(parent);
        resourceElement.SavePool(GameAssets.Instance.UI.ResourceUIElementPool);
        return resourceElement;
    }

    RequestUIElement CreateRequestUIElement(Transform parent)
    {
        if (parent == null)
        {
            Debug.LogError(Ultra.Utilities.Instance.DebugErrorString("UIManager", "CreateRequestUIElement", "Parent for RequestUIElement was NULL!"));
            return null;
        }
        RequestUIElement requestElement = GameAssets.Instance.UI.RequestUIElementPool.GetValue();
        requestElement.transform.SetParent(parent);
        requestElement.SavePool(GameAssets.Instance.UI.RequestUIElementPool);
        return requestElement;
    }

    SelectorPanelElement CreateSelectionPanel()
    {
        SelectorPanelElement selectorPanelElement = GameAssets.Instance.UI.SelectorPanelElementPool.GetValue();
        selectorPanelElement.transform.SetParent(selectionHolder.transform, false);
        selectorPanelElement.SavePool(GameAssets.Instance.UI.SelectorPanelElementPool);
        return selectorPanelElement;
    }

    void RemoveSelectorPanelElement(SelectorPanelElement selectorPanelElement)
    {
        if (selectorPanelElement == null)
        {
            Debug.LogError(Ultra.Utilities.Instance.DebugErrorString("UIManager", "RemoveSelectorPanelElement", "SelectorPanelElement was NULL!"));
            return;
        }
        selectorPanelElement.Cleanup();
    }
}
