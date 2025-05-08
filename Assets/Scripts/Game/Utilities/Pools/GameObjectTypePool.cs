using Unity.VisualScripting;
using UnityEngine;

public class GameObjectTypePool<T> : PoolBase<T> where T : MonoBehaviour
{
	T instance;
	GameObject parent;
	public GameObject Parent { get { return parent; } }

	public GameObjectTypePool(T instance, GameObject parent) : base()
	{
		this.instance = instance;
		this.parent = parent;
	}

	public GameObjectTypePool(T instance, GameObject parent, int size) : base(size)
	{
		this.instance = instance;
		this.parent = parent;
	}

	void SetupName(GameObject g)
	{
		g.name = ">> " + this.instance.name; // for Hirachy Visualization
	}

	public override T GetValue()
	{
		if (!IsTStackInit || ElementsInStackAreNull()) InitStack();

		if (NoMoreTInStack)
			SpawnValue();

		T t = stack.Pop();
		ActivateValue(t);
		return t;
	}

	protected override void ActivateValue(T value)
	{
		value.gameObject.SetActive(true);
	}

	protected override void DeactivateValue(T value)
	{
		value.transform.SetParent(Parent.transform, false);		// Bring them back to parent so they not get exedently Destroyed
		value.gameObject.SetActive(false);
	}

	protected override void DestroyElement(T element)
	{
		GameObject.Destroy(element);
	}

	protected override void SpawnValue()
	{
		T t = GameObject.Instantiate(instance, Parent.transform);
		SetupName(t.gameObject);
		stack.Push(t);
		DeactivateValue(t);
	}
}
