using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GC : MonoBehaviour {

	public const int STARTING_GRAIN = 300;

	public GameObject imagePrefab;
	public GameObject workerPrefab;

	public static GC inst;

	public Map map;

	public List<Worker> idleWorkers = new List<Worker>();
	public List<Worker> workers = new List<Worker>();

	public ResourceSystem rs = new ResourceSystem();

	public Queue<JobStructure> jobQueue = new Queue<JobStructure>();

	const float TIME_BEFORE_CONSUMES_FOOD = 2f;
	public float foodTimer;

	const float TIME_BEFORE_LOSE_SUPPORT = 10f;
	public float supportTimer;

	public float time;

	void Start () {
		inst = this;

		// Create an empty map.
		map = new Map (WorldGenerator.WORLD_SIZE);

		// Generate the world.
		GetComponent<WorldGenerator> ().GenerateWorld ();

		// Genearte the resource system visuals and building visuals.
		UIController.inst.SetupVisuals ();

		// Start with some grain.
		rs.Add ("grain", STARTING_GRAIN);
		rs.Add ("harvest-god", 50);
		rs.Add ("food-god", 50);
	}

	void Update () {
		foodTimer += Time.deltaTime;
		supportTimer += Time.deltaTime;

		if (supportTimer > TIME_BEFORE_LOSE_SUPPORT){
			supportTimer -= TIME_BEFORE_LOSE_SUPPORT;
			rs.Add ("harvest-god", -1);
		}

		if (foodTimer > TIME_BEFORE_CONSUMES_FOOD){
			foodTimer -= TIME_BEFORE_CONSUMES_FOOD;
			foreach (Worker worker in workers) {
				if (rs.Get("grain") <= 0){
					// MAX one worker dead per frame.
					worker.Die ();
					return;
				} else {
					rs.Add ("grain", -1);
				}
			}
		}

		time += Time.deltaTime;
		UIController.inst.timeText.text = ((int)time).ToString ();
	}

	public void GenerateWorkerAt(Coord spot){

		// For now just make the game object.
		Instantiate (workerPrefab, spot.GetWorldCoords () + WorldGenerator.RandomCirclePos () + (Vector3.back * 2), Quaternion.identity);

	}

	public Coord GetHexCoordAt(Vector3 pos){

		// Do a raycast.
		RaycastHit2D[] hits = Physics2D.RaycastAll (pos, Vector3.zero);

		foreach (RaycastHit2D hit in hits) {
			if (hit.transform.GetComponent<Hex>() != null) {
				return hit.transform.GetComponent<Hex> ().coord;
			}
		}

		return null;

	}

	public Worker FindClosestIdleWorker(Vector3 pos){

		float minSquare = Mathf.Infinity;

		Worker closest = null;

		foreach (Worker worker in idleWorkers) {

			Vector3 wPos = worker.transform.position;
			wPos -= pos;
			float ds = wPos.x * wPos.x + wPos.y * wPos.y;

			if (ds < minSquare) {
				minSquare = ds;
				closest = worker;
			}

		}

		return closest;

	}

}