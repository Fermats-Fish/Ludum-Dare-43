using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobStructure : MonoBehaviour {

	public Sprite jobSymbol;

	public float jobDuration = Mathf.Infinity;
	public float deathChance = 0f;

	public bool destroyOnJobFinish = false;

	public string resourceGainedOnComplete = "";

	public BuildingType.AmountGained getAmountGained = null;

	public bool removeTreeOnComplete = false;

	public GameObject hoverObject;

	public bool beingWorked = false;

	public bool amountGainedPropToSat = false;

	public delegate void OnJobFinished();
	public OnJobFinished onJobFinished;

	public void Start(){

		// Add amount gained for trees.
		if (removeTreeOnComplete == true){
			getAmountGained = () => {
				return 1;
			};
		}

	}

	public void OnMouseEnter(){
		// If there isn't a hover object, create one.
		if (hoverObject == null && jobSymbol != null) {
			hoverObject = Instantiate (GC.inst.imagePrefab);
			hoverObject.transform.position = transform.position + Vector3.back;
			hoverObject.GetComponent<SpriteRenderer> ().sprite = jobSymbol;
		}

		// If the mouse is held down, then do OnMouseDown.
		if (Input.GetMouseButton(0)){
			OnMouseDown ();
		}
	}

	public void OnMouseDown(){

		// If no worker is working this yet...
		if (beingWorked == false) {

			// If there is an idle worker...
			if (GC.inst.idleWorkers.Count > 0) {
				// Tell the closest one to work on this structure.
				GC.inst.FindClosestIdleWorker (transform.position).GiveJob (this);
			} else {
				// Queue this job to the next available worker.
				GC.inst.jobQueue.Enqueue (this);

				// Set this as being worked, so that it can't be added to the job queue twice.
				beingWorked = true;
			}

		}
	}

	public void OnMouseExit(){
		if (hoverObject != null && beingWorked == false) {
			Destroy (hoverObject);
			hoverObject = null;
		}
	}

	public void Demolish(){
		if (hoverObject != null){
			Destroy (hoverObject);
		}
		Destroy (gameObject);
	}

}
