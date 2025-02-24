using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;

public enum TileType
{
    None,
    Grass,
    Dirt,
    Path
}
public class WFCTile : MonoBehaviour
{
    //TILETYPE, FOR FASTER COMPARISON
    public TileType tileType;
    //ALL SPRITES
    public List<SpriteRenderer> spriteRenderers;
    //SPRITES WITH TILETYPE ALLOWED BASED ON NEIGHBOURS
    Dictionary<SpriteRenderer, TileType> allowedSprites = new();
    //DIRECTIONS WHICH THE TILE IS OPEN WHEN IT IS A PATH TYPE
    // public Dictionary<string, bool> openDirections = new()
    // {
    //     {"North", false},
    //     {"East", false},
    //     {"South", false},
    //     {"West", false}
    // };
    //DIRECTIONS WHICH THE TILE IS OPEN WHEN IT IS A PATH TYPE: 0:NORTH 1:WEST 2:SOUTH 3:EAST
    public List<bool> openDirections = new(){false, false, false, false};
    //0:NORTH 1:WEST 2:SOUTH 3:EAST
    public List<WFCTile> adjacentTiles = new();
    //THE TILEMANAGER THAT CREATED THIS TILE
    public TileManager manager;
    //THE ACTIVE AND VISIBLE SPRITE
    SpriteRenderer activeSprite;
    //THE LOCATION OF THE TILE
    Vector2 tileLoc;

    // START IS CALLED BEFORE THE FIRST FRAME UPDATE
    void Awake()
    {
        ResetSprite();
    }

    //SET THE TILEMANAGER VARIABLE
    public void SetManager(TileManager newManager)
    {
        manager = newManager;
    }

    //SET THETILE LOCATION
    public void SetTileLoc(Vector2 loc)
    {
        tileLoc = loc;
    }

    public Vector2 GetTileLoc()
    {
        return tileLoc;
    }

    //SELECT WHICH SPRITE TO USE
    public void SelectSprite()
    {
        if (adjacentTiles.Count < 4)
            FindAdjacent();
        int random = UnityEngine.Random.Range(0, allowedSprites.Count);
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            if (i == random)
            {
                spriteRenderers[i].enabled = true;
                activeSprite = spriteRenderers[i];
                tileType = allowedSprites[activeSprite];
                transform.Rotate(Vector3.forward, UnityEngine.Random.Range(0,4) * 90);
                if (tileType == TileType.Path)
                {
                    AssignOpen();
                }
            }
            else
            {
                spriteRenderers[i].enabled = false;
            }
        }
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            if (sprite != activeSprite && allowedSprites.ContainsKey(sprite))
            {
                allowedSprites.Remove(sprite);
            }
        }
    }

    //ASSIGN THE OPEN DIRECTIONS TO THE OPENDIRECTIONS LIST
    void AssignOpen()
    {
        //ALL DIRECTIONS ARE OPEN AT THE INNER CORNER
        if (activeSprite == spriteRenderers[4])
        {
            for (int i = 0; i < openDirections.Count; i++)
            {
                openDirections[i] = true;
            }
        }
        else
        {
            //ROTATE WITH NEGATIVE SINCE MY LIST IS ORDERED OPPOSITE OF HOW THE UNIT CIRCLE ROTATES
            if (activeSprite == spriteRenderers[5])
            {
                openDirections[(0 + (int)(transform.rotation.eulerAngles.z / 90) % 4 + 4) % 4] = true;
                openDirections[(2 + (int)(transform.rotation.eulerAngles.z / 90) % 4 + 4) % 4] = true;
                openDirections[(3 + (int)(transform.rotation.eulerAngles.z / 90) % 4 + 4) % 4] = true;
            }
            else
            {
                openDirections[(0 + (int)(transform.rotation.eulerAngles.z / 90) % 4 + 4) % 4] = true;
                openDirections[(3 + (int)(transform.rotation.eulerAngles.z / 90) % 4 + 4) % 4] = true;
            }
        }
    }

    //HIDE ALL SPRITES
    public void ResetSprite()
    {
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            if (allowedSprites.ContainsKey(sprite))
            {
                allowedSprites.Remove(sprite);
            }
        }
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            if (i == 0 || i == 4 || i == 5)
            {
                allowedSprites.Add(spriteRenderers[i], TileType.Path);
            }
            else if (i == 1)
            {
                allowedSprites.Add(spriteRenderers[i], TileType.Dirt);
            }
            else if (i == 2 || i == 3)
            {
                allowedSprites.Add(spriteRenderers[i], TileType.Grass);
            }
            spriteRenderers[i].enabled = false;
            if (spriteRenderers[i].name == "Empty")
            {
                spriteRenderers[i].enabled = true;
            }
            transform.rotation = Quaternion.identity;
        }
        for (int i = 0; i < openDirections.Count; i++)
        {
            openDirections[i] = false;
        }
    }

    //FIND ALL TILE NEIGHBOURS
    void FindAdjacent()
    {
        if (manager == null)
        {
            transform.parent.GetComponentInParent<TileManager>();
        }
        for (int i = 1; i <= 4; i++)
        {
            float xDiff = Mathf.RoundToInt(Mathf.Cos(math.radians(90 * i)));
            float yDiff = Mathf.RoundToInt(Mathf.Sin(math.radians(90 * i)));
            try
            {
            adjacentTiles.Add(manager.GetTileFromLoc(new(tileLoc.x + xDiff, tileLoc.y + yDiff)));
                print("Added: " + adjacentTiles[i-1]);
            }
            catch(System.NullReferenceException)
            {
                print("Added null to adjacents");
                print(manager);
                adjacentTiles.Add(null);
            }
            catch(System.Exception ex)
            {
                Debug.LogError("An unexpected error occurred: " + ex.Message);
            }
        }
    }

    //FIND ALLOWED SPRITES
    void GetAllowedSprites()
    {

    }

    // UPDATE IS CALLED ONCE PER FRAME
    void Update()
    {

    }
}
