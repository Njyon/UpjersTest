using System;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedQueue<T> where T : class
{
	private LinkedList<T> _list = new LinkedList<T>();


	/// <summary>
	/// Count of the Queue
	/// </summary>
	public int Count => _list.Count;

	/// <summary>
	/// Add the object to the Queue (Last position)
	/// </summary>
	/// <param name="item">The to be added object</param>
	public void Enqueue(T item)
	{
		_list.AddLast(item);
	}

	/// <summary>
	/// Remove the first object in the Queue
	/// </summary>
	/// <returns>index 0 of the queue (can be NULL!)</returns>
	public T Dequeue()
	{
		if (_list.Count <= 0)
		{
			Debug.Log("Queue is Empty.");
			return null;
		}

		var value = _list.First.Value;
		_list.RemoveFirst();
		return value;
	}

	/// <summary>
	/// Check whats the first object in the Queue without removing it
	/// </summary>
	/// <returns>index 0 of the queue (can be NULL!)</returns>
	public T Peek()
	{
		if (_list.Count == 0)
		{
			Debug.Log("Queue is Empty.");
			return null;
		}

		return _list.First.Value;
	}

	/// <summary>
	/// Removes the object from the Queue
	/// </summary>
	/// <param name="item">The object to be removed</param>
	/// <returns>Wether the object could be removed successfully or not</returns>
	public bool Remove(T item)
	{
		return _list.Remove(item);
	}

	/// <summary>
	/// Removes the object at the index from the Queue
	/// </summary>
	/// <param name="item">The index to be removed</param>
	/// <returns>Wether the object could be removed successfully or not</returns>
	public bool RemoveAt(int index)
	{
		T item;
		return RemoveAt(index, out item);
	}

	/// <summary>
	/// Removes the object at the index from the Queue
	/// </summary>
	/// <param name="index">The index to be removed</param>
	/// <param name="item">The Removed Item</param>
	/// <returns>Wether the object could be removed successfully or not</returns>
	public bool RemoveAt(int index, out T item)
	{
		item = null;
		if (index < 0 || index >= _list.Count)
		{
			Debug.Log("Index was out of Range");
			return false;
		}

		var current = _list.First;
		for (int i = 0; i < index; i++)
		{
			current = current.Next;
		}

		item = current.Value;
		_list.Remove(current);
		return true;
	}

	/// <summary>
	/// Return Queue as List
	/// </summary>
	/// <returns>The List with the current state of the Queue</returns>
	public List<T> ToList()
	{
		return new List<T>(_list);
	}
}
