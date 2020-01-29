using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State{
	Idle, Walking, Working
};

public class Worker : MonoBehaviour {

	const float grassMoveSpeed = 1f;
	const float waterMoveSpeed = 0.5f;
	const float closeEnough = 0.3f;

	JobStructure job = null;
	ProgressBar bar = null;
	float progress = 0f;

	State state = State.Idle;

	void Start () {
		GC.inst.workers.Add (this);
		SetIdle ();
	}

	void Update () {
		float dt = Time.deltaTime;

		if (state == State.Walking) {

			// Get vector to destination.
			Vector2 delta = new Vector2 (job.transform.position.x - transform.position.x, job.transform.position.y - transform.position.y);


			// Work out our current move speed.
			float curMoveSpeed;

			// See if the tile type we are over is water.
			if (GC.inst.map.GetTileAt(GC.inst.GetHexCoordAt (transform.position)).tileType == "water") {
				curMoveSpeed = waterMoveSpeed;
			} else {
				curMoveSpeed = grassMoveSpeed;
			}

			float distanceToMove = curMoveSpeed * dt;

			// If we are going to overshoot just being in range this frame, then just move close enough, and use the extra time working.
			if (delta.magnitude - closeEnough < distanceToMove){

				distanceToMove = delta.magnitude - closeEnough;

				dt -= distanceToMove/curMoveSpeed;

				state = State.Working;
				progress = 0f;
				bar = ProgressBar.CreateBar (transform.position, transform);

			}

			// Move the calculated distance.
			transform.Translate (delta.normalized * distanceToMove);

		}

		// If we are working...
		if (state == State.Working) {

			progress += dt / job.jobDuration;

			if(progress >= 1f){
				Destroy (bar.gameObject);

				// Add any resources that get added on completion.
				if (job.getAmountGained != null) {

					int prop = 1;
					if (job.amountGainedPropToSat){
						prop = 2 + GC.inst.map.GetTileAt(GC.inst.GetHexCoordAt (transform.position)).saturation;
					}

					GC.inst.rs.Add (job.resourceGainedOnComplete, prop*job.getAmountGained());
				}

				// If the job was a tree job, remove the number of trees on that tile.
				if (job.removeTreeOnComplete) {
					GC.inst.map.GetTileAt (GC.inst.GetHexCoordAt (transform.position)).nTrees -= 1;
				}

				// If this job destroys the game object it is connected to, destroy it.
				if (job.hoverObject != null) {
					Destroy (job.hoverObject);
					job.hoverObject = null;
				}

				if (job.destroyOnJobFinish){
					job.Demolish();
				}

				// Otherwise set it to not being worked anymore.
				else{
					job.beingWorked = false;
				}

				// Run the function that it is to be run on completion.
				if (job.onJobFinished != null) {
					job.onJobFinished ();
				}

				JobStructure js = job;

				SetIdle ();

				// See if the worker dies...
				if (Random.Range(0f, 1f) < js.deathChance){
					Die ();
					return;
				}

			} else {
				bar.SetProgress (1f - progress);
			}
		}
	}

	public void SetIdle(){
		// First see if there is a job available...
		if (GC.inst.jobQueue.Count > 0){
			GiveJob (GC.inst.jobQueue.Dequeue());
		} else {
			GC.inst.idleWorkers.Add (this);
			UIController.inst.UpdateIdleWorkerText ();
			state = State.Idle;
		}
	}

	public void GiveJob(JobStructure j){

		job = j;
		state = State.Walking;
		GC.inst.idleWorkers.Remove (this);
		UIController.inst.UpdateIdleWorkerText ();

		job.beingWorked = true;
	}

	public void Die(){
		GC.inst.workers.Remove (this);
		GC.inst.idleWorkers.Remove (this);
		UIController.inst.UpdateIdleWorkerText ();
		print ("Worker dead!");

		if (state != State.Idle) {
			job.beingWorked = false;
			job.OnMouseDown ();
		}

		Destroy (gameObject);
	}

}
