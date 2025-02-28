using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;

public enum TileType
{
	Corner,
    Dirt,
	Flowers,
    Grass,
	Inner,
	Straight,
    None,
	LASTINLIST
}
// public class WFCTile : MonoBehaviour
// {
//     //TILETYPE, FOR FASTER COMPARISON
//     public TileType tileType;
//     //ALL SPRITES
//     public List<SpriteRenderer> spriteRenderers;
//     //SPRITES WITH TILETYPE ALLOWED BASED ON NEIGHBOURS
//     Dictionary<TileType, Quaternion> allowedSprites = new();
//     //DIRECTIONS WHICH THE TILE IS OPEN WHEN IT IS A PATH TYPE: 0:NORTH 1:WEST 2:SOUTH 3:EAST
//     public List<bool> openDirections = new(){false, false, false, false};
//     //0:NORTH 1:WEST 2:SOUTH 3:EAST
//     public List<WFC2Tile> adjacentTiles = new();
//     //THE TILEMANAGER THAT CREATED THIS TILE
//     public TileManager manager;
//     //THE ACTIVE AND VISIBLE SPRITE
//     SpriteRenderer activeSprite;
//     //THE LOCATION OF THE TILE
//     Vector2 tileLoc;

// 	/*--------ALLOWED NEIGHBOURS--------*/
// 	List<Dictionary<TileType, Quaternion>> allowedUpNeighbour = new();
// 	List<Dictionary<TileType, Quaternion>> allowedLeftNeighbour = new();
// 	List<Dictionary<TileType, Quaternion>> allowedDownNeighbour = new();
// 	List<Dictionary<TileType, Quaternion>> allowedRightNeighbour = new();
// 	/*----------------------------------*/


//     // START IS CALLED BEFORE THE FIRST FRAME UPDATE
//     void Awake()
//     {
//         ResetSprite();
//     }

//     //SET THE TILEMANAGER VARIABLE
//     public void SetManager(TileManager newManager)
//     {
//         manager = newManager;
//     }

//     //SET THETILE LOCATION
//     public void SetTileLoc(Vector2 loc)
//     {
//         tileLoc = loc;
//     }

//     public Vector2 GetTileLoc()
//     {
//         return tileLoc;
//     }

//     //SELECT WHICH SPRITE TO USE
//     public void SelectSprite()
//     {
//         if (adjacentTiles.Count < 4)
//             FindAdjacent();
//         int random = UnityEngine.Random.Range(0, allowedSprites.Count);
//         for (int i = 0; i < spriteRenderers.Count; i++)
//         {
//             if (i == random)
//             {
//                 spriteRenderers[i].enabled = true;
//                 activeSprite = spriteRenderers[i];
//                 tileType = allowedSprites[];
//                 transform.Rotate(Vector3.forward, UnityEngine.Random.Range(0,4) * 90);
//                 // if (tileType == TileType.Path)
//                 // {
//                 //     AssignOpen();
//                 // }
//             }
//             else
//             {
//                 spriteRenderers[i].enabled = false;
//             }
//         }
//         // foreach (SpriteRenderer sprite in spriteRenderers)
//         // {
//         //     if (sprite != activeSprite && allowedSprites.ContainsKey(sprite))
//         //     {
//         //         allowedSprites.Remove(sprite);
//         //     }
//         // }
//     }

//     //ASSIGN THE OPEN DIRECTIONS TO THE OPENDIRECTIONS LIST
//     void AssignOpenDirections()
// 	{
// 		for (int i = 0; i < openDirections.Count; i++)
// 		{
// 			openDirections[i] = false;
// 		}

// 		if (tileType == TileType.Inner)
// 		{
// 			for (int i = 0; i < openDirections.Count; i++)
// 			{
// 				openDirections[i] = true;
// 			}
// 		}
// 		else if (tileType == TileType.Straight)
// 		{
// 			openDirections[0] = true;
// 			openDirections[2] = true;
// 		}
// 		else if (tileType == TileType.Corner)
// 		{
// 			openDirections[0] = true;
// 			openDirections[3] = true;
// 		}
// 		//Rotate directions to match rotation.
// 		int rotation = (int)(transform.rotation.eulerAngles.z / 90);
// 		List<bool> rotatedDirections = new List<bool>(openDirections);
// 		for (int i = 0; i < 4; i++)
// 		{
// 			openDirections[i] = rotatedDirections[(i - rotation + 4) % 4];
// 		}
// 	}

//     //HIDE ALL SPRITES
//     public void ResetSprite()
//     {
// 		for (int i = 0; i < (int)TileType.LASTINLIST; i++)
//         {
//             if (allowedSprites.ContainsKey((TileType)i))
//             {
//                 allowedSprites.Remove((TileType)i);
//             }
//         }
//         for (int i = 0; i < spriteRenderers.Count; i++)
//         {
// 			allowedSprites.Add((TileType)i, Quaternion.identity);
//             // if (i == 0 || i == 4 || i == 5)
//             // {
//             //     allowedSprites.Add(spriteRenderers[i], TileType.Path);
//             // }
//             // else if (i == 1)
//             // {
//             //     allowedSprites.Add(spriteRenderers[i], TileType.Dirt);
//             // }
//             // else if (i == 2 || i == 3)
//             // {
//             //     allowedSprites.Add(spriteRenderers[i], TileType.Grass);
//             // }
//             spriteRenderers[i].enabled = false;
//             if (spriteRenderers[(int)TileType.None])
//             {
//                 spriteRenderers[i].enabled = true;
//             }
//             transform.rotation = Quaternion.identity;
//         }
//         for (int i = 0; i < openDirections.Count; i++)
//         {
//             openDirections[i] = false;
//         }
//     }

//     //FIND ALL TILE NEIGHBOURS
//     void FindAdjacent()
//     {
// 		//NORTH
// 		adjacentTiles[0] = manager.GetTileFromLoc(tileLoc + Vector2.up);
// 		//WEST
//         adjacentTiles[1] = manager.GetTileFromLoc(tileLoc + Vector2.left);
// 		//SOUTH
//         adjacentTiles[2] = manager.GetTileFromLoc(tileLoc + Vector2.down);
// 		//EAST
//         adjacentTiles[3] = manager.GetTileFromLoc(tileLoc + Vector2.right);
//     }

//     //FIND ALLOWED SPRITES
//     void GetAllowedSprites()
//     {

//     }

//     // UPDATE IS CALLED ONCE PER FRAME
//     void Update()
//     {

//     }
// }
public class WFCTile : MonoBehaviour
{
	//THIS TILE'S TYPE
    public TileType tileType;
	//LIST OF ALL SPRITES
    public List<SpriteRenderer> spriteRenderers;
	//LIST OF THIS TILE'S POSSIBLE TYPES
    public List<TileType> possibleTileTypes = new();
	//THIS TILE'S OPEN DIRECTIONS
    public List<bool> openDirections = new() { false, false, false, false };
	//
    public WFCTile[] adjacentTiles = new WFCTile[4];
    public TileManager manager;
    public Vector2 tileLoc;

    public int rotation = 0;
	//ALLOWED NEIGHBOURS BASED ON TYPE AND ROTATION
    public Dictionary<TileType, List<int>>[] allowedNeighbours = new Dictionary<TileType, List<int>>[4];

    void Awake()
    {
        ResetTile();
    }

    public void SetManager(TileManager newManager)
    {
        manager = newManager;
    }

    public void SetTileLoc(Vector2 loc)
    {
        tileLoc = loc;
    }

    public Vector2 GetTileLoc()
    {
        return tileLoc;
    }

    public void CollapseTile()
    {
        if (possibleTileTypes.Count == 0)
        {
            Debug.LogError("No possible tiles at " + tileLoc);
            return;
        }

        tileType = possibleTileTypes[UnityEngine.Random.Range(0, possibleTileTypes.Count)];
        rotation = GetPossibleRotations(tileType)[UnityEngine.Random.Range(0, GetPossibleRotations(tileType).Count)];
        UpdateSprite();
        AssignOpenDirections();
        UpdateNeighbours();
    }

    void UpdateSprite()
    {
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].enabled = i == (int)tileType;
        }
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    List<int> GetPossibleRotations(TileType type)
    {
        List<int> possibleRotations = new List<int>() { 0, 90, 180, 270 };
        for (int i = 0; i < 4; i++)
        {
            if (adjacentTiles[i] != null && adjacentTiles[i].tileType != TileType.None)
            {
                if (adjacentTiles[i].allowedNeighbours[(i + 2) % 4].ContainsKey(type))
                {
                    List<int> allowed = adjacentTiles[i].allowedNeighbours[(i + 2) % 4][type];
                    possibleRotations = possibleRotations.Intersect(allowed).ToList();
                }
                else
                {
                    possibleRotations.Clear();
                    break;
                }
            }
        }
        return possibleRotations;
    }

    void UpdateNeighbours()
    {
        for (int i = 0; i < 4; i++)
        {
            if (adjacentTiles[i] != null)
            {
                adjacentTiles[i].UpdatePossibleTileTypes(this, (i + 2) % 4);
            }
        }
    }

    public void UpdatePossibleTileTypes(WFCTile neighbor, int direction)
    {
        List<TileType> newPossibleTypes = new List<TileType>(possibleTileTypes);
        foreach (TileType type in possibleTileTypes)
        {
            if (!allowedNeighbours[direction].ContainsKey(type))
            {
                newPossibleTypes.Remove(type);
            }
            else
            {
                List<int> possibleRotations = GetPossibleRotations(type);
                List<int> allowedRotations = allowedNeighbours[direction][type];
                if (possibleRotations.Intersect(allowedRotations).ToList().Count == 0)
                {
                    newPossibleTypes.Remove(type);
                }
            }
        }
        possibleTileTypes = newPossibleTypes;
    }

    public void ResetTile()
    {
        possibleTileTypes.Clear();
        for (int i = 0; i < (int)TileType.LASTINLIST; i++)
        {
            possibleTileTypes.Add((TileType)i);
        }
        for (int i = 0; i < 4; i++)
        {
            allowedNeighbours[i] = new Dictionary<TileType, List<int>>();
            for (int j = 0; j < (int)TileType.LASTINLIST; j++)
            {
                allowedNeighbours[i].Add((TileType)j, new List<int>() { 0, 90, 180, 270 });
            }
        }

        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].enabled = i == (int)TileType.None;
        }
        transform.rotation = Quaternion.identity;
    }

    void FindAdjacent()
    {
		//NORTH
		adjacentTiles[0] = manager.GetTileFromLoc(tileLoc + Vector2.up);
		//WEST
        adjacentTiles[1] = manager.GetTileFromLoc(tileLoc + Vector2.left);
		//SOUTH
        adjacentTiles[2] = manager.GetTileFromLoc(tileLoc + Vector2.down);
		//EAST
        adjacentTiles[3] = manager.GetTileFromLoc(tileLoc + Vector2.right);
    }

    void AssignOpenDirections()
    {
        for (int i = 0; i < openDirections.Count; i++)
        {
            openDirections[i] = false;
        }

        if (tileType == TileType.Inner)
        {
            for (int i = 0; i < openDirections.Count; i++)
            {
                openDirections[i] = true;
            }
        }
        else if (tileType == TileType.Straight)
        {
            openDirections[0] = true;
            openDirections[2] = true;
        }
        else if (tileType == TileType.Corner)
        {
            openDirections[0] = true;
            openDirections[3] = true;
        }
        int rotationIndex = (int)(transform.rotation.eulerAngles.z / 90);
        List<bool> rotatedDirections = new List<bool>(openDirections);
        for (int i = 0; i < 4; i++)
        {
            openDirections[i] = rotatedDirections[(i - rotationIndex + 4) % 4];
        }
    }
}
