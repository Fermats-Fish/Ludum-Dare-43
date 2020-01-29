using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {

	public int size;

	Tile[][] tiles;

	public Map(int size) {

		this.size = size;

		tiles = new Tile[2 * size - 1][];

		// Loops through a variable i. In this loop we will create the two rows which are at index i from the top and bottom of the map.
		for (int i = 0; i < size; i++) {

			// We need WORLD_SIZE + i no. of hexes for each row we make.

			// Bottom row:
			tiles[i] = new Tile[size + i];

			// Top row:
			if (i != size - 1){
				tiles[2 * size - i - 2] = new Tile[size + i];
			}

		}

	}

	// Returns whether a tile is in the hex map.
	public bool InMap(Coord c){
		return InMap (c.q, c.r);
	}

	public bool InMap(int q, int r){

		return !(q < Mathf.Max (0, r - size + 1) || q > Mathf.Min(size + r - 1, 2 * size - 2) || r < 0 || r > 2 * size - 2);

	}

	public Tile GetTileAt (Coord c){
		return GetTileAt (c.q, c.r);
	}

	public void SetTileAt (Coord c, Tile t){
		SetTileAt (c.q, c.r, t);
	}

	public Tile GetTileAt (int q, int r){

		if (InMap(q,r) == false) {
			Debug.LogError (q.ToString() + ", " + r.ToString() + " is out of the map!");
		}

		// If our row is more than half, then the first column for this row is not 0.
		int x = q;

		if ( r > size - 1 ) {
			x -= r - size + 1;
		}

		return tiles[r][x];

	}

	public void SetTileAt (int q, int r, Tile t){

		if (InMap(q,r) == false) {
			Debug.LogError (q.ToString() + ", " + r.ToString() + " is out of the map!");
		}

		// If our row is more than half, then the first column for this row is not 0.
		int x = q;

		if ( r > size - 1 ) {
			x -= r - size + 1;
		}

		tiles[r][x] = t;
	}

}
