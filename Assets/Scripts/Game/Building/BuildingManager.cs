using System.Collections.Generic;
using Ultra;
using UnityEngine;

public class BuildingManager : MonoSingelton<BuildingManager>
{
    [SerializeField] Transform worldTransform;
    bool tryingToBuild = false;
    Tower cachedBuildingTower;
    Vector2Int cachedTowerSize;
    GridTile cachedHitGridTile = null;
    string towerNameFlag = "Try Building ";

    bool TryingToBuild
    {
        get { return tryingToBuild; }
        set {
            tryingToBuild = value; 
            if (!tryingToBuild)
            {
                cachedHitGridTile = null;
            }
        }
    }

    void Start()
    {
        SelectionManager.Instance.selectionEvent += OnSelect;
    }

    void OnDestroy()
    {
        SelectionManager.Instance.selectionEvent -= OnSelect;
        TryingToBuild = false;
    }

    public void TryToBuildTower(Tower towerPrefab, Vector2Int towerSize)
    {
        cachedBuildingTower = Instantiate(towerPrefab, worldTransform);
        cachedBuildingTower.gameObject.name = towerNameFlag + cachedBuildingTower.gameObject.name;
        cachedBuildingTower.BuildingState = TowerBuildingState.TryToBuild;
        cachedTowerSize = towerSize;

        TryingToBuild = true;
        
    }

    void Update()
    {
        if (TryingToBuild)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
                    List<GridTile> gridTiles = new List<GridTile>();

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
                                gridTiles.Add(tile);
                            }
                            else
                            {
                                gridTiles.Add(null);
                            }
                        }
                    }

                    Vector3 TowerPosition = GetTowerPosition(tilePositions); ;
                    float gridHalfHight = GridManager.Instance.TileSize.y / 2;
                    TowerPosition.y += gridHalfHight;
                    cachedBuildingTower.transform.position = TowerPosition;

                    bool canPlaceTomwer = true;
                    foreach (var tile in gridTiles)
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
        if (TryingToBuild) 
        {
            Build();
        }
    }

    void Build()
    {
        if (cachedBuildingTower != null)
        {
            cachedBuildingTower.BuildingState = TowerBuildingState.Build;
            cachedBuildingTower.gameObject.name = cachedBuildingTower.gameObject.name.Substring(towerNameFlag.Length);
            cachedBuildingTower = null; 
        }
    }
}
