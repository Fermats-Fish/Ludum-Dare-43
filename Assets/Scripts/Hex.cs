using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coord {

	public int q;
	public int r;

	public Coord(int q, int r){
		this.q = q;
		this.r = r;
	}

	public static Vector3 AxialToWorld(int q, int r){

		float x = Hex.R3 * (float) q - Hex.HR3 * (float) r;
		float y = 1.5f * (float) r;

		return new Vector3 (x, y, 0f);

	}

	public int DistanceTo(Coord other){
		int dr = other.r - r;
		int dq = other.q - q;

		// Convert to a different axial basis, which can be used to calculate the distance using cube coordinate tricks.
		dq -= dr;

		return (Mathf.Abs (dr) + Mathf.Abs (dq) + Mathf.Abs (dr + dq)) / 2;

	}

	public Vector3 GetWorldCoords(){

		return AxialToWorld(q, r);

	}

	public Coord[] GetNeighbours(){

		Coord[] array = new Coord[]{
			new Coord(q+1, r+1), // Top Right
			new Coord(q+1, r  ), // Right
			new Coord(q  , r-1), // Bottom Right
			new Coord(q-1, r-1), // Bottom Left
			new Coord(q-1, r  ), // Left
			new Coord(q  , r+1), // Top Left
		};

		return array;

	}

	// Returns all hexes within distance N of a hex.
	public List<Coord> GetWithin(int N){

		List<Coord> coords = new List<Coord> ();

		// Use a cube coord formula.
		for (int nq = -N; nq <= N; nq++) {
			for (int y = Mathf.Max(-N, -N-nq); y <= Mathf.Min(N, N-nq); y++) {
				
				// Transform it back into our coordinate system.
				int nr = y + nq;

				Coord c = new Coord (q + nq, r + nr);

				if (GC.inst.map.InMap(c)) {
					coords.Add (c);
				}

			}
		}

		return coords;
	}

	public override string ToString(){
		return "(" + q.ToString () + ", " + r.ToString () + ")";
	}

}

public class Hex : MonoBehaviour {
	
	public static readonly float R3 = Mathf.Sqrt(3);
	public static readonly float HR3 = Mathf.Sqrt (3) / 2f;
	public static readonly float QR3 = Mathf.Sqrt (3) / 4f;

	public Coord coord;

	public void SetCoords(Coord coord){
		this.coord = coord;
	}

	void OnMouseDown(){
		//print ( GC.inst.map.GetTileAt(coord).saturation.ToString() + ", " + GC.inst.map.GetTileAt(coord).nTrees.ToString() );
	}

	static readonly Vector3[] verts = new Vector3[]{
		new Vector3 (0f, 0f),           // Centre
		new Vector3 (0f, 1f),           // Top
		new Vector3 (Hex.HR3, 0.5f),  // Top Right
		new Vector3 (Hex.HR3, -0.5f), // Bottom Right
		new Vector3 (0f, -1f),          // Bottom
		new Vector3 (-Hex.HR3, -0.5f),// Bottom Left
		new Vector3 (-Hex.HR3, 0.5f), // Top Left
	};

	static readonly Vector2[] boundary = new Vector2[]{
		new Vector2 (0f, 1f),           // Top
		new Vector2 (Hex.HR3, 0.5f),  // Top Right
		new Vector2 (Hex.HR3, -0.5f), // Bottom Right
		new Vector2 (0f, -1f),          // Bottom
		new Vector2 (-Hex.HR3, -0.5f),// Bottom Left
		new Vector2 (-Hex.HR3, 0.5f), // Top Left
	};

	static readonly int[] hexTris = new int[]{
		0, 1, 2,
		0, 2, 3,
		0, 3, 4,
		0, 4, 5,
		0, 5, 6,
		0, 6, 1
	};

	static readonly Vector2[] uv = new Vector2[]{
		new Vector2( 0.5f, 0.5f ),
		new Vector2( 0.5f, 1f ),
		new Vector2( 1f, 1f ),
		new Vector2( 1f, 0f ),
		new Vector2( 0.5f, 0f ),
		new Vector2( 0f, 0f ),
		new Vector2( 0f, 1f )
	};

	void Start () {
		GenerateMesh ();
	}

	void GenerateMesh() {

		Mesh mesh = new Mesh ();
		mesh.vertices = verts;
		mesh.triangles = hexTris;
		mesh.uv = uv;
		mesh.RecalculateNormals ();

		GetComponent<MeshFilter> ().mesh = mesh;

		GetComponent<PolygonCollider2D> ().SetPath (0, boundary);
	}
}
