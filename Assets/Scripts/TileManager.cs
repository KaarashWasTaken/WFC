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
	float NextTilePosX = 0.5f;
	//Next tile's Y position
	float NextTilePosY = 0.5f;
	Vector3 nextRotation = new Vector3(0,0,90);
	// The size of the map used as: MapSize x MapSize
	public int MapSize = 10;
	//A list of the tiles prefabs
	public List<GameObject> TilesPrefabs;
	//Tiles placed in the world
	public List<GameObject> PlacedTiles = new();
	//The gameobject that should parent the tiles
	public GameObject TilesParent;

    // Start is called before the first frame update
    void Start()
    {
		GenerateMap();
    }

    // Update is called once per frame
    void Update() 
	{
	}

	public void GenerateMap()
	{
		if (PlacedTiles.Count > 0)
		{
			foreach (GameObject tile in PlacedTiles)
			{
				Destroy(tile);
			}
			PlacedTiles.Clear();
		}
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
				NextTilePosX = 0.5f;
				NextTilePosY += 1;
			}
			TileMap.Add(TilePos);
		}
		NextTilePosX = 0.5f;
		NextTilePosY = 0.5f;
		//Spawn all tiles
		for (int i = 0; i < TileMap.Count; i++)
		{
			Quaternion quat = new(0, 0, 0, 1);
			nextRotation.z = UnityEngine.Random.Range(0,3) * 90;
			quat.eulerAngles = nextRotation;
			// GameObject tile = Instantiate(TilesPrefabs[UnityEngine.Random.Range(0,6)], TilesParent);
			GameObject tile = Instantiate(TilesPrefabs[UnityEngine.Random.Range(0,6)], new Vector3 (TileMap[i].x, TileMap[i].y, 0), quat);
			tile.transform.SetParent(TilesParent.transform);
			PlacedTiles.Add(tile);
		}

	}
}
