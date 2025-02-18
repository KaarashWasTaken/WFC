using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
	//List of tile positions
	public List<Vector2> TileMap = new List<Vector2>();
	//Next tile's X position
	float NextTilePosX = 0.0f;
	//Next tile's Y position
	float NextTilePosY = 0.0f;
	Vector3 nextRotation = new Vector3(0,0,90);
	// The size of the map used as: MapSize x MapSize
	public int MapSize = 10;
	//A list of the tiles prefabs
	// public List<GameObject> TilesPrefabs;
	//Tiles placed in the world
	public List<WFCTile> PlacedTiles = new();
	//The gameobject that should parent the tiles
	public GameObject TilesParent;
	public WFCTile tileObj;
	bool fullySpawned = false;
	public bool stepByStep = false;
	int tilePosIndex = 0;
	int tilesPerFrame = 1;

    // Start is called before the first frame update
    void Start()
    {
		MapSize = 10;
		GenerateMap();
    }

    // Update is called once per frame
    void Update() 
	{
		for (int i = 0; i < tilesPerFrame; i++)
		 {
			if (!fullySpawned && stepByStep)
			{
				StepGenerateMap();
			}
		 }
	}

	public void GenerateMap()
	{
		TileMap.Clear();
        for (int i = 1; i <= MapSize * MapSize; i++)
		{
			Vector2 TilePos = Vector2.zero;
			TilePos.x = NextTilePosX;
			TilePos.y = NextTilePosY;
			NextTilePosX += 1;
			// Increase Y
			if (i % MapSize == 0 && i != 0)
			{
				NextTilePosX = 0.0f;
				NextTilePosY += 1;
			}
			TileMap.Add(TilePos);
		}
		//Spawn all tiles
		for (int i = 0; i < TileMap.Count; i++)
		{
			Quaternion quat = new(0, 0, 0, 1);
			nextRotation.z = UnityEngine.Random.Range(0, 3) * 90;
			quat.eulerAngles = nextRotation;
			// GameObject tile = Instantiate(TilesPrefabs[UnityEngine.Random.Range(0,6)], TilesParent);
			WFCTile tile = Instantiate(tileObj, new Vector3(TileMap[i].x, TileMap[i].y, 0), Quaternion.identity);
			tile.transform.SetParent(TilesParent.transform);
			tile.SetManager(this);
			PlacedTiles.Add(tile);
		}
		fullySpawned = true;
		
	}
	public void RegenerateMap()
	{
		foreach (WFCTile WFCtile in PlacedTiles)
		{
			Destroy(WFCtile.gameObject);
		}
		PlacedTiles.Clear();
		TileMap.Clear();
		tilePosIndex = 1;
		NextTilePosX = 0.0f;
		NextTilePosY = 0.0f;
		if (stepByStep)
		{
			fullySpawned = false;
			return;
		}
		GenerateMap();
	}
	public void StepGenerateMap()
	{
        if (tilePosIndex <= MapSize*MapSize)
		{
			Vector2 TilePos = Vector2.zero;
			TilePos.x = NextTilePosX;
			TilePos.y = NextTilePosY;
			NextTilePosX += 1;
			// Increase Y
			if (tilePosIndex % MapSize == 0 && tilePosIndex != 0)
			{
				print(tilePosIndex);
				NextTilePosX = 0.0f;
				NextTilePosY += 1;
			}
			TileMap.Add(TilePos);
		}

		//Spawn next tile
		WFCTile tile = Instantiate(tileObj, new Vector3 (TileMap[tilePosIndex-1].x, TileMap[tilePosIndex-1].y, 0), Quaternion.identity);
		tile.transform.SetParent(TilesParent.transform);
		tile.SetManager(this);
		PlacedTiles.Add(tile);
		tilePosIndex ++;

		if (PlacedTiles.Count == MapSize * MapSize)
		{
			fullySpawned = true;
		}
	}
}
