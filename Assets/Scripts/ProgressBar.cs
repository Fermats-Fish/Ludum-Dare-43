using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

	static GameObject PROGRESS_BAR_PREFAB;

	public static ProgressBar CreateBar( Vector2 coords, Transform t ){
		if (PROGRESS_BAR_PREFAB == null) {
			PROGRESS_BAR_PREFAB = Resources.Load<GameObject>("ProgressBarPrefab");
		}
		return Instantiate (PROGRESS_BAR_PREFAB, new Vector3 (coords.x - 0.35f, coords.y - 0.3f, -5f), Quaternion.identity, t).GetComponent<ProgressBar>();
	}

	public void SetProgress(float perunit){
		transform.localScale = new Vector3 (perunit, 1f, 1f);
	}

}
