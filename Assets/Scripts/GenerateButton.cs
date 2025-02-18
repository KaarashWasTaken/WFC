using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateButton : MonoBehaviour
{
	public int MapSize;
	TileManager tileManagerScript;
	public GameObject TileManager;
    // Start is called before the first frame update
    void Start()
    {
		tileManagerScript = TileManager.GetComponent<TileManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ButtonPress()
	{
		tileManagerScript.MapSize = MapSize;
		tileManagerScript.RegenerateMap();
	}

	public void UpdateSize(string newVal)
	{
		if (int.TryParse(newVal, out int val))
		{
			if (val >= 2)
			{
				MapSize = val;
			}
		}
	}
}
