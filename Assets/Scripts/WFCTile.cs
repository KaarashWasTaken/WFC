using System.Collections.Generic;
using UnityEngine;

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
    TileType tileType;
	//LIST OF ALL SPRITES
    public List<SpriteRenderer> spriteRenderers;
	//THE ADJACENT TILES TO THIS TILE 0: NORTH, 1: WEST, 2: SOUTH, 3: EAST
    public WFCTile[] adjacentTiles = new WFCTile[4];
	//THIS TILE'S ROTATION
    public TileManager manager;
	//THIS TILE'S ROTATION
    Vector2 tileLoc;

	//ASSIGNING BITS TO CORRESPOND TILE TYPE
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
	int tilebyte = 0b0000;
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
		int randomTile = ChooseRandomTile();
        tileType = possibleTypesWithRot[randomTile].type;
		rotation = 90 * possibleTypesWithRot[randomTile].rot;
		tilebyte = ShiftBitsRight(typeToBitMap[tileType], possibleTypesWithRot[randomTile].rot );
        UpdateSprite();
		collapsed = true;
    }

	int ChooseRandomTile()
	{
		//MAKE THE RANDOM HAVE A LITTLE BIAS TOWARDS GRASS AS THE MAP ELSE LOOKS LIKE A LABYRINTH
		if (possibleTypesWithRot.Contains(new (TileType.Grass, 0)) && possibleTypesWithRot.Count > 1)
		{
			if (UnityEngine.Random.Range(0, 10) >= 2)
			{
				return possibleTypesWithRot.IndexOf(new(TileType.Grass, 0));
			}
			else
			{
				possibleTypesWithRot.Remove(new(TileType.Grass, 0));
			}
		}
		return UnityEngine.Random.Range(0, possibleTypesWithRot.Count);
	}

	//UPDATE SPRITE TO BE THE CURRENT TILETYPE
    void UpdateSprite()
    {
		//IF ITS A GRASS MAKE IT RANDOM BETWEEN FLOWERS AND GRASS
		if (tileType == TileType.Grass)
		{
			spriteRenderers[UnityEngine.Random.Range(2,4)].enabled = true;
			spriteRenderers[(int)TileType.None].enabled = false;
		}
		//OTHERWISE SET THE CORRECT CORRESPONDING TYPE
		else
		{
			for (int i = 0; i < spriteRenderers.Count; i++)
			{
				spriteRenderers[i].enabled = i == (int)tileType;
			}
		}
		transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public void ResetTile()
    {
		tileType = TileType.None;
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

    public void FindAdjacent()
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
		//CHECK NORTH
		if (adjacentTiles[0] != null && adjacentTiles[0].collapsed)
		{
			possibleByte |= SwapNibblesInByte(adjacentTiles[0].tilebyte) & NORTHBIT;
			setBits |= NORTHBIT;
		}
		//CHECK WEST
		if (adjacentTiles[1] != null && adjacentTiles[1].collapsed)
		{
			possibleByte |= ReverseBitOrder(adjacentTiles[1].tilebyte) & WESTBIT;
			setBits |= WESTBIT;
		}
		//CHECK SOUTH
		if (adjacentTiles[2] != null && adjacentTiles[2].collapsed)
		{
			possibleByte |= SwapNibblesInByte(adjacentTiles[2].tilebyte) & SOUTHBIT;
			setBits |= SOUTHBIT;
		}
		//CHECK EAST
		if (adjacentTiles[3] != null && adjacentTiles[3].collapsed)
		{
			possibleByte |= ReverseBitOrder(adjacentTiles[3].tilebyte) & EASTBIT;
			setBits |= EASTBIT;
		}
		foreach (var mapEntry in typeToBitMap)
		{
			//GRASS AND DIRT DOESN'T NEED TO BE ROTATED
			if (mapEntry.Key == TileType.Grass || mapEntry.Key == TileType.Dirt)
			{
				TypeWithRotation tileToCheck = new(mapEntry.Key, 0);
				if (((mapEntry.Value ^ possibleByte) & setBits) == 0 && !possibleTypesWithRot.Contains(tileToCheck))
				{
					possibleTypesWithRot.Add(tileToCheck);
				}
				else if (((mapEntry.Value ^ possibleByte) & setBits) != 0 && possibleTypesWithRot.Contains(tileToCheck))
				{
					possibleTypesWithRot.Remove(tileToCheck);
				}
				continue;
			}
			//ROTATE 90 DEGREES 4 TIMES TO SEE IF THE TILETYPE CAN FIT
			for (int i = 0; i < 4; i++)
			{
				int rotatedShape = ShiftBitsRight(mapEntry.Value, i);
				TypeWithRotation tileToCheck = new(mapEntry.Key, i);
				if (((rotatedShape ^ possibleByte) & setBits) == 0 && !possibleTypesWithRot.Contains(tileToCheck))
				{
					possibleTypesWithRot.Add(tileToCheck);
				}
				else if (((rotatedShape ^ possibleByte) & setBits) != 0 && possibleTypesWithRot.Contains(tileToCheck))
				{
					possibleTypesWithRot.Remove(tileToCheck);
				}
			}
		}
	}

	//MAKE BYTE GO FROM 0bABCD to 0bDCBA
	int ReverseBitOrder(int num)
	{
		byte reversed = 0;
		reversed |= (byte)((num & 0x01) << 3);
		reversed |= (byte)((num & 0x02) << 1);
		reversed |= (byte)((num & 0x04) >> 1);
		reversed |= (byte)((num & 0x08) >> 3);

		return reversed;
	}

	//MAKE BYTE GO FROM 0bABCD TO 0bBADC
	int SwapNibblesInByte(int num)
	{
		//0bABCD -> 0bDCBA
		int reversedOrder = ReverseBitOrder(num);
		//0bDCBA -> 0bBADC
		return ShiftBitsRight(reversedOrder, 2);
	}

	//SHIFT BITS TO THE RIGHT WITH A WRAP AROUND AT 4 BITS
	int ShiftBitsRight(int bit, int count)
	{
		return (bit >> count) | ((bit << (4 - count)) & 0b1111);
	}

	//SHIFT BITS TO THE LEFT WITH A WRAP AROUND AT 4 BITS
	int ShiftBitsLeft(int bit, int count)
	{
		return (bit << count) | ((bit >> (4 - count)) & 0b1111);
	}

}
