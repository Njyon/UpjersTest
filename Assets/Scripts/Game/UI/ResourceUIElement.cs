using UnityEngine;
using UnityEngine.UI;

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
