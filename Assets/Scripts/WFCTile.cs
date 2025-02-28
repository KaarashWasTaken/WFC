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
    Grass,
	Flower,
	Inner,
	Straight,
    None,
	LASTINLIST
}

public struct TypeWithRotation
{
	public TileType type;
	public int rot;
	public TypeWithRotation(TileType newType, int newRot)
	{
		type = newType;
		rot = newRot;
	}
}

public class WFCTile : MonoBehaviour
{
	/*--------BIT VARIABLES--------*/
	const int NORTHBIT = 0b1001;
	const int WESTBIT = 0b1100;
	const int SOUTHBIT = 0b0110;
	const int EASTBIT = 0b0011;
	/*-----------------------------*/

	//THIS TILE'S TYPE
    public TileType tileType;
	//LIST OF ALL SPRITES
    public List<SpriteRenderer> spriteRenderers;
	//LIST OF THIS TILE'S POSSIBLE TYPES
    public List<TileType> possibleTileTypes = new();
	//THE ADJACENT TILES TO THIS TILE 0: NORTH, 1: WEST, 2: SOUTH, 3: EAST
    public WFCTile[] adjacentTiles = new WFCTile[4];
	//THIS TILE'S ROTATION
    public TileManager manager;
	//THIS TILE'S ROTATION
    public Vector2 tileLoc;

	Dictionary<TileType, int> typeToBitMap = new Dictionary<TileType, int>
	{
		{TileType.Corner, 0b1110},
		{TileType.Dirt, 0b0000},
		{TileType.Grass, 0b1111},
		{TileType.Inner, 0b0100},
		{TileType.Straight, 0b1100}
	};

	public List<TypeWithRotation> possibleTypesWithRot = new();

	//THIS TILE'S ROTATION
    public int rotation = 0;

	//BYTE FOR TILE USED FOR BIT-WISE COMPARISON
	public int tilebyte = 0b0000;
	public bool collapsed = false;
	public bool neigboursFound = false;

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
		FindPossibleTypes();
		int randomTile = UnityEngine.Random.Range(0, possibleTypesWithRot.Count);
        tileType = possibleTypesWithRot[randomTile].type;
		rotation = 90 * possibleTypesWithRot[randomTile].rot;
		tilebyte = ShiftBitsRight(typeToBitMap[tileType], possibleTypesWithRot[randomTile].rot );
        UpdateSprite();
		collapsed = true;
    }

	//UPDATE SPRITE TO BE THE CURRENT TILETYPE
    void UpdateSprite()
    {
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].enabled = i == (int)tileType;
        }
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public void ResetTile()
    {
        possibleTileTypes.Clear();
		tileType = TileType.None;
        for (int i = 0; i < (int)TileType.None; i++)
        {
            possibleTileTypes.Add((TileType)i);
        }

        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].enabled = i == (int)TileType.None;
        }
		if (!neigboursFound)
			FindAdjacent();
        transform.rotation = Quaternion.identity;
		collapsed = false;
		possibleTypesWithRot.Clear();
		tilebyte = 0b0000;
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
		neigboursFound = true;
    }

	//FIND POSSIBLE TILE TYPES FROM COMPARING TILEBYTES
	public void FindPossibleTypes()
	{
		int possibleByte = 0b0000;
		//KEEPS TRACK OF WHICH NEIGHBOURS ARE AFFECTING THE TILE
		int setBits = 0b0000;
		if (adjacentTiles[0] != null && adjacentTiles[0].collapsed)
		{
			possibleByte |= ShiftBitsRight(adjacentTiles[0].tilebyte, 2) & NORTHBIT;
			setBits |= NORTHBIT;
		}
		if (adjacentTiles[1] != null && adjacentTiles[1].collapsed)
		{
			possibleByte |= ShiftBitsRight(adjacentTiles[1].tilebyte, 2) & WESTBIT;
			setBits |= WESTBIT;
		}
		if (adjacentTiles[2] != null && adjacentTiles[2].collapsed)
		{
			possibleByte |= ShiftBitsRight(adjacentTiles[2].tilebyte, 2) & SOUTHBIT;
			setBits |= SOUTHBIT;
		}
		if (adjacentTiles[3] != null && adjacentTiles[3].collapsed)
		{
			possibleByte |= ShiftBitsRight(adjacentTiles[3].tilebyte, 2) & EASTBIT;
			setBits |= EASTBIT;
		}
		foreach (var mapEntry in typeToBitMap)
		{
			//ADDED AMOUNT OF 90 ROTATIONS UNTIL IT FITS
			for (int i = 0; i < 4; i++)
			{
				int rotatedShape = ShiftBitsRight(mapEntry.Value, i);
				if (((rotatedShape ^ possibleByte) & setBits) == 0)
				{
					possibleTypesWithRot.Add(new TypeWithRotation(mapEntry.Key, i));
				}
			}
		}
	}

	//
	int ShiftBitsRight(int bit, int count)
	{
		int x = bit >> count;
		int y = (bit << (4 - count)) & 0b1111;
		return (bit >> count) | ((bit << (4 - count)) & 0b1111);
	}

	//
	int ShiftBitsLeft(int bit, int count)
	{
		return (bit << count) | (bit >> (4 - count));
	}

}
