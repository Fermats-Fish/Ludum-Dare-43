using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSystem {

	Dictionary<string, int> amounts = new Dictionary<string, int> ();

	public void Add(string resource, int amount){

		// See if the itemtype is in the dictionary.
		int curAmount = 0;
		if (amounts.ContainsKey(resource)) {
			amounts.TryGetValue (resource, out curAmount);
			amounts.Remove (resource);
		}

		amounts.Add (resource, curAmount + amount);

		UIController.inst.UpdateResourceText ();

	}

	public int Get(string resource){

		int curAmount = 0;
		amounts.TryGetValue (resource, out curAmount);
		return curAmount;

	}

}

public class RStack{

	public string resource;
	public int amount;

	public RStack(string resource, int amount){
		this.resource = resource;
		this.amount = amount;
	}

}
