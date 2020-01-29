using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBuildingGhost : MonoBehaviour {

	BuildingType bt;

	public void SetBuildingType(BuildingType buildingType){

		bt = buildingType;

		// Set the image for the ghost.
		GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> (bt.name);

	}

	void Update(){

		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		// Follow the mouse!
		transform.position = new Vector3 ( mousePos.x, mousePos.y, -7 );

		// When the mouse button is pressed...
		if (Input.GetMouseButtonDown(0)) {

			// This becomes a worksite... provided that the building materials are all there.
			foreach (RStack stack in bt.buildCost) {
				if (GC.inst.rs.Get(stack.resource) < stack.amount) {
					Destroy (gameObject);
					print ("Not enough resources!");
					return;
				}
			}

			// Find the current hex.
			Coord c = GC.inst.GetHexCoordAt (transform.position);
			if (c == null){
				Destroy (gameObject);
				return;
			}

			// Double check that this is a valid hex, and if not simply return.
			Tile t = GC.inst.map.GetTileAt (c);
			if (t == null || t.tileType == "water" || t.nTrees > 0 || t.structure != null) {
				Destroy (gameObject);
				if (t==null){
					print ("Tile null");
				} else if (t.tileType == "water"){
					print ("Water");
				} else if (t.nTrees > 0){
					print ("Trees = " + t.nTrees.ToString ());
				} else if (t.structure != null){
					print ("Structure = " + t.structure);
				}
				return;
			}

			// Use up building materials.
			foreach (RStack stack in bt.buildCost) {
				GC.inst.rs.Add (stack.resource, -stack.amount);
			}

			// Move the ghost to the hex center.
			transform.position = c.GetWorldCoords () + Vector3.back;

			// Set a bulding site structure here so that no other buildings can be scheduled to be placed here.
			t.structure = "buildingSite";

			// Add a job structure here, and assign a worker to it.
			JobStructure js = gameObject.AddComponent<JobStructure> ();

			// Set all of the variables of the job structure.
			js.jobDuration = bt.buildTime;
			js.deathChance = 0f;
			js.destroyOnJobFinish = false;
			js.resourceGainedOnComplete = "";
			js.getAmountGained = null;

			// Setup what happens on job finish.
			js.onJobFinished = () => {

				GameObject go = js.gameObject;

				// Make fully opaque.
				go.GetComponent<SpriteRenderer>().color = Color.white;

				// Generate people equal to housing.
				Coord cd = GC.inst.GetHexCoordAt (go.transform.position);
				for (int i = 0; i < bt.houses; i++) {
					GC.inst.GenerateWorkerAt(cd);
				}

				// Delete the old job structure.
				Destroy(js);

				// Create a new job structure, if this structure has a job associated with it.
				if (bt.jobDuration >= 0){
					JobStructure njs = go.AddComponent<JobStructure>();
					njs.jobSymbol = Resources.Load<Sprite>(bt.hoverSymbolName);
					njs.jobDuration = bt.jobDuration;
					njs.deathChance = bt.jobDeathChance;
					njs.resourceGainedOnComplete = bt.resourceGainedOnJobComplete;
					njs.getAmountGained = bt.getAmountGained;
					njs.amountGainedPropToSat = bt.propToSaturation;
				}

				// Add structure to the tile.
				GC.inst.map.GetTileAt(cd).structure = bt.name;

			};

			// Assign a worker to this structure.
			js.OnMouseDown ();

			// Remove this component.
			Destroy (this);

		}

	}

}
