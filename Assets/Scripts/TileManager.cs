using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
	//LIST OF TILE POSITIONS
	// public List<Vector2> tilePosMap = new();
	//MAP OF TILES AND THEIR LOCATIONS
	public Dictionary<Vector2, WFCTile> tileMap = new();
	//TILES PLACED IN THE WORLD
	public List<WFCTile> placedTiles = new();
	//THE GAMEOBJECT THAT SHOULD PARENT THE TILES
	public GameObject tilesParent;
	//THE PREFAB FOR TILES
	public WFCTile tileObj;
	//SEED UI TOGGLE
	public Toggle seedToggle;
	//STEP BY STEP UI TOGGLE
	public Toggle sbsToggle;
	//WAVE FUNCTION COLLAPSE SCRIPT
	WFC wfc;

	// THE SIZE OF THE MAP USED AS: MAPSIZE X MAPSIZE
	public int mapSize = 10;
	//LARGEST MAP DIMENSION SO FAR
	int largestMapSize = 0;

	//SEED USED FOR THE RANDOM NUMBER GENERATOR 
	int seed = 0;
	//FILEPATH TO SEED LOG
	string seedLogPath = "Assets/seeds.log";	

	/*--------STEP BY STEP VARIABLES--------*/
	/*USED IN A FOR LOOP FOR THE STEP BY STEP TO DIRECT	
	HOW MANY TILES SHOULD BE PLACED PER LOOP*/
	int tilesPerFrame = 1;
	//USED DURING STEP BY STEP FOR KNOWING WHICH TILE IT'S AT
	int tilePosIndex = 0;
	//BOOL TO CHECK IF THE STEP BY STEP SHOULD CONTINUE
	bool fullySpawned = false;
	//BOOL TO CHECK IF STEP BY STEP SHOULD BE SETUP
	bool stepByStep = false;
	//TIME AT LAST STEP
	float timerStart = 0.0f;
	//TIME ELAPSED FROM LAST STEP
	float timerElapsed = 0.0f;
	//TIME BETWEEN EACH STEP IN SECONDS
	public float sbsStepTime = 0.5f;
	/*--------------------------------------*/


    // START IS CALLED BEFORE THE FIRST FRAME UPDATE
    void Start()
    {
		mapSize = 10;
		wfc = GetComponent<WFC>();
		RegenerateMap();
    }

    // UPDATE IS CALLED ONCE PER FRAME
    void Update() 
	{
		timerElapsed = Time.time - timerStart;
		if (!fullySpawned && stepByStep && timerElapsed >= sbsStepTime)
		{
			for (int i = 0; i < tilesPerFrame; i++)
			{
				StepGenerateMap();
			}
			timerStart = Time.time;
		}
	}

	//GENERATE A MAP WITHOUT STEP BY STEP
	public void GenerateMap()
	{
		foreach (WFCTile tile in placedTiles)
		{
			if (tile.GetTileLoc().x < mapSize && tile.GetTileLoc().y < mapSize)
				tile.SelectSprite();
		}
		fullySpawned = true;
	}

	//SPAWN NEW TILES
	void SpawnTiles()
	{
        for (int i = 0; i < mapSize * mapSize; i++)
		{
			if (!tileMap.ContainsKey(new(i % mapSize, i / mapSize)))
			{
				WFCTile tile = Instantiate(tileObj, new Vector3(i % mapSize, i / mapSize, 0), Quaternion.identity);
				tileMap.Add(new(i % mapSize, i / mapSize), tile);
				tile.SetTileLoc(new(i % mapSize, i / mapSize));
				tile.transform.SetParent(tilesParent.transform);
				tile.SetManager(GetComponent<TileManager>());
				placedTiles.Add(tile);
			}
		}
	}
	public WFCTile GetTileFromLoc(Vector2 loc)
	{
		if (tileMap.ContainsKey(loc))
			return tileMap[loc];
		else
			return null;
	}
	//RESTART GENERATION OF MAP AND DECIDE WHAT TYPE OF GENERATION
	public void RegenerateMap()
	{
		if (mapSize > largestMapSize)
		{
			largestMapSize = mapSize;
			SpawnTiles();
		}
		foreach (WFCTile tile in placedTiles)
		{
			tile.ResetSprite();
		}

		if (seedToggle.isOn)
		{
			seed = UnityEngine.Random.Range(0, 10000);
		}
		UnityEngine.Random.InitState(seed);
		LogSeed();

		stepByStep = sbsToggle.isOn;
		if (stepByStep)
		{
			tilePosIndex = 0;
			fullySpawned = false;
			StepGenerateMap();
			return;
		}
		GenerateMap();
	}

	//GENERATE A MAP WITH STEP BY STEP
	public void StepGenerateMap()
	{
		placedTiles[tilePosIndex].SelectSprite();

		//Spawn next tile
		// WFCTile tile = Instantiate(tileObj, new Vector3 (TilePosMap[tilePosIndex-1].x, TilePosMap[tilePosIndex-1].y, 0), Quaternion.identity);
		// tile.transform.SetParent(TilesParent.transform);
		// tile.SetManager(this);
		// PlacedTiles.Add(tile);

		tilePosIndex ++;
		if (tilePosIndex == mapSize * mapSize)
		{
			fullySpawned = true;
		}
	}

	public void LogSeed()
	{
		string formattedSeed = seed.ToString("D4");
		DateTime now = DateTime.Now;
		string timeStamp = now.ToString("yyyy-MM-dd HH:mm:ss");
		string logEntry = $"{timeStamp}: {formattedSeed}";
		try
		{
			File.AppendAllText(seedLogPath, logEntry + "\n\n");
		}
		catch(System.Exception ex)
		{
			UnityEngine.Debug.LogError("Error logging seed: " + ex.Message);
		}
	}
}
