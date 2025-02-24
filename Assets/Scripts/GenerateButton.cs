using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateButton : MonoBehaviour
{
	public int mapSize;
	public TileManager manager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ButtonPress()
	{
		manager.mapSize = mapSize;
		manager.RegenerateMap();
	}

	public void UpdateSize(string newVal)
	{
		if (int.TryParse(newVal, out int val))
		{
			if (val >= 2)
			{
				mapSize = val;
			}
		}
	}
}
