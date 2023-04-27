using System.Collections;
using UnityEngine;

public class ASOCustom {

	public enum TileType {EnableDisableGO, CallFunctionWithinRange};
	public enum thresholdDirection {CallAbove, CallBelow};
	public TileType type;

	//Used for both
	public GameObject go;

	//calls function above or below the threshold
	public string functionToCall;
	public thresholdDirection threshDirection;
	public bool callOnce;
	public bool called;

	//threshold
	public float threshold;

}
