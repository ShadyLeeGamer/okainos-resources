#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ASO)), DisallowMultipleComponent]
public class ASOInspector : Editor {

	//Quality
	bool crd;
	bool sd;
	bool plc;
	bool sr;
	bool st;
	bool lb;
	bool gtl;
	bool aa;
	bool dd;
	bool rrp;
	bool sp;

	//Terrain
	bool sv;

	bool apiFoldout;

	ASO aso;
	ASOTooltipEngine ti;

	private void OnEnable () {
		aso = (ASO)target; //get target
		ti = new ASOTooltipEngine();
	}

	public override void OnInspectorGUI () {
		//DrawDefaultInspector ();


		EditorGUILayout.BeginVertical ();

		EditorGUILayout.LabelField ("Main ASO settings", EditorStyles.boldLabel);
		if (Application.isPlaying) {
			EditorGUILayout.LabelField ("Fps: " + Mathf.Round (aso.rawFps));
		}

		aso.currentLevel = EditorGUILayout.Slider (new GUIContent("Dynamic Qualtiy level", ti.currentLevel), aso.currentLevel, 0f, 1f);
		aso.DesiredFps = EditorGUILayout.IntField (new GUIContent("Desired Framerate", ti.DesiredFps), aso.DesiredFps);
		aso.autoLevel = EditorGUILayout.Toggle (new GUIContent("Auto Level", ti.autoLevel), aso.autoLevel);

		aso.aggressiveness = EditorGUILayout.IntField (new GUIContent("Aggressiveness", ti.agressiveness), aso.aggressiveness);
		aso.deadZone = EditorGUILayout.IntField (new GUIContent("Framerate DeadZone", ti.deadZone), aso.deadZone);

		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Quality Settings", EditorStyles.boldLabel);

		crd = EditorGUILayout.Foldout (crd, "Camera Render Distance");
		if (crd) {
			aso.CameraRenderDistance = EditorGUILayout.Toggle ("Enabled", aso.CameraRenderDistance);
			aso.minCameraRenderDistance = EditorGUILayout.FloatField ("Min", aso.minCameraRenderDistance);
			aso.maxCameraRenderDistance = EditorGUILayout.FloatField ("Max", aso.maxCameraRenderDistance);
		}

		sd = EditorGUILayout.Foldout (sd, "Shadow Distance");
		if (sd) {
			aso.ShadowDistance = EditorGUILayout.Toggle ("Enabled", aso.ShadowDistance);
			aso.minShadowDistance = EditorGUILayout.FloatField ("Min", aso.minShadowDistance);
			aso.maxShadowDistance = EditorGUILayout.FloatField ("Max", aso.maxShadowDistance);
		}

		plc = EditorGUILayout.Foldout (plc, "Pixel Light Count");
		if (plc) {
			aso.PixelLightCount = EditorGUILayout.Toggle ("Enabled", aso.PixelLightCount);
			aso.minPixelLightCount = EditorGUILayout.IntField ("Min", aso.minPixelLightCount);
			aso.maxPixelLightCount = EditorGUILayout.IntField ("Max", aso.maxPixelLightCount);
		}

		sr = EditorGUILayout.Foldout (sr, "Shadow Resolution");
		if (sr) {
			aso.ShadowResolution = EditorGUILayout.Toggle ("Enabled", aso.ShadowResolution);
			aso.minShadowResolution = (ASO.ShadowResolutions)EditorGUILayout.EnumPopup ("Min", aso.minShadowResolution);
			aso.maxShadowResolution = (ASO.ShadowResolutions)EditorGUILayout.EnumPopup ("Max", aso.maxShadowResolution);
		}

		st = EditorGUILayout.Foldout (st, "Shadow Type");
		if (st) {
			aso.ShadowType = EditorGUILayout.Toggle ("Enabled", aso.ShadowType);
			aso.minShadowType = (ASO.ShadowTypes)EditorGUILayout.EnumPopup ("Min", aso.minShadowType);
			aso.maxShadowType = (ASO.ShadowTypes)EditorGUILayout.EnumPopup ("Max", aso.maxShadowType);
		}

		lb = EditorGUILayout.Foldout (lb, "LOD");
		if (lb) {
			aso.LodBias = EditorGUILayout.Toggle ("Enabled", aso.LodBias);
			aso.minLodBias = EditorGUILayout.FloatField ("Min", aso.minLodBias);
			aso.maxLodBias = EditorGUILayout.FloatField ("Max", aso.maxLodBias);
		}

		gtl = EditorGUILayout.Foldout (gtl, "Texture Resolution Limit");
		if (gtl) {
			aso.GlobalTextureLimit = EditorGUILayout.Toggle ("Enabled", aso.GlobalTextureLimit);
			aso.minGlobalTexureLimit = (ASO.GlobalTextureLimits)EditorGUILayout.EnumPopup ("Min", aso.minGlobalTexureLimit);
			aso.maxGlobalTexureLimit = (ASO.GlobalTextureLimits)EditorGUILayout.EnumPopup ("Max", aso.maxGlobalTexureLimit);
		}

		aa = EditorGUILayout.Foldout (aa, "Anit-Aliasing");
		if (aa) {
			aso.AntiAliasing = EditorGUILayout.Toggle ("Enabled", aso.AntiAliasing);
			aso.minAntiAliasing = (ASO.AntiAliasingLevels)EditorGUILayout.EnumPopup ("Min", aso.minAntiAliasing);
			aso.maxAntiAliasing = (ASO.AntiAliasingLevels)EditorGUILayout.EnumPopup ("Max", aso.maxAntiAliasing);
		}

		rrp = EditorGUILayout.Foldout (rrp, "Realtime Reflection Probes");
		if (rrp) {
			aso.RealtimeReflectionProbes = EditorGUILayout.Toggle ("Enabled", aso.RealtimeReflectionProbes);
			aso.RealtimeReflectionProbesThreshold = EditorGUILayout.FloatField (new GUIContent("Threshold", ti.GLOBAL_levelthreshold), aso.RealtimeReflectionProbesThreshold);
		}

		sp = EditorGUILayout.Foldout (sp, "Soft Particles");
		if (sp) {
			aso.SoftParticles = EditorGUILayout.Toggle ("Enabled", aso.SoftParticles);
			aso.SoftParticleThreshold = EditorGUILayout.FloatField (new GUIContent("Threshold", ti.GLOBAL_levelthreshold), aso.SoftParticleThreshold);
		}


		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Terrain Settings", EditorStyles.boldLabel);

		dd = EditorGUILayout.Foldout (dd, "Detail Distance");
		if (dd) {
			aso.DetailDistance = EditorGUILayout.Toggle ("Enabled", aso.DetailDistance);
			aso.minDetailDistance = EditorGUILayout.FloatField ("Min", aso.minDetailDistance);
			aso.maxDetailDistance = EditorGUILayout.FloatField ("Max", aso.maxDetailDistance);
		}

		sv = EditorGUILayout.Foldout (sv, "Soft Vegitation");
		if (sv) {
			aso.SoftVegitation = EditorGUILayout.Toggle ("Enabled", aso.SoftVegitation);
			aso.SoftVegitationThreshold = EditorGUILayout.FloatField (new GUIContent("Threshold", ti.GLOBAL_levelthreshold), aso.SoftVegitationThreshold);
		}

		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("API Settings", EditorStyles.boldLabel);
		aso.updateApiEvents = EditorGUILayout.Toggle ("Run API event updates", aso.updateApiEvents);
		apiFoldout = EditorGUILayout.Foldout (apiFoldout, "API Driven events");
		if(apiFoldout) {
			if (aso.custom.Count > 0) {
				EditorGUILayout.LabelField ("Total Events: " + aso.custom.Count);
			} else {
				
				EditorGUILayout.LabelField ("No custom events active. You can define events using the api.");
				EditorGUILayout.LabelField ("Check the readme for more info about the api", EditorStyles.miniLabel);
			}
		}

		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Other Settings", EditorStyles.boldLabel);

		aso.fpsType = (ASO.FPSTypes)EditorGUILayout.EnumPopup(new GUIContent("Fps Type", ti.fpsType), aso.fpsType);
		aso.fpsListLength = EditorGUILayout.IntField (new GUIContent("Smoothed Fps Frames", ti.fpsListLength), aso.fpsListLength);
		aso.disableVsync = EditorGUILayout.Toggle (new GUIContent("Disable Vsync", ti.disableVsync), aso.disableVsync);
		aso.settleDeadZone = EditorGUILayout.FloatField (new GUIContent("Settled deadzone", ti.settleDeadZone), aso.settleDeadZone);
		aso.doDebug = EditorGUILayout.Toggle (new GUIContent("Debug", ti.doDebug), aso.doDebug);
		aso.dontDestroyOnLoad = EditorGUILayout.Toggle (new GUIContent("Dont Destroy On Load", ti.dontDestroyOnLoad), aso.dontDestroyOnLoad);
		aso.resetCamsTerrainsOnLoad = EditorGUILayout.Toggle (new GUIContent("Refresh Cams/Terrains OnLoad", ti.resetCamsTerrainsOnLoad), aso.resetCamsTerrainsOnLoad);

		EditorGUILayout.EndVertical ();

		aso.ValidateVars ();
	}

}
#endif