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
	// WFC wfc;

	//LIST OF COLLAPSED TILES
	List<WFCTile> collapsedTiles = new();
	//LIST OF UNCOLLAPSED TILES
	List<WFCTile> uncollapsedTiles = new();

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
	int tilesPerFrame = 4;
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
		// wfc = GetComponent<WFC>();
		RegenerateMap();
    }

    // UPDATE IS CALLED ONCE PER FRAME
    void Update() 
	{
		timerElapsed = Time.time - timerStart;
		/*RUN STEP GENERATION IF STEP BY STEP IS ACTIVATED, NOT ALL TILES ARE SPAWNED, 
		THE TIME BETWEEN EACH STEP IS MORE THAN THE DELAY TIME AND MANUAL STEPPING IS NOT ACTIVATED*/
		if (!fullySpawned && stepByStep && timerElapsed >= sbsStepTime && !sbsToggle.gameObject.GetComponent<ManualStepToggle>().GetManualMode())
		{
			for (int i = 0; i < tilesPerFrame; i++)
			{
				StepGenerateMap();
			}
			timerStart = Time.time;
		}
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
			tile.ResetTile();
		}

		if (seedToggle.isOn)
		{
			seed = UnityEngine.Random.Range(0, 10000);
		}
		UnityEngine.Random.InitState(seed);
		LogSeed();

		uncollapsedTiles.Clear();
		for (int i = 0; i < mapSize; i++)
		{
			for (int j = 0; j < mapSize; j++)
			{
				uncollapsedTiles.Add(tileMap[new(j, i)]);
			}
		}

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

	//GENERATE A MAP WITHOUT STEP BY STEP
	public void GenerateMap()
	{
		//FIRST TILE WILL START FROM THE CENTER OF THE MAP
		placedTiles[mapSize*mapSize/2].CollapseTile();
		uncollapsedTiles.Remove(placedTiles[mapSize*mapSize/2]);
		WFCTile tileToCollapse;
		while (uncollapsedTiles.Count > 0)
		{
			tileToCollapse = FindLowestEntropyTile();
			tileToCollapse.CollapseTile();
			foreach (var neighbour in tileToCollapse.adjacentTiles)
			{
				if (neighbour != null && !neighbour.collapsed)
				{
					neighbour.FindPossibleTypes();
				}
			}
			uncollapsedTiles.Remove(tileToCollapse);
		}
		fullySpawned = true;
	}


	//GENERATE A MAP WITH STEP BY STEP
	public void StepGenerateMap()
	{
		if (fullySpawned)
			return;
		WFCTile tileToCollapse;
		//FIRST TILE WILL START FROM THE CENTER OF THE MAP
		if (uncollapsedTiles.Count == mapSize * mapSize)
			tileToCollapse = tileMap[new(mapSize/2, mapSize/2)];
		else
			tileToCollapse = FindLowestEntropyTile();
		tileToCollapse.CollapseTile();
		foreach (var neighbour in tileToCollapse.adjacentTiles)
		{
			if (neighbour != null && !neighbour.collapsed)
			{
				neighbour.FindPossibleTypes();
			}
		}
		uncollapsedTiles.Remove(tileToCollapse);

		if (uncollapsedTiles.Count <= 0)
		{
			fullySpawned = true;
		}
	}

	//FIND THE TILE WITH THE LOWEST POSSIBLE AMOUNT OF TILETYPES
	WFCTile FindLowestEntropyTile()
	{
		int lowestEntropy = int.MaxValue;
		WFCTile lowestEntropyTile = uncollapsedTiles[0];
		int entropy;
		foreach (WFCTile tile in uncollapsedTiles)
		{
			entropy = tile.possibleTypesWithRot.Count;
			if (entropy < lowestEntropy && entropy > 1)
			{
				lowestEntropyTile = tile;
				lowestEntropy = entropy;
			}
		}
		return lowestEntropyTile;
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

	//GET WFCTILE FROM GIVEN VEC2 LOCATION
	public WFCTile GetTileFromLoc(Vector2 loc)
	{
		if (tileMap.ContainsKey(loc))
			return tileMap[loc];
		else
			return null;
	}

	//SAVE SEED TO 'seeds.log' FILE
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
