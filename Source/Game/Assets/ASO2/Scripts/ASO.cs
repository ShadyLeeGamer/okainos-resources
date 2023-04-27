using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ASO : MonoBehaviour {
	//TODO make frequency change stuff


	//GO Requirements
	public Camera[] Cameras;
	public Terrain[] Terrains;

	public bool setCamerasOnStart = true;
	public bool setTerrainsOnStart = true;

	//Quality
	public bool CameraRenderDistance = true;
	public float minCameraRenderDistance = 80;
	public float maxCameraRenderDistance = 500;

	public bool ShadowDistance = true;
	public float minShadowDistance = 25;
	public float maxShadowDistance = 200;

	public bool PixelLightCount = true;
	public int minPixelLightCount = 1;
	public int maxPixelLightCount = 15;

	public bool ShadowResolution = true;
	public enum ShadowResolutions {Off = 0, Low = 1, Medium = 2, High = 3, VeryHigh = 4};
	public ShadowResolutions minShadowResolution = ShadowResolutions.Off;
	public ShadowResolutions maxShadowResolution = ShadowResolutions.VeryHigh;

	public bool ShadowType = false;
	public enum ShadowTypes {Hard = 0, HardAndSoft = 1};
	public ShadowTypes minShadowType = ShadowTypes.Hard;
	public ShadowTypes maxShadowType = ShadowTypes.HardAndSoft;
 
	public bool LodBias = true;
	public float minLodBias = 0.3f;
	public float maxLodBias = 1.2f;

	public bool GlobalTextureLimit = true;
	public enum GlobalTextureLimits {Eighth = 0, Quarter = 1, Half = 2, Full = 3};
	public GlobalTextureLimits minGlobalTexureLimit = GlobalTextureLimits.Quarter;
	public GlobalTextureLimits maxGlobalTexureLimit = GlobalTextureLimits.Full;

	public bool AntiAliasing = true;
	public enum AntiAliasingLevels {Off = 0, Two = 1, Four = 2, Eight = 3};
	public AntiAliasingLevels minAntiAliasing = AntiAliasingLevels.Off;
	public AntiAliasingLevels maxAntiAliasing = AntiAliasingLevels.Eight;

	public bool RealtimeReflectionProbes = true;
	public float RealtimeReflectionProbesThreshold = 0.7f;

	public bool SoftParticles = true;
	public float SoftParticleThreshold  = 0.25f;

	//Terrain
	public bool DetailDistance = true;
	public float minDetailDistance = 0;
	public float maxDetailDistance = 200;

	public bool SoftVegitation = true;
	public float SoftVegitationThreshold = 0.6f;

	//Custom user vars
	public List<ASOCustom> custom = new List<ASOCustom>();  //TODO

	public enum FPSTypes {Raw, Avg, Adj};
	public FPSTypes fpsType = FPSTypes.Adj;

	public bool dontDestroyOnLoad = true;

	public int aggressiveness = 15; //Between 1 - 100
	public int deadZone = 5; //FPS
	public bool disableVsync = true;
	public bool autoLevel = true;

	public int DesiredFps = 60;
	public float currentLevel = 0.5f;

	public float settleDeadZone = 10; //settle unless they go extra above or extra below
	public bool isSettled = false; // if target framerate is reached settle unless they go below 

	public bool saveLevelToConfig = true;
	public bool loadLevelFromConfig = true;

	public bool doDebug = true;
	public bool updateApiEvents = true;
	public bool resetCamsTerrainsOnLoad = true;

	List<float> fpsList = new List<float>();
	public int fpsListLength = 30;
	public float rawFps;
	public float adjFps;
	public float avgFps;

	int frameCount;
	float deltaTime;
	int currentUpdate;

	void Awake () {
		if (disableVsync) {
			QualitySettings.vSyncCount = 0;
		}

		if (setCamerasOnStart) {
			autoSetCams ();
		}

		if (setTerrainsOnStart) {
			autoSetTerrains ();
		}

		if (loadLevelFromConfig) {
			LoadLevelFromConfig ();
		}

		if (dontDestroyOnLoad) {
			DontDestroyOnLoad (this.gameObject);
		}
	}

	void OnLevelWasLoaded (int level) { //Unity calls this function when a level is loaded.
		if(resetCamsTerrainsOnLoad) {
			reloadTerrainsCams ();
		}
	}

	void autoSetCams () {
		Cameras = Camera.allCameras;
	}

	void autoSetTerrains () {
		Terrains = Terrain.activeTerrains;
	}

	public void reloadTerrainsCams () {
		autoSetCams ();
		autoSetTerrains ();
		if (doDebug) {
			Debug.Log ("ASO: Reloaded terrains and cameras");
		}
	}

	//Gets current framerate
	void GetFrameRate () {
		deltaTime += Time.deltaTime;
		deltaTime /= 2.0f;
		rawFps = 1.0f / deltaTime;
	}

	void UpdateFpsList (float newFps) {
		fpsList.Add (newFps);

		if (fpsList.Count > fpsListLength) {
			fpsList.RemoveAt (0);
		}

		float average = 0;

		foreach (float f in fpsList) {
			average += f;
		}
		avgFps = average / fpsList.Count;
		//Debug.Log (average);
		//Update smoothed fps
		float x = 0;

		foreach(float f in fpsList) {
			if (f > x) {
				x = f;
			}
		}
		adjFps = x;
	}

	public void ValidateVars () {
		deadZone = Mathf.Clamp (deadZone, 1, 99);
		aggressiveness = Mathf.Clamp (aggressiveness, 1, 100);
		currentLevel = Mathf.Clamp (currentLevel, 0, 1);
		minCameraRenderDistance = Mathf.Max (minCameraRenderDistance, 1);
		maxCameraRenderDistance = Mathf.Max (minCameraRenderDistance, maxCameraRenderDistance);
		minShadowDistance = Mathf.Max (minShadowDistance, 1);
		maxShadowDistance = Mathf.Max (minShadowDistance, maxShadowDistance);
		minPixelLightCount = Mathf.Max (minPixelLightCount, 1);
		maxPixelLightCount = Mathf.Max (minPixelLightCount, maxPixelLightCount);

		int maxSR = Mathf.Max ((int)minShadowResolution, (int)maxShadowResolution); 
		switch (maxSR) {
		case 0:
			maxShadowResolution = ASO.ShadowResolutions.Off;
			break;
		case 1:
			maxShadowResolution = ASO.ShadowResolutions.Low;
			break;
		case 2:
			maxShadowResolution = ASO.ShadowResolutions.Medium;
			break;
		case 3:
			maxShadowResolution = ASO.ShadowResolutions.High;
			break;
		case 4:
			maxShadowResolution = ASO.ShadowResolutions.VeryHigh;
			break;
		}

		int maxST = Mathf.Max ((int)minShadowType, (int)maxShadowType); 
		switch (maxST) {
		case 0:
			maxShadowType = ASO.ShadowTypes.Hard;
			break;
		case 1:
			maxShadowType = ASO.ShadowTypes.HardAndSoft;
			break;
		}

		minLodBias = Mathf.Max (minLodBias, 0.01f);
		maxLodBias = Mathf.Max (minLodBias, maxLodBias);

		int maxGTL = Mathf.Max ((int)minGlobalTexureLimit, (int)maxGlobalTexureLimit); 
		switch (maxGTL) {
		case 0:
			maxGlobalTexureLimit = ASO.GlobalTextureLimits.Eighth;
			break;
		case 1:
			maxGlobalTexureLimit = ASO.GlobalTextureLimits.Quarter;
			break;
		case 2:
			maxGlobalTexureLimit = ASO.GlobalTextureLimits.Half;
			break;
		case 3:
			maxGlobalTexureLimit = ASO.GlobalTextureLimits.Full;
			break;
		}

		int maxAA = Mathf.Max ((int)minAntiAliasing, (int)maxAntiAliasing); 
		switch (maxAA) {
		case 0:
			maxAntiAliasing = ASO.AntiAliasingLevels.Off;
			break;
		case 1:
			maxAntiAliasing = ASO.AntiAliasingLevels.Two;
			break;
		case 2:
			maxAntiAliasing = ASO.AntiAliasingLevels.Four;
			break;
		case 3:
			maxAntiAliasing = ASO.AntiAliasingLevels.Eight;
			break;
		}

		Mathf.Clamp (RealtimeReflectionProbesThreshold, 0, 1);
		Mathf.Clamp (SoftParticleThreshold, 0, 1);
		minDetailDistance = Mathf.Max (minDetailDistance, 0.0f);
		maxDetailDistance = Mathf.Max (minDetailDistance, maxDetailDistance);
		Mathf.Clamp (SoftVegitationThreshold, 0, 1);
		Mathf.Clamp (fpsListLength, 1, 1200);
		Mathf.Clamp (settleDeadZone, 1, 100);
	}

	void UpdateCurrentLevel (float fpsDifference, float multiplier) {
		currentLevel += fpsDifference * aggressiveness * 0.00001f * multiplier;
		Mathf.Clamp (currentLevel, 0, 1);
	}

	void Update () {
		GetFrameRate ();
		UpdateFpsList (rawFps);

		if (autoLevel) {
			AutoLevel ();
		}

		ValidateVars ();
		doUpdates ();

	}

	void AutoLevel () {
		float fps = 0;
		if (fpsType == FPSTypes.Adj) { //Set the framerate to use in the calculation of settings
			fps = adjFps; 
		} else if (fpsType == FPSTypes.Raw) {
			fps = rawFps;
		} else if (fpsType == FPSTypes.Avg) {
			fps = avgFps;
		}

		float absDiff = Mathf.Abs (DesiredFps - fps); //Absolute value difference in fps
		float fpsDiff = fps - DesiredFps; // To get a proper negitive number diff to calculate current level
		if (absDiff > deadZone) { //Our framerate is not where we want it to be so change some settings
			if (absDiff > deadZone + settleDeadZone && Mathf.Abs (DesiredFps - avgFps) > deadZone + settleDeadZone) {
				isSettled = false;
			} else {
				isSettled = true;
			}

		}
		if (fps > DesiredFps + deadZone * .2f && currentLevel < 1) { //We have more fps than desired
			isSettled = false;
		}


		if (!isSettled) {
			//Adjust some things because we aren't settled
			UpdateCurrentLevel (fpsDiff, 1f);

		} else {
			if(currentLevel != PlayerPrefs.GetFloat("aso_level")) {
				SetLevelToConfig (); //Run this after it is done adjusting
			}
		}
	}

	public void LoadLevelFromConfig() {
		currentLevel = PlayerPrefs.GetFloat ("aso_level", currentLevel);
		if (doDebug) {
			Debug.Log ("ASO: Loaded level from PlayerPrefs");
		}
	}

	public void SetLevelToConfig() {
		PlayerPrefs.SetFloat ("aso_level", currentLevel);
		PlayerPrefs.Save ();
		if (doDebug) {
			Debug.Log ("ASO: Saved current level to PlayerPrefs");
		}
	}

	void doUpdates () {
		currentUpdate++;
		switch (currentUpdate) {
		case 1:
			if (CameraRenderDistance) {
				AdjCameraRenderDistance ();
			}
			break;
		case 2:
			if (ShadowDistance) {
				AdjShadowDistance ();
			}
			break;
		case 4:
			if (DetailDistance) {
				AdjDetailDistance ();
			}
			break;
		case 6:
			if (ShadowType || ShadowResolution) {
				AdjShadowStuff ();
			}
			break;
		case 7:
			if (LodBias) {
				AdjLodBias ();
			}
			break;
		case 8:
			if (GlobalTextureLimit) {
				AdjGlobalTextureLimit ();
			}
			break;
		case 9:
			if (AntiAliasing) {
				AdjAntiAliasing ();
			}
			break;
		case 10:
			if (SoftParticles) {
				AdjSoftParticles ();
			}
			break;
		case 11:
			if (RealtimeReflectionProbes) {
				AdjRealtimeReflectionProbes ();
			}
			break;
		case 12:
			if (SoftVegitation) {
				AdjSoftVegitation ();
			}
			break;
		case 13:
			if (custom.Count > 0 && updateApiEvents) {
				AdjUserSettings ();
			}
			break;
		case 14:
			if (PixelLightCount && !isSettled) {
				AdjPixelLightCount ();
			}
			break;
		case 15:
			currentUpdate = 0;
			break;
		}


	}

	void AdjUserSettings() { //BIG TODO
		foreach (ASOCustom t in custom) {
			
			if (t.type == ASOCustom.TileType.CallFunctionWithinRange) {
				
				if (t.threshDirection == ASOCustom.thresholdDirection.CallBelow) {
					if (currentLevel < t.threshold) { //It is below the level
						if (t.callOnce) { //Call once?
							if (!t.called) {
								t.go.SendMessage (t.functionToCall);
								t.called = true;
							}
						} else { //Nope just keep calling it
							t.go.SendMessage (t.functionToCall);
						}
					} else {
						if (t.called) {
							t.called = false;
						}
					}

				} else if (t.threshDirection == ASOCustom.thresholdDirection.CallAbove) {
					if (currentLevel > t.threshold) { //It is below the level
						if (t.callOnce) { //Call once?
							if (!t.called) {
								t.go.SendMessage (t.functionToCall);
								t.called = true;
							}
						} else { //Nope just keep calling it
							t.go.SendMessage (t.functionToCall);
						}
					} else {
						if (t.called) {
							t.called = false;
						}
					}
				}

			}

			if (t.type == ASOCustom.TileType.EnableDisableGO) {
				//Logic for enabling and disabling
				if (currentLevel < t.threshold) { //If the level is below threshold
					if (t.go.activeSelf == true) {//If it is active
						t.go.SetActive (false);
					}
				} else if (currentLevel > t.threshold) {
					if (t.go.activeSelf == false) {
						t.go.SetActive (true);
					}
				}

			}
		}
	}

	void AdjCameraRenderDistance () {
		float diff = maxCameraRenderDistance - minCameraRenderDistance;
		float amt = diff * currentLevel;
		float levelToBe = minCameraRenderDistance + amt;
		foreach (Camera cam in Cameras) {
			cam.farClipPlane = levelToBe;
		}
	}

	void AdjShadowDistance () {
		float diff = maxShadowDistance - minShadowDistance;
		float amt = diff * currentLevel;
		float levelToBe = minShadowDistance + amt;
		QualitySettings.shadowDistance = levelToBe;
	}

	void AdjPixelLightCount () {
		int diff = maxPixelLightCount - minPixelLightCount;
		int amt = Mathf.RoundToInt(diff * currentLevel);
		int levelToBe = minPixelLightCount + amt;
		QualitySettings.pixelLightCount = levelToBe;
	}

	void AdjDetailDistance () {
		float diff = maxDetailDistance - minDetailDistance;
		float amt = Mathf.RoundToInt(diff * currentLevel);
		float levelToBe = minDetailDistance + amt;
		foreach (Terrain t in Terrains) {
			t.detailObjectDistance = levelToBe;
		}
	}

	void AdjShadowStuff() {
		ShadowQuality a = new ShadowQuality ();
		if (ShadowType) { //If shadowType is enabled change it!
			a = AdjShadowType ();
		}

		int diff = (int)maxShadowResolution - (int)minShadowResolution;
		int amt = Mathf.RoundToInt (diff * currentLevel);
		int levelToBe = (int)minShadowResolution + amt;


		switch (levelToBe) {
		default:
			Debug.LogError ("ASO: Ummm shadow quality error. This message should never appear so please contact me about it.");
			break;
		case 0:
			if (ShadowType || ShadowResolution) {
				QualitySettings.shadows = ShadowQuality.Disable;
			}
			break;
		case 1:
			QualitySettings.shadows = ShadowQuality.All;
			if (ShadowType) {
				QualitySettings.shadows = a;
			}
			if (ShadowResolution) {
				QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Low;
			}
			break;
		case 2:
			QualitySettings.shadows = ShadowQuality.All;
			if (ShadowType) {
				QualitySettings.shadows = a;
			}
			if (ShadowResolution) {
				QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Medium;
			}
			break;
		case 3:
			QualitySettings.shadows = ShadowQuality.All;
			if (ShadowType) {
				QualitySettings.shadows = a;
			}
			if (ShadowResolution) {
				QualitySettings.shadowResolution = UnityEngine.ShadowResolution.High;
			}
			break;
		case 4:
			QualitySettings.shadows = ShadowQuality.All;
			if (ShadowType) {
				QualitySettings.shadows = a;
			}
			if (ShadowResolution) {
				QualitySettings.shadowResolution = UnityEngine.ShadowResolution.VeryHigh;
			}
			break;
		}
	}

	ShadowQuality AdjShadowType () {
		int diff = (int)maxShadowType - (int)minShadowType;
		int amt = Mathf.RoundToInt (diff * currentLevel);
		int levelToBe = (int)minShadowType + amt;

		switch(levelToBe) {
		default:
			Debug.LogError ("ASO: Ummm shadow type error. This message should never appear so please contact me about it.");
			return ShadowQuality.All; //Avoid null pointer exceptions :)
		case 0:
			return ShadowQuality.HardOnly;
		case 1:
			return ShadowQuality.All;
		}
	}

	void AdjLodBias () {
		float diff = maxLodBias - minLodBias;
		float amt = diff * currentLevel;
		float levelToBe = minLodBias + amt;
		QualitySettings.lodBias = levelToBe;
	}

	void AdjGlobalTextureLimit () {
		int diff = (int)maxGlobalTexureLimit - (int)minGlobalTexureLimit;
		int amt = Mathf.RoundToInt (diff * currentLevel);
		int levelToBe = (int)minGlobalTexureLimit + amt;

		//levelToBe = 0 1/8 | 1 1/4 | 2 1/2 | 3 1/1

		//Quality settings master texture limit 0 = none, 3 = 1/8
		int a = 3 - levelToBe;
		QualitySettings.masterTextureLimit = a;
	}

	void AdjAntiAliasing () {
		int diff = (int)maxAntiAliasing - (int)minAntiAliasing;
		int amt = Mathf.RoundToInt (diff * currentLevel);
		int levelToBe = (int)minAntiAliasing + amt;
		int a = 0;

		switch (levelToBe) {
		case 0:
			a = 0;
			break;
		case 1:
			a = 2;
			break;
		case 2:
			a = 4;
			break;
		case 3:
			a = 8;
			break;
		}

		QualitySettings.antiAliasing = a;
	}

	void AdjSoftParticles () {
		if (SoftParticleThreshold < currentLevel) {
			QualitySettings.softParticles = true;
		} else {
			QualitySettings.softParticles = false;
		}
	}

	void AdjRealtimeReflectionProbes () {
		if (currentLevel > RealtimeReflectionProbesThreshold) {
			QualitySettings.realtimeReflectionProbes = false;
		} else {
			QualitySettings.realtimeReflectionProbes = true;
		}
	}

	void AdjSoftVegitation () {
		if (currentLevel > SoftVegitationThreshold) {
			QualitySettings.softVegetation = true;
		} else {
			QualitySettings.softVegetation = false;
		}
	}


}
