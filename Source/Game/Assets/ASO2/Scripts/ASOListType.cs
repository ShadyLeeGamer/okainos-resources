using UnityEngine;
using System;

[Serializable]
public struct ASOTile {
	public enum TileType {EnableDisableGO, CallFunctionWithinRange};
	public enum thresholdDirection {CallAbove, CallBelow};

	//Used for both
	public GameObject go;
	public TileType type;

	//calls function above or below the threshold
	public string functionToCall;
	public thresholdDirection threshDirection;
	public bool callOnce;
	public bool called;

	//threshold
	public float threshold;

}