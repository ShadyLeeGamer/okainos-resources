namespace ASO2Api {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[RequireComponent(typeof(ASO))]
	public class ASOApi : MonoBehaviour {

		ASO aso;

		void Awake () {
			aso = GetComponent<ASO> ();
		}

		void Start () {
			aso = GetComponent<ASO> ();
		}

		public ASOApi getAso () {
			return this;
		}

		public void LoadLevelFromConfig() {
			aso.LoadLevelFromConfig ();
		}

		public void setAutoLevel (bool b) {
			aso.autoLevel = b;
		}

		public bool getAutoLevel () {
			return aso.autoLevel;
		}

		public void autoReloadCamsTerrains () {
			aso.reloadTerrainsCams ();
		}

		public void resetConfig () {
			PlayerPrefs.DeleteKey ("aso_level");
			Debug.Log ("ASO: Config reset");
		}

		public void addCustom_enableDisable (GameObject go, float threshold) {
			ASOCustom a = new ASOCustom ();
			a.type = ASOCustom.TileType.EnableDisableGO;
			a.go = go;
			a.threshold = threshold;
			aso.custom.Add (a);
		}

		public void addCustom_callFunction (GameObject go, float threshold, string function, ASOCustom.thresholdDirection direction, bool callOnce) {
			ASOCustom a = new ASOCustom ();
			a.type = ASOCustom.TileType.CallFunctionWithinRange;
			a.go = go;
			a.threshold = threshold;
			a.functionToCall = function;
			a.threshDirection = direction;
			a.callOnce = callOnce;
			aso.custom.Add (a);
		}

		public void removeCustom(GameObject go) {
			foreach (ASOCustom a in aso.custom) {
				if (a.go = go) {
					aso.custom.Remove (a);
				}
			}
		}

		public void setTerrains(Terrain[] terrains) {
			aso.Terrains = terrains;
		}

		public void setCameras(Camera[] cameras) {
			aso.Cameras = cameras;
		}

		public void saveLevelToConfig() {
			aso.SetLevelToConfig ();
		}

		public float getFps () {
			return aso.rawFps;
		}

		public int MinPixelLightCount {
			get {
				return aso.minPixelLightCount;
			}
			set {
				aso.minPixelLightCount = value;
			}
		}

		public int MaxPixelLightCount {
			get {
				return aso.maxPixelLightCount;
			}
			set {
				aso.maxPixelLightCount = value;
			}
		}

		public int aggressiveness {
			get {
				return aso.aggressiveness;
			}
			set {
				aso.aggressiveness = value;
			}
		}

		public int deadZone {
			get {
				return aso.deadZone;
			}
			set {
				aso.deadZone = value;
			}
		}

		public int DesiredFps {
			get {
				return aso.DesiredFps;
			}
			set {
				aso.DesiredFps = value;
			}
		}

		public int fpsListLength {
			get {
				return aso.fpsListLength;
			}
			set {
				aso.fpsListLength = value;
			}
		}



		public float settleDeadZone {
			get {
				return aso.settleDeadZone;
			}
			set {
				aso.settleDeadZone = value;
			}
		}

		public float currentLevel {
			get {
				return aso.currentLevel;
			}
			set {
				aso.currentLevel = value;
			}
		}

		public float MinCameraRenderDistance {
			get {
				return aso.minCameraRenderDistance;
			}
			set {
				aso.minCameraRenderDistance = value;
			}
		}

		public float MaxCameraRenderDistance {
			get {
				return aso.maxCameraRenderDistance;
			}
			set {
				aso.maxCameraRenderDistance = value;
			}
		}

		public float MinShadowDistance {
			get {
				return aso.minShadowDistance;
			}
			set {
				aso.minShadowDistance = value;
			}
		}

		public float MaxShadowDistance {
			get {
				return aso.maxShadowDistance;
			}
			set {
				aso.maxShadowDistance = value;
			}
		}

		public float MinLodBias {
			get {
				return aso.minLodBias;
			}
			set {
				aso.minLodBias = value;
			}
		}

		public float MaxLodBias {
			get {
				return aso.maxLodBias;
			}
			set {
				aso.maxLodBias = value;
			}
		}

		public float MinDetailDistance {
			get {
				return aso.minDetailDistance;
			}
			set {
				aso.minDetailDistance = value;
			}
		}

		public float MaxDetailDistance {
			get {
				return aso.maxDetailDistance;
			}
			set {
				aso.maxDetailDistance = value;
			}
		}



		public bool isSettled {
			get {
				return aso.isSettled;
			}
		}

		public bool doDebug {
			get {
				return aso.doDebug;
			}
			set {
				aso.doDebug = value;
			}
		}

		public bool autoLevel {
			get {
				return aso.autoLevel;
			}
			set {
				aso.autoLevel = value;
			}
		}

		public bool dontDestroyOnLoad {
			get {
				return aso.dontDestroyOnLoad;
			}
			set {
				aso.dontDestroyOnLoad = value;
			}
		}

		public bool CameraRenderDistance {
			get {
				return aso.CameraRenderDistance;
			}
			set {
				aso.CameraRenderDistance = value;
			}
		}

		public bool ShadowDistance {
			get {
				return aso.ShadowDistance;
			}
			set {
				aso.ShadowDistance = value;
			}
		}

		public bool PixelLightCount {
			get {
				return aso.PixelLightCount;
			}
			set {
				aso.PixelLightCount = value;
			}
		}

		public bool ShadowResolution {
			get {
				return aso.ShadowResolution;
			}
			set {
				aso.ShadowResolution = value;
			}
		}

		public bool ShadowType {
			get {
				return aso.ShadowType;
			}
			set {
				aso.ShadowType = value;
			}
		}

		public bool LodBias {
			get {
				return aso.LodBias;
			}
			set {
				aso.LodBias = value;
			}
		}

		public bool GlobalTextureLimit {
			get {
				return aso.GlobalTextureLimit;
			}
			set {
				aso.GlobalTextureLimit = value;
			}
		}

		public bool AntiAliasing {
			get {
				return aso.AntiAliasing;
			}
			set {
				aso.AntiAliasing = value;
			}
		}

		public bool RealtimeReflectionProbes {
			get {
				return aso.RealtimeReflectionProbes;
			}
			set {
				aso.RealtimeReflectionProbes = value;
			}
		}

		public bool SoftParticles {
			get {
				return aso.SoftParticles;
			}
			set {
				aso.SoftParticles = value;
			}
		}

		public bool DetailDistance {
			get {
				return aso.DetailDistance;
			}
			set {
				aso.DetailDistance = value;
			}
		}

		public bool SoftVegitation {
			get {
				return aso.SoftVegitation;
			}
			set {
				aso.SoftVegitation = value;
			}
		}


		public ASO.ShadowResolutions minShadowResolution {
			get {
				return aso.minShadowResolution;
			}
			set {
				aso.minShadowResolution = value;
			}
		}

		public ASO.ShadowResolutions maxShadowResolution {
			get {
				return aso.maxShadowResolution;
			}
			set {
				aso.maxShadowResolution = value;
			}
		}

		public ASO.ShadowTypes minShadowType {
			get {
				return aso.minShadowType;
			}
			set {
				aso.minShadowType = value;
			}
		}

		public ASO.ShadowTypes maxShadowType {
			get {
				return aso.maxShadowType;
			}
			set {
				aso.maxShadowType = value;
			}
		}

		public ASO.AntiAliasingLevels minAntiAliasing {
			get {
				return aso.minAntiAliasing;
			}
			set {
				aso.minAntiAliasing = value;
			}
		}

		public ASO.AntiAliasingLevels maxAntiAliasing {
			get {
				return aso.maxAntiAliasing;
			}
			set {
				aso.maxAntiAliasing = value;
			}
		}

		public ASO.GlobalTextureLimits minGlobalTexureLimit {
			get {
				return aso.minGlobalTexureLimit;
			}
			set {
				aso.minGlobalTexureLimit = value;
			}
		}

		public ASO.GlobalTextureLimits maxGlobalTexureLimit {
			get {
				return aso.maxGlobalTexureLimit;
			}
			set {
				aso.maxGlobalTexureLimit = value;
			}
		}

	}
}