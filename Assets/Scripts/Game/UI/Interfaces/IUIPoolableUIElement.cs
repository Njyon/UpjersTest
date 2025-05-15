using UnityEngine;

public interface IUIPoolableUIElement<T> where T : MonoBehaviour
{
    /// <summary>
    /// Safe the pool where the object comes from so it can return itself later
    /// </summary>
    /// <param name="pool"> pool the objects origins from </param>
    public void SavePool(GameObjectTypePool<T> pool);

    /// <summary>
    /// Get the pool the object comes from
    /// </summary>
    /// <returns></returns>
    public GameObjectTypePool<T> GetPool();
}
