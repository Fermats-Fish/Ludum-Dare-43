using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

	public GameObject hexagonPrefab;
	public GameObject treePrefab;

	public const int WORLD_SIZE = 20;

	const int LAKE_MIN_DIST_FROM_EDGE = 7;
	const int LAKE_RADIUS = 3;
	const int MAX_LAKE_RADIUS = 5;
	const float LAKE_CHANCE_1_EXTRA = 0.7f;
	const float LAKE_CHANCE_2_EXTRA = 0.3f;

	const int SATURATION_DISTANCE = 4;
	const int MAX_SAT = 15;

	const float TREE_WAVELENGTH = 10f;
	const float treeMax = 5f;
	const float treeMin = 2f;

	int STARTING_WORKERS = 3;

	float seedX;
	float seedY;

	static readonly Color FULL_SAT_COLOR = new Color(0f, 190f/255f, 0f); //new Color(1f, 192f/255f, 203f/255f);
	static readonly Color NO_SAT_COLOR = new Color(0.1f, 130f/255f, 0.1f); //new Color (200f/255f, 162f/255f, 200f/255f);

	// Maximum amount the direction of the river can change per tile.
	const float MAX_RIVER_DIR_CHANGE = 0.3f;

	public static Vector3 RandomCirclePos(){
		Vector3 pos = Random.onUnitSphere;
		pos.z = 0;
		pos.Normalize ();
		pos *= Random.Range (0f, 0.7f);
		return pos;
	}

	public void GenerateWorld(){

		// Calculate the world center.
		Coord centre = new Coord(WORLD_SIZE - 1, WORLD_SIZE - 1);

		// Generate a seed for the perlin noise.
		seedX = Random.Range (0f, 1000f);
		seedY = Random.Range (0f, 1000f);

		// First we shall generate a river!
		float dir = Random.Range (0f, 6f);
		int idir = (int) dir;
		float opp = dir + 3f;
		if (opp >= 6f){
			opp -= 6f;
		}
		int iopp = (int) opp;
		GenerateRiver (centre.GetNeighbours()[idir], dir);
		GC.inst.map.SetTileAt (centre, new Tile ("water", -1));
		GenerateRiver (centre.GetNeighbours()[iopp], opp);

		// Now generate the lake coordinates!
		int lr = Random.Range (LAKE_MIN_DIST_FROM_EDGE, WORLD_SIZE);
		int lq = Random.Range (LAKE_MIN_DIST_FROM_EDGE, WORLD_SIZE + lr - LAKE_MIN_DIST_FROM_EDGE);

		if (Random.Range(0, 2) == 0) {
			lq += WORLD_SIZE - lr - 1;
			lr = 2 * WORLD_SIZE - lr - 2;
		}

		Coord lakeCoord = new Coord (lq, lr);

		
		// Generate water tiles around the lake position.
		foreach (var coord in lakeCoord.GetWithin(MAX_LAKE_RADIUS)) {

			// If within lake radius, this is water.
			int d = coord.DistanceTo (lakeCoord);
			if (d < LAKE_RADIUS) {
				GC.inst.map.SetTileAt (coord, new Tile ("water", -1));
			} else if (d-1 < LAKE_RADIUS && Random.Range(0f, 1f) < LAKE_CHANCE_1_EXTRA){
				GC.inst.map.SetTileAt (coord, new Tile ("water", -1));
			} else if (d-2 < LAKE_RADIUS && Random.Range(0f, 1f) < LAKE_CHANCE_2_EXTRA){
				GC.inst.map.SetTileAt (coord, new Tile ("water", -1));
			}

		}

		// Generate the tiles in a big hexagon.

		// Loops through a variable i. In this loop we will create the two rows which are at index i from the top and bottom of the map.
		for (int i = 0; i < WORLD_SIZE; i++) {

			// We need to create WORLD_SIZE + i no. of hexes for each row we make.
			for (int c = 0; c < WORLD_SIZE + i; c++) {

				// Bottomm Hex:
				CreateHex (c, i);

				// Top Hex:
				if (i != WORLD_SIZE - 1) {
					CreateHex (c + WORLD_SIZE - i - 1, 2*WORLD_SIZE - i - 2);
				}

			}

		}

		// Generate a hut by the river.
		int dist = 1;
		bool br = false;
		while (br == false){

			// Get hexes within distance of 1 (then 2 and so on if no valid hex is found), then shuffle them and pick the first which isn't water.
			List<Coord> spots = centre.GetWithin (dist);
			for (int i = 0; i < spots.Count; i++) {
				int j = Random.Range (0, spots.Count);
				Coord temp = spots [i];
				spots [i] = spots [j];
				spots [j] = temp;
			}

			foreach (Coord spot in spots) {
				if (GC.inst.map.GetTileAt(spot).tileType != "water") {

					// Place a hut here!
					Instantiate (GC.inst.imagePrefab, spot.GetWorldCoords () + Vector3.back, Quaternion.identity).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("hut");
					GC.inst.map.GetTileAt (spot).structure = "hut";

					// Remove the trees on this tile!
					GC.inst.map.GetTileAt (spot).nTrees = 0;
					Vector3 wc = spot.GetWorldCoords ();
					List<JobStructure> trees = new List<JobStructure> ();
					RaycastHit2D[] rcs = Physics2D.RaycastAll (new Vector2 (wc.x, wc.y), Vector2.zero);
					foreach (RaycastHit2D rc in rcs) {
						JobStructure t = rc.transform.GetComponentInChildren<JobStructure> ();
						if (t != null){
							trees.Add (t);
						}
					}
					foreach (JobStructure t in trees){
						Destroy (t.gameObject);
						print("Tree Removed!");
					}

					// Place some people here!
					for (int i = 0; i < STARTING_WORKERS; i++) {
						GC.inst.GenerateWorkerAt (spot);
					}

					br = true;
					break;

				}
			}

			dist += 1;
		}

		// There is no longer any need for this game object.
		Destroy (this);

	}

	void GenerateRiver(Coord c, float dir){

		// If we reached the end of the map, break!!!
		if (GC.inst.map.InMap(c) == false) {
			return;
		}

		// If we hit water break!!!
		if (GC.inst.map.GetTileAt(c) != null && GC.inst.map.GetTileAt(c).tileType == "water"){
			return;
		}

		// Convert the float direction to an integer one, with some randomness, so that the river appears to be going in roughly the float direction.
		int idir = (int)(dir + Random.Range (-0.5f, 0.5f));
		while (idir < 0) {
			idir += 6;
		}
		while (idir > 5) {
			idir -= 6;
		}

		Coord next = c.GetNeighbours () [idir];

		// Set the tiletype here to that of a river.
		GC.inst.map.SetTileAt (c, new Tile ("water", -1));

		// Change the direction slightly.
		float change = Mathf.Sqrt(Random.Range(0f, MAX_RIVER_DIR_CHANGE * MAX_RIVER_DIR_CHANGE));
		if (Random.Range(0,2) == 0) {
			change *= -1;
		}
		dir += change;

		GenerateRiver (next, dir);

	}

	public void CreateHex (int q, int r){

		Coord c = new Coord (q, r);

		GameObject hexGO = Instantiate (hexagonPrefab, c.GetWorldCoords(), Quaternion.identity, transform);

		hexGO.transform.name = "Hex - (" + q.ToString () + ", " + r.ToString () + ")";

		if (GC.inst.map.GetTileAt (c) == null) {
			
			GC.inst.map.SetTileAt (c, new Tile ("grass", -1));
		}

		// Find out the saturation of this tile.
		int sat = 0;
		foreach (Coord coord in c.GetWithin(SATURATION_DISTANCE)) {
			Tile t = GC.inst.map.GetTileAt (coord);
			if ( t != null && t.tileType == "water" ) {
				sat += SATURATION_DISTANCE - coord.DistanceTo(c);
			}
		}
		if (sat > MAX_SAT){
			sat = MAX_SAT;
		}
		GC.inst.map.GetTileAt (c).saturation = sat;

		// Set the texture of this tile.
		string tileType = GC.inst.map.GetTileAt (c).tileType;
		MeshRenderer mr = hexGO.GetComponent<MeshRenderer> ();
		mr.material.mainTexture = Resources.Load<Texture>(tileType);

		// If the tiletype is grass, set the visual to indicate saturation level.
		if (tileType == "grass") {
			mr.material.color = Color.Lerp (NO_SAT_COLOR, FULL_SAT_COLOR, ((float) sat)/MAX_SAT);
		}

		// Shall there be trees here?
		if (tileType == "grass") {
			int nTrees = (int) (Mathf.PerlinNoise (seedX + hexGO.transform.position.x / TREE_WAVELENGTH, seedY + hexGO.transform.position.y / TREE_WAVELENGTH) * (treeMax + 1) - treeMin);

			GC.inst.map.GetTileAt (c).nTrees = nTrees;
			
			while (nTrees > 0) {
				Vector3 pos = RandomCirclePos ();
				Instantiate (treePrefab, hexGO.transform.position + pos + Vector3.back * 3, Quaternion.identity, hexGO.transform);
				nTrees -= 1;
			}
		}

		hexGO.GetComponent<Hex> ().SetCoords (c);

	}
}
