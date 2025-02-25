using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC : MonoBehaviour
{
    //TILEMANAGER COMPONENT
    TileManager manager;
	Queue<WFC2Tile> tileQueue;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<TileManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Add(WFC2Tile tile)
	{
		tileQueue.Enqueue(tile);
	}
	// [System.Serializable]
    // public class Tile
    // {
    //     public GameObject prefab;
    //     public List<string> compatibleNeighborsUp;
    //     public List<string> compatibleNeighborsDown;
    //     public List<string> compatibleNeighborsLeft;
    //     public List<string> compatibleNeighborsRight;
    // }

    // public Tile[] tileTypes;
    public int width = 10;
    public int height = 10;
    public Vector3 startPosition = Vector3.zero;
    public float tileSpacing = 1f;

    private Tile?[][] grid; // Nullable Tile, represents possible tile states

    void Start()
    {
        InitializeGrid();
        Collapse();
    }

    void InitializeGrid()
    {
        grid = new Tile?[width][];
        for (int x = 0; x < width; x++)
        {
            grid[x] = new Tile?[height];
            for (int y = 0; y < height; y++)
            {
                grid[x][y] = null; // Initially, all cells are uncollapsed and have all possibilities.
            }
        }
    }

    void Collapse()
    {
        List<(int x, int y)> uncollapsedCells = new List<(int x, int y)>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                uncollapsedCells.Add((x, y));
            }
        }

        while (uncollapsedCells.Count > 0)
        {
            (int x, int y) = FindLowestEntropy(uncollapsedCells);
            CollapseCell(x, y);
            UpdateNeighbors(x, y);
            uncollapsedCells.Remove((x, y));
        }

        InstantiateGrid();
    }

    (int x, int y) FindLowestEntropy(List<(int x, int y)> uncollapsedCells)
    {
        (int x, int y) lowestEntropyCell = uncollapsedCells[0];
        int lowestEntropy = int.MaxValue;

        foreach ((int x, int y) cell in uncollapsedCells)
        {
            int entropy = CalculateEntropy(cell.x, cell.y);
            if (entropy < lowestEntropy)
            {
                lowestEntropy = entropy;
                lowestEntropyCell = cell;
            }
        }
        return lowestEntropyCell;
    }

    int CalculateEntropy(int x, int y)
    {
        if (grid[x][y] != null) return int.MaxValue; // Already collapsed.

        List<Tile> possibleTiles = GetPossibleTiles(x, y);
        return possibleTiles.Count;
    }

    void CollapseCell(int x, int y)
    {
        List<Tile> possibleTiles = GetPossibleTiles(x, y);
        if (possibleTiles.Count == 0)
        {
            Debug.LogError("No possible tiles at (" + x + ", " + y + "). Wave function collapse failed.");
            return;
        }

        Tile chosenTile = possibleTiles[Random.Range(0, possibleTiles.Count)];
        grid[x][y] = chosenTile;
    }

    List<Tile> GetPossibleTiles(int x, int y)
    {
        if (grid[x][y] != null) return new List<Tile> { grid[x][y].Value }; // Already collapsed.

        List<Tile> possibleTiles = tileTypes.ToList();

        // Check Neighbors
        if (y > 0 && grid[x][y - 1] != null) // Down
        {
            possibleTiles = possibleTiles.Where(tile => grid[x][y - 1].Value.compatibleNeighborsUp.Contains(tile.prefab.name)).ToList();
        }
        if (y < height - 1 && grid[x][y + 1] != null) // Up
        {
            possibleTiles = possibleTiles.Where(tile => grid[x][y + 1].Value.compatibleNeighborsDown.Contains(tile.prefab.name)).ToList();
        }
        if (x > 0 && grid[x - 1][y] != null) // Left
        {
            possibleTiles = possibleTiles.Where(tile => grid[x - 1][y].Value.compatibleNeighborsRight.Contains(tile.prefab.name)).ToList();
        }
        if (x < width - 1 && grid[x + 1][y] != null) // Right
        {
            possibleTiles = possibleTiles.Where(tile => grid[x + 1][y].Value.compatibleNeighborsLeft.Contains(tile.prefab.name)).ToList();
        }

        return possibleTiles;
    }

    void UpdateNeighbors(int x, int y)
    {
        // Add logic to propagate constraints if needed. For simple cases, this is not required.
    }

    void InstantiateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile? tile = grid[x][y];
                if (tile != null)
                {
                    Vector3 position = startPosition + new Vector3(x * tileSpacing, 0, y * tileSpacing);
                    Instantiate(tile.Value.prefab, position, Quaternion.identity);
                }
            }
        }
    }
}
