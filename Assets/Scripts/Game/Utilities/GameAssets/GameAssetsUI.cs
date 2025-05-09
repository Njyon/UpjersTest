using System;
using UnityEngine;

public class GameAssetsUI : MonoBehaviour
{
	[Header("Prefabs")]
	[SerializeField] SelectorPanelElement selectorPanelElement;
	[SerializeField] RequestUIElement requestUIElement;
	[SerializeField] ResourceUIElement resourceUIElement;

	GameObjectTypePool<SelectorPanelElement> selectorPanelElementPool;
	public GameObjectTypePool<SelectorPanelElement> SelectorPanelElementPool
	{ 
		get 
		{ 
			if (selectorPanelElementPool == null)
			{
				selectorPanelElementPool = new GameObjectTypePool<SelectorPanelElement>(selectorPanelElement, Ultra.Utilities.Instance.CreateHolderChild("SelectorPanelPoolHolder"), 1);
			}
			return selectorPanelElementPool; 
		} 
	}
	GameObjectTypePool<RequestUIElement> requestUIElementPool;
	public GameObjectTypePool<RequestUIElement> RequestUIElementPool
	{
		get
		{
			if (requestUIElementPool == null)
			{
				requestUIElementPool = new GameObjectTypePool<RequestUIElement>(requestUIElement, Ultra.Utilities.Instance.CreateHolderChild("RequestUIElementPoolHolder"));
			}
			return requestUIElementPool;
		}
	}
	GameObjectTypePool<ResourceUIElement> resourceUIElementPool;
	public GameObjectTypePool<ResourceUIElement> ResourceUIElementPool
	{
		get
		{
			if (resourceUIElementPool == null)
			{
				resourceUIElementPool = new GameObjectTypePool<ResourceUIElement>(resourceUIElement, Ultra.Utilities.Instance.CreateHolderChild("RequestUIElementPoolHolder"));
			}
			return resourceUIElementPool;
		}
	}
}
