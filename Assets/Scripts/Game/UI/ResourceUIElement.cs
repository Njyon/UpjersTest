using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pooled Resource Element is used to visualise Resources (Value + Image)
/// </summary>
public class ResourceUIElement : MonoBehaviour, IUIElement, IUIPoolableUIElement<ResourceUIElement>
{
    [SerializeField] Image resourceImage;
    [SerializeField] TMPro.TextMeshProUGUI resourceText;

    GameObjectTypePool<ResourceUIElement> ownerPool;

    public void SetupResourceElement(Sprite resourceSprit, string costAmountString)
    {
        resourceImage.sprite = resourceSprit;
        resourceText.text = costAmountString;
    }

    /// <summary>
    /// Remove Data and return to Pool
    /// </summary>
    public void Cleanup()
    {
        resourceImage.sprite = null;
        resourceText.text = "";

        GetPool().ReturnValue(this);
    }

    public void SavePool(GameObjectTypePool<ResourceUIElement> pool)
    {
        ownerPool = pool;
    }

    public GameObjectTypePool<ResourceUIElement> GetPool()
    {
        return ownerPool;
    }
}
