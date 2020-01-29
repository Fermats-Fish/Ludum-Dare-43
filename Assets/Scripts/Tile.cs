using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

	public string tileType;
	public int saturation;
	public int nTrees;
	public string structure;

	public Tile(string tileType, int saturation){
		this.tileType = tileType;
		this.saturation = saturation;
		this.nTrees = 0;
	}

	public Tile (string tileType, int saturation, int nTrees)
	{
		this.tileType = tileType;
		this.saturation = saturation;
		this.nTrees = nTrees;
	}

}
