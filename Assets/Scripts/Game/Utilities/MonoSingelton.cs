using UnityEngine;

namespace Ultra
{
	/// <summary>
	/// Singelton for Monobehaviours. Usefull for GameObjects that should be interactable via Inspector out of Playmode
	/// (Technicly not safe, user can add more that 1 Monobehaviour at the scene)
	/// </summary>
	/// <typeparam name="T"> The Monobehaviour that should be Singelt </typeparam>
	public class MonoSingelton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static bool shuttingDown = false;
		private static object lockObj = new object();
		private static T instance;

		public delegate void OnObjectChanged(T newObject, T oldObject);
		public static OnObjectChanged onObjectChanged;

		public static T Instance
		{
			get
			{
				if (shuttingDown)
				{
					Debug.LogError("[Singelton::Get] Instance '" + typeof(T) + "' destroyed!");
					return null;
				}
				lock (lockObj)
				{
					if (!instance) instance = (T)FindFirstObjectByType(typeof(T));
					if (!instance)
					{
						var singletonObj = new GameObject();
						instance = singletonObj.AddComponent<T>();
						singletonObj.name = typeof(T).ToString() + " (Singelton)";
						DontDestroyOnLoad(singletonObj);
					}
				}
				return instance;
			}
			set
			{
				var oldObject = instance;
				instance = value;
				if (onObjectChanged != null) onObjectChanged(instance, oldObject);

			}
		}

		private void OnDestroy()
		{
			Instance = null;
		}
	}
}
