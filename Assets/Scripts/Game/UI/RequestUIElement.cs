using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pooled Button Object thats used to Visulize Requests
/// </summary>
public class RequestUIElement : MonoBehaviour, IUIElement, IUIPoolableUIElement<RequestUIElement>
{
    [SerializeField] Image requestImage;
    [SerializeField] TMPro.TextMeshProUGUI requestName;
    [SerializeField] Button requestButton;
    [SerializeField] GameObject requestResourceCostHolder;
    public Transform ResourceCostHolder
    {
        get { return requestResourceCostHolder.transform; }
    }

    List<IUIElement> resourceCosts;
    public List<IUIElement> ResourceCosts
    {
        get { return resourceCosts; }
    }

    ScriptableRequest scriptableRequest;
    GameObjectTypePool<RequestUIElement> ownerPool;
    IRequestOwner requestOwner;

    public void SetupRequestUIElement(ScriptableRequest request, IRequestOwner requestOwner)
    {
        resourceCosts = new List<IUIElement>();
        this.requestOwner = requestOwner;

        requestImage.sprite = request.RequestSprite;
        requestName.text = request.RequestName;
        scriptableRequest = request;


        requestButton.onClick.AddListener(ButtonPress);
    }

    void ButtonPress()
    {
        requestOwner.QueueRequest(new RequestTransaction(scriptableRequest.Requests, scriptableRequest.Costs));
    }

    /// <summary>
    /// Remove Data and return back to its pool
    /// </summary>
    public void Cleanup()
    {
        requestImage.sprite = null;
        requestName.text = "Name";

        requestButton.onClick.RemoveListener(ButtonPress);

        foreach (var resourceCost in resourceCosts)
        {
            resourceCost.Cleanup();
        }

        requestOwner = null;
        GetPool().ReturnValue(this);
    }

    public void SavePool(GameObjectTypePool<RequestUIElement> pool)
    {
        this.ownerPool = pool;
    }

    public GameObjectTypePool<RequestUIElement> GetPool()
    {
        return ownerPool;
    }
}
