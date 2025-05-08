using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    public GridTile gridTile;
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float padding = 0.2f;

    [Header("Path")]
    private GridTile[,] gridTiles;
    private List<Vector2Int> path = new List<Vector2Int>();
    [SerializeField] private Color pathColor = new Color(0.5f, 0.25f, 0f);

    void Start()
    {
        SpawnGrid();
    }

    void SpawnGrid()
    {
        if (gridTile == null)
        {
            Debug.LogError("Cube Prefab is not assigned.");
            return;
        }

        // Check Size 
        Vector3 cubeSize = GetPrefabSize(gridTile);

        // Center Grind
        Vector3 startPos = transform.position - new Vector3((gridWidth - 1) * (cubeSize.x + padding) / 2f, 0, (gridHeight - 1) * (cubeSize.z + padding) / 2f);

        gridTiles = new GridTile[gridWidth, gridHeight];

        CreateGrid(cubeSize, startPos);

        GeneratePath();
    }

    void CreateGrid(Vector3 cubeSize, Vector3 startPos)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 spawnPos = startPos + new Vector3( x * (cubeSize.x + padding), 0, y * (cubeSize.z + padding));

                GridTile gridTileObj = Instantiate(gridTile, spawnPos, Quaternion.identity, transform);
                gridTiles[x, y] = gridTileObj;
            }
        }
    }

    void GeneratePath()
    {
        path.Clear();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        int startY = Random.Range(0, gridHeight);
        int endY = Random.Range(0, gridHeight);
        Vector2Int current = new Vector2Int(0, startY);
        Vector2Int end = new Vector2Int(gridWidth - 1, endY);

        path.Add(current);
        visited.Add(current);

        while (current.x < gridWidth - 1)
        {
            List<Vector2Int> possibleSteps = new List<Vector2Int>();

            Vector2Int right = current + Vector2Int.right;
            Vector2Int up = current + Vector2Int.up;
            Vector2Int down = current + Vector2Int.down;

            if (IsInsideGrid(right) && !visited.Contains(right)) possibleSteps.Add(right);
            if (IsInsideGrid(up) && !visited.Contains(up)) possibleSteps.Add(up);
            if (IsInsideGrid(down) && !visited.Contains(down)) possibleSteps.Add(down);

            if (possibleSteps.Count == 0) break; // Deadend

            // Get random next step
            Vector2Int next = possibleSteps[Random.Range(0, possibleSteps.Count)];
            current = next;
            visited.Add(current);
            path.Add(current);

            gridTiles[current.x, current.y].IsPath = true;
        }

        // Connect to end point
        if (current.x == gridWidth - 1 && current.y != end.y)
        {
            int direction = (end.y > current.y) ? 1 : -1;
            while (current.y != end.y)
            {
                current = new Vector2Int(current.x, current.y + direction);
                if (!visited.Contains(current))
                {
                    path.Add(current);
                    gridTiles[current.x, current.y].IsPath = true;
                    visited.Add(current);
                }
            }
        }

        ColorPath();
    }

    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    void ColorPath()
    {
        foreach (Vector2Int pos in path)
        {
            GridTile cube = gridTiles[pos.x, pos.y];
            cube.GetComponent<Renderer>().material.color = pathColor;
        }
    }

    Vector3 GetPrefabSize(GridTile prefab)
    {
        GridTile temp = Instantiate(prefab);
        Vector3 size = temp.GetComponent<Renderer>().bounds.size;
        Destroy(temp.gameObject);
        return size;
    }
}
