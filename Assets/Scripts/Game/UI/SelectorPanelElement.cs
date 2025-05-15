using System.Collections.Generic;
using UnityEngine;

public class SelectorPanelElement : MonoBehaviour, IUIElement, IUIPoolableUIElement<SelectorPanelElement>
{
    /// <summary>
    /// Parent of the Buttons inside the Panel
    /// </summary>
    [SerializeField] GameObject contentGameObject;
    public Transform ContentHolder
    {
        get
        {
            return contentGameObject.transform;
        }
    }

    private List<IUIElement> uiElements;
    /// <summary>
    /// IUIElement the panel holds
    /// </summary>
    public List<IUIElement> UIElements
    {
        get { return uiElements; }
    }

    /// <summary>
    /// Panel pool
    /// </summary>
    GameObjectTypePool<SelectorPanelElement> ownerPool;

    public void SetupSelectorPanelElement()
    {
        uiElements = new List<IUIElement>();
    }

    /// <summary>
    /// Cleaup all Childs & Return it self back to its pool
    /// </summary>
    public void Cleanup()
    {
        foreach (var uiElement in uiElements)
        {
            uiElement.Cleanup();
        }
        GetPool().ReturnValue(this);
    }

    public void SavePool(GameObjectTypePool<SelectorPanelElement> pool)
    {
        ownerPool = pool;
    }

    public GameObjectTypePool<SelectorPanelElement> GetPool()
    {
        return ownerPool;
    }
}
