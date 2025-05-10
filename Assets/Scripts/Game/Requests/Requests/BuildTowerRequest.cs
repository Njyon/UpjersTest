using System;
using UnityEngine;

[Serializable]
public class BuildTowerRequest : ARequest
{
    public Tower TowerPrefab;
    public Vector2Int Size;

    public override void ExecuteRequest(IRequestOwner owner)
    {
        BuildingManager.Instance.TryToBuildTower(TowerPrefab, Size);
    }
}
