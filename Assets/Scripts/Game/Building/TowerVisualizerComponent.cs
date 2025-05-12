using System.Collections.Generic;
using UnityEngine;

public class TowerVisualizerComponent 
{
    Tower tower;

    List<Material> materials;
    public TowerVisualizerComponent(Tower tower)
    {
        this.tower = tower;
        materials = new List<Material>();
    }

    public void ChacheMaterials()
    {
        materials.Clear();

        Renderer[] renderers = tower.GetComponentsInChildren<Renderer>();

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
            material.SetColor("_BaseColor", tower.buildingColor);
        }
    }

    public void MakeTowerTileIsBlockedColor()
    {
        foreach (Material material in materials)
        {
            material.SetColor("_BaseColor", tower.cantBuildColor);
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
