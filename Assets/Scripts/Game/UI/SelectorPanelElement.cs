using System.Collections.Generic;
using UnityEngine;

public class SelectorPanelElement : MonoBehaviour, IUIElement, IUIPoolableUIElement<SelectorPanelElement>
{
    [SerializeField] GameObject contentGameObject;
    public Transform ContentHolder
    {
        get
        {
            return contentGameObject.transform;
        }
    }

    private List<IUIElement> uiElements;
    public List<IUIElement> UIElements
    {
        get { return uiElements; }
    }

    GameObjectTypePool<SelectorPanelElement> ownerPool;

    public void SetupSelectorPanelElement()
    {
        uiElements = new List<IUIElement>();
    }

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
