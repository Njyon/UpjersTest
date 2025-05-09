using System.Collections.Generic;
using Ultra;
using UnityEngine;

public class BuildingManager : MonoSingelton<BuildingManager>
{
    [SerializeField] Transform worldTransform;
    bool tryingToBuild = false;
    Tower cachedBuildingTower;

    void Start()
    {
        SelectionManager.Instance.selectionEvent += OnSelect;
    }

    void OnDestroy()
    {
        SelectionManager.Instance.selectionEvent -= OnSelect;
        tryingToBuild = false;
    }

    public void TryToBuildTower(Tower towerPrefab, Vector2 towerSize)
    {
        cachedBuildingTower = Instantiate(towerPrefab, worldTransform);
        cachedBuildingTower.gameObject.name = "Try Building " + cachedBuildingTower.gameObject.name;
        cachedBuildingTower.BuildingState = TowerBuildingState.TryToBuild;

        tryingToBuild = true;
        
    }

    void Update()
    {
        if (tryingToBuild)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                GameObject tile = hit.collider.gameObject;
                GridTile gridTile = tile.GetComponent<GridTile>();
                if (gridTile != null && cachedBuildingTower != null)
                {
                    Vector3 gridPos = gridTile.transform.position;
                    float gridHalfHight = GridManager.Instance.GridSize.y / 2;
                    gridPos.y += gridHalfHight;
                    cachedBuildingTower.transform.position = gridPos;

                    if (gridTile.IsBlocked)
                        cachedBuildingTower.BuildingState = TowerBuildingState.Blocked;
                    else
                        cachedBuildingTower.BuildingState = TowerBuildingState.TryToBuild;
                }
            }
        }
    }

    void OnSelect(List<ISelectable> newSelection, List<ISelectable> oldSelection)
    {
        if (tryingToBuild) 
        {
            Build();
        }
    }

    void Build()
    {

    }
}
