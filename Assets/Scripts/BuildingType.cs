using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingType {

	public static BuildingType[] buildingTypes = new BuildingType[]{
		new BuildingType("farm",    new List<RStack>{ new RStack("wood", 10) }, 10f, "plow",      50f, 0f, "grain",       () => {return 20 * GC.inst.rs.Get("harvest-god") / 100;}, 0, true),
		new BuildingType("hut",     new List<RStack>{ new RStack("wood", 5) },  3f,  "",         -1f,  0f, "",            null,  3, false),
		new BuildingType("pyramid", new List<RStack>{ new RStack("wood", 10) }, 10f, "sacrifice", 10f, 1f, "harvest-god", () => {return 10;}, 0, false),
	};

	// Name also determines icon.
	public string name;
	public List<RStack> buildCost;
	public float buildTime;
	public string hoverSymbolName;
	public float jobDuration;
	public float jobDeathChance;
	public string resourceGainedOnJobComplete;
	public int houses;
	public bool propToSaturation;

	public delegate int AmountGained();
	public AmountGained getAmountGained;

	public BuildingType (string buildingName, List<RStack> buildCost, float buildTime, string hoverSymbolName, float jobDuration, float jobDeathChance, string resourceGainedOnJobComplete, AmountGained getAmountGained, int houses, bool propToSaturation){
		this.name = buildingName;
		this.buildCost = buildCost;
		this.buildTime = buildTime;
		this.hoverSymbolName = hoverSymbolName;
		this.jobDuration = jobDuration;
		this.jobDeathChance = jobDeathChance;
		this.resourceGainedOnJobComplete = resourceGainedOnJobComplete;
		this.getAmountGained = getAmountGained;
		this.houses = houses;
		this.propToSaturation = propToSaturation;
	}

}
