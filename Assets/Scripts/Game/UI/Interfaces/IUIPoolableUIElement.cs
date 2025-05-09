using UnityEngine;

public interface IUIPoolableUIElement<T> where T : MonoBehaviour
{
    public void SavePool(GameObjectTypePool<T> pool);
    public GameObjectTypePool<T> GetPool();
}
