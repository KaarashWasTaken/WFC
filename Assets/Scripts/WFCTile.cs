using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Grass,
    Dirt,
    Path
}
public class WFCTile : MonoBehaviour
{
    public TileType tileType;
    public List<SpriteRenderer> spriteRenderers;
    public List<SpriteRenderer> allowedSprites;
    SpriteRenderer activeSprite;
    // Start is called before the first frame update
    void Start()
    {
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            allowedSprites.Add(sprite);
        }
        int random = UnityEngine.Random.Range(0, 5);
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            if (i == random)
            {
                spriteRenderers[i].enabled = true;
                activeSprite = spriteRenderers[i];
                if (i != 1 && i != 3)
                {
                    tileType = TileType.Path;
                }
                else if (i == 1)
                {
                    tileType = TileType.Dirt;
                }
                else
                {
                    tileType = TileType.Grass;
                }
            }
            else
            {
                spriteRenderers[i].enabled = false;
            }
        }
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            if (sprite != activeSprite && allowedSprites.Contains(sprite))
            {
                allowedSprites.Remove(sprite);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
