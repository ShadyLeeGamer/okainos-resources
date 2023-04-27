#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using ASO2Api;

public class ASOEditor : EditorWindow {

	ASO aso;
	ASOApi api;

	bool asoOn;
	float setlevel;

	void getaso() {
		aso = GameObject.FindObjectOfType (typeof(ASO)) as ASO;
		api = GameObject.FindObjectOfType (typeof(ASOApi)) as ASOApi;
	}

	public void Start () {
		getaso ();
		setlevel = aso.currentLevel;
	}

	public void OnInspectorUpdate () {
		getaso ();
		Repaint ();
	}

	[MenuItem("ASO/Stats")]
	static void OpenWindow() {
		ASOEditor window = (ASOEditor)EditorWindow.GetWindow(typeof(ASOEditor));
		window.Show();
	}

	[MenuItem("ASO/New ASO Object")]
	static void newASO() {
		if (GameObject.FindObjectOfType (typeof(ASO)) as ASO) {
			Debug.LogError ("ASO: Failed to create NEW ASO because it already exists");
			return;
		}
		GameObject asoGo = new GameObject();
		asoGo.name = "ASO";
		asoGo.AddComponent<ASO> ();
		asoGo.AddComponent<ASO2Api.ASOApi> ();
		Selection.objects = new Object[] { asoGo };
	}

	void OnGUI () {
		if (aso) {
			if (!aso.isSettled && Application.isPlaying && aso.autoLevel) {
				GUILayout.Label ("...Adjusting...", EditorStyles.boldLabel);
			}
			GUILayout.Label ("Framerate:", EditorStyles.boldLabel);
			GUILayout.Label ("Fps: " + Mathf.Round (aso.rawFps));
			GUILayout.Label ("Average Fps: " + Mathf.Round (aso.avgFps));
			GUILayout.Label ("Adjusted Fps: " + Mathf.Round (aso.adjFps));

			GUILayout.Label ("Levels:", EditorStyles.boldLabel);

			GUILayout.Label ("Quality Level: " + Mathf.Round (aso.currentLevel * 100) / 100);

			GUILayout.Label ("Options:", EditorStyles.boldLabel);
			if (GUILayout.Button ("Reset PlayerPrefs", EditorStyles.miniButton)) {
				api.resetConfig ();
			}

			if (GUILayout.Button ("Refresh Cams/Terrains", EditorStyles.miniButton)) {
				api.autoReloadCamsTerrains ();
			}

			GUIDivider ();

			asoOn = GUILayout.Toggle (asoOn, "Auto Level");
			GUILayout.Label ((Mathf.Round(setlevel*100)/100).ToString(), EditorStyles.miniLabel);
			setlevel = GUILayout.HorizontalSlider (setlevel, 0, 1);
			if(GUILayout.Button("Set", EditorStyles.miniButton)) {
				aso.autoLevel = asoOn;
				aso.currentLevel = setlevel;
			}

		} else {
			GUILayout.Label ("NO ASO in project.", EditorStyles.boldLabel);
			GUILayout.Label ("To add one to the the 'ASO/New ASO Object' menu item");
		}
	}

	void GUIDivider () {
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
	}

}
#endif