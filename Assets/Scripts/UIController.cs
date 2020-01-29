using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public static UIController inst;

	public GameObject resourceIconPrefab;
	public GameObject godIconPrefab;

	public GameObject buildBuildingButtonPrefab;
	public GameObject buildBuildingGhostPrefab;

	public Text idleWorkerText;

	public GameObject godPanel;
	public GameObject resourcePanel;

	public GameObject rightPanel;

	Dictionary<string, Text> resourceText = new Dictionary<string, Text>();
	Dictionary<string, Text> godText = new Dictionary<string, Text>();

	public Text timeText;

	void Start () {
		inst = this;
	}

	public void SetupVisuals(){

		// Generate god panel.
		foreach (Sprite sprite in Resources.LoadAll<Sprite>("gods")) {
			GameObject go = Instantiate (godIconPrefab, godPanel.transform);
			go.GetComponent<Image> ().sprite = sprite;
			godText.Add (sprite.name, go.GetComponentInChildren<Text> ());
		}

		// Genearte resource panel.
		foreach (Sprite sprite in Resources.LoadAll<Sprite>("resources")) {
			GameObject go = Instantiate (resourceIconPrefab, resourcePanel.transform);
			go.GetComponent<Image> ().sprite = sprite;
			resourceText.Add (sprite.name, go.GetComponentInChildren<Text> ());
		}

		UpdateResourceText ();

		// Generate the buildings.
		foreach (BuildingType bt in BuildingType.buildingTypes) {

			// Instantiate a button.
			GameObject go = Instantiate (buildBuildingButtonPrefab, rightPanel.transform);

			// Set the sprite based on the name.
			go.GetComponent<Image> ().sprite = Resources.Load<Sprite> (bt.name);

			// Setup the resource costs.
			foreach (RStack stack in bt.buildCost) {
				GameObject bc = Instantiate (resourceIconPrefab, go.transform.GetChild(0));
				bc.GetComponent<Image> ().sprite = Resources.Load<Sprite>(stack.resource);
				bc.GetComponentInChildren<Text> ().text = stack.amount.ToString ();
			}

			// When you click the button...
			go.GetComponent<Button>().onClick.AddListener(() => {
				BuildingTypeSelected(bt);
			});

		}
	}

	public void BuildingTypeSelected(BuildingType bt){

		// Instantiate a build building ghost, and set its building type.
		Instantiate (buildBuildingGhostPrefab).GetComponent<BuildBuildingGhost>().SetBuildingType(bt);

	}

	public void UpdateResourceText(){
		foreach (string key in godText.Keys) {
			Text t;
			godText.TryGetValue (key, out t);
			t.text = GC.inst.rs.Get (key).ToString() + "%";
		}
		foreach (string key in resourceText.Keys) {
			Text t;
			resourceText.TryGetValue (key, out t);
			t.text = GC.inst.rs.Get (key).ToString();
		}
	}

	public void UpdateIdleWorkerText(){
		idleWorkerText.text = "Idle Workers: " + GC.inst.idleWorkers.Count.ToString() + "/" + GC.inst.workers.Count.ToString();
	}

}
