using System.Collections.Generic;
using UnityEngine;

public enum TowerBuildingState
{
    TryToBuild,
    Blocked,
    Build
}

public class Tower : MonoBehaviour
{
    // Serialized for debugging in runtime
    [SerializeField] List<Material> materials;
    [SerializeField] Color buildingColor;
    [SerializeField] Color cantBuildColor;

    TowerBuildingState buildingState;
    public TowerBuildingState BuildingState
    {
        get { return buildingState; }
        set
        {
            switch (value)
            {
                case TowerBuildingState.TryToBuild:
                    if (buildingState != value)
                        MakeTowerTransparent();
                    break;
                case TowerBuildingState.Blocked:
                    if (buildingState != value)
                        MakeTowerTileIsBlockedColor();
                    break;
                case TowerBuildingState.Build:
                    if (buildingState != value)
                        ResetColor();
                    break;
                default:
                    Ultra.Utilities.Instance.DebugErrorString("Tower", "BuildingState", "BuildingState not Implemented");
                    break;
            }

            buildingState = value;
        }
    }

    void Awake()
    {
        ChacheMaterials();
    }

    void ChacheMaterials()
    {
        materials.Clear();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat != null && !materials.Contains(mat))
                {
                    materials.Add(mat);
                }
            }
        }
    }

    // Needed for building for clearer behaviour
    public void MakeTowerTransparent()
    {
        foreach (Material material in materials) 
        { 
            material.SetColor("_BaseColor", buildingColor);
        }
    }

    public void MakeTowerTileIsBlockedColor()
    {
        foreach (Material material in materials)
        {
            material.SetColor("_BaseColor", cantBuildColor);
        }
    }

    public void ResetColor()
    {
        foreach (Material material in materials)
        {
            material.SetColor("_BaseColor", Color.white);
        }
    }
}
