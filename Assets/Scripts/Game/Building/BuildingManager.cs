using System;
using System.Collections.Generic;
using Ultra;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Building Manager to handle creating towers on the Grid
/// </summary>
public class BuildingManager : MonoSingelton<BuildingManager>
{
    public Action onTowerBuild;
    public Action onTowerBuildCancled;
    public Action onTowerReplace;
    public Action onTowerReplaceCanceled;

    /// <summary>
    /// Parent Object of the Towers so we dont spam the Inspector
    /// </summary>
    [SerializeField] Transform worldTransform;
    bool tryingToBuild = false;
    bool tryingToReplace = false;
    Tower cachedBuildingTower;
    Vector2Int cachedTowerSize;
    GridTile cachedHitGridTile = null;
    string towerNameFlag = "Try Building ";
    SelectionManager selectionManager;
    public Action cancleBuild;
    public List<GridTile> cachedTowerPlacedGridTiles;

    public bool TryingToBuild
    {
        get { return tryingToBuild; }
        private set {
            tryingToBuild = value; 
            if (!tryingToBuild)
            {
                cachedHitGridTile = null;
            }
        }
    }
    public bool TryingToReplace
    {
        get { return tryingToReplace; }
        private set
        {
            tryingToReplace = value;
            if (!tryingToReplace)
            {
                cachedHitGridTile = null;
            }
        }
    }

    void Awake()
    {
        selectionManager = SelectionManager.Instance;
        selectionManager.selectionEvent += OnSelect;
        selectionManager.contextEvent += OnContextAction;

        cachedTowerPlacedGridTiles = new List<GridTile>();
    }

    void OnDestroy()
    {
        if (selectionManager != null)
        {
            selectionManager.selectionEvent -= OnSelect;
            selectionManager.contextEvent -= OnContextAction;
        }
        TryingToBuild = false;
    }

    public void TryToBuildTower(Tower towerPrefab, Vector2Int towerSize)
    {
        cachedBuildingTower = Instantiate(towerPrefab, worldTransform);
        cachedBuildingTower.gameObject.name = towerNameFlag + cachedBuildingTower.gameObject.name;
        cachedBuildingTower.BuildingState = TowerBuildingState.TryToBuild;
        cachedBuildingTower.towerSize = towerSize;
        cachedTowerSize = cachedBuildingTower.towerSize;

        TryingToBuild = true;
    }

    void Update()
    {
        if (TryingToBuild || TryingToReplace)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
            foreach (RaycastHit hit in hits) 
            {
                GameObject go = hit.collider.gameObject;
                GridTile gridTile = go.GetComponent<GridTile>();
                // should probably use layers with casting on the tile but good enough for now
                if (gridTile == null) continue;
                if (gridTile == cachedHitGridTile) break;
                if (cachedBuildingTower != null)
                {
                    List<Vector3> tilePositions = new List<Vector3>();
                    cachedTowerPlacedGridTiles.Clear();

                    for (int x = 0; x < cachedTowerSize.x; x++)
                    {
                        for (int y = 0; y < cachedTowerSize.y; y++)
                        {
                            // possitions can be outside the grid so we guess the position for visualization purpose
                            Vector3 possibleTilePos = gridTile.transform.position + new Vector3((GridManager.Instance.TileSize.x * x) + GridManager.Instance.padding * x, 0, (GridManager.Instance.TileSize.z * y) + GridManager.Instance.padding * y);
                            tilePositions.Add(possibleTilePos);
                            if (GridManager.Instance.gridWidth > gridTile.GridPos.x + x && GridManager.Instance.gridHeight > gridTile.GridPos.y + y)
                            {
                                GridTile tile = GridManager.Instance.GridTiles[gridTile.GridPos.x + x, gridTile.GridPos.y + y];
                                cachedTowerPlacedGridTiles.Add(tile);
                            }
                            else
                            {
                                cachedTowerPlacedGridTiles.Add(null);
                            }
                        }
                    }

                    Vector3 TowerPosition = GetTowerPosition(tilePositions); ;
                    float gridHalfHight = GridManager.Instance.TileSize.y / 2;
                    TowerPosition.y += gridHalfHight;
                    cachedBuildingTower.transform.position = TowerPosition;

                    bool canPlaceTomwer = true;
                    foreach (var tile in cachedTowerPlacedGridTiles)
                    {
                        if (tile == null || tile.IsBlocked)
                        {
                            canPlaceTomwer = false;
                            cachedBuildingTower.BuildingState = TowerBuildingState.Blocked;
                            break;
                        }   
                    }

                    if (canPlaceTomwer)
                        cachedBuildingTower.BuildingState = TowerBuildingState.TryToBuild;

                    cachedHitGridTile = gridTile;

                    break;
                }
            }
        }
    }

    Vector3 GetTowerPosition(List<Vector3> tilePositions)
    {
        Vector3 averagePos = Vector3.zero;
        foreach (Vector3 pos in tilePositions)
        {
            averagePos += pos;
        }
        averagePos /= tilePositions.Count;
        return averagePos;
    }

    void OnSelect(List<ISelectable> newSelection, List<ISelectable> oldSelection)
    {
        if (TryingToBuild || TryingToReplace) 
        {
            Build();
        }
    }

    void OnContextAction(IContextAction newContext, IContextAction oldContext)
    {
        if (TryingToBuild)
            CancleBuild();
        if (TryingToReplace)
            CancelTryToReplace();
    }

    void Build()
    {
        if (cachedBuildingTower != null)
        {
            foreach (GridTile tile in cachedTowerPlacedGridTiles)
            {
                if (tile.IsBlocked) return;
            }

            cachedBuildingTower.BuildingState = TowerBuildingState.Build;
            cachedBuildingTower.gameObject.name = cachedBuildingTower.gameObject.name.Substring(towerNameFlag.Length);
            cachedBuildingTower.gridTilesTheTowerIsBuildOn = new List<GridTile>(cachedTowerPlacedGridTiles);
            foreach (GridTile tile in cachedBuildingTower.gridTilesTheTowerIsBuildOn)
            {
                tile.IsBuildOn = true;
            }

            cachedBuildingTower = null;
            cachedTowerPlacedGridTiles.Clear();

            TryingToBuild = false;
            TryingToReplace = false;

            if (onTowerBuild != null) onTowerBuild.Invoke();
        }
    }

    void CancleBuild()
    {
        if (TryingToBuild && cachedBuildingTower != null)
        {
            Destroy(cachedBuildingTower.gameObject);
            cachedBuildingTower = null;
            TryingToBuild = false;
            GameManager.Instance.ResourceAccountant.RemoveLastTransaction();

            if (cancleBuild != null) cancleBuild.Invoke();

            if (onTowerBuildCancled != null) onTowerBuildCancled.Invoke();
        }
    }

    void CancelTryToReplace()
    {
        if (TryingToReplace && cachedBuildingTower != null)
        {
            cachedBuildingTower.transform.position = cachedBuildingTower.lastPlacedPosition;
            cachedBuildingTower.BuildingState = TowerBuildingState.Build;

            foreach (GridTile tile in cachedBuildingTower.gridTilesTheTowerIsBuildOn)
            {
                tile.IsBuildOn = true;
            }

            cachedBuildingTower = null;
            TryingToReplace = false;

            if (onTowerReplaceCanceled != null) onTowerReplaceCanceled.Invoke();
        }
    }

    public void ReplaceTower(Tower towerToReplace)
    {
        if (TryingToBuild)
        {
            CancleBuild();
        }
        TryingToReplace = true;
        cachedBuildingTower = towerToReplace;
        cachedBuildingTower.gameObject.name = towerNameFlag + cachedBuildingTower.gameObject.name;
        cachedBuildingTower.BuildingState = TowerBuildingState.Replace;
        cachedTowerSize = cachedBuildingTower.towerSize;

        foreach (GridTile tile in cachedBuildingTower.gridTilesTheTowerIsBuildOn)
        {
            tile.IsBuildOn = false;
        }
        if (onTowerReplace != null) onTowerReplace.Invoke();
    }
}
