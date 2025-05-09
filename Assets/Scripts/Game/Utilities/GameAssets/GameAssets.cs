using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class GameAssets : MonoBehaviour
{
	private static GameAssets instance;
	public static GameAssets Instance
	{
		get
		{
			if (instance == null)
			{
				//  Path: "Assets/Prefab/Resources/GameAssets"
				instance = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
				instance.name = ">> " + instance.name;
			}
			return instance;
		}
	}

	[SerializeField] private GameAssetsUI uiInstance;
	public GameAssetsUI UI
	{
		get
		{
			if (uiInstance == null)
			{
				uiInstance = GetComponent<GameAssetsUI>();
				if (uiInstance == null)
				{
					string debugString = Ultra.Utilities.Instance.DebugErrorString("GameAssets", "uiInstance", "Ui Instance was NULL Missing GameAssetsUI on GameAssets!");
					Debug.LogError(debugString);
				}
			}
			return uiInstance;
		}
	}

	// List of Objects below
}