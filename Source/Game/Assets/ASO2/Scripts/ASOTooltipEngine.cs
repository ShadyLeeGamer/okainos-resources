public class ASOTooltipEngine {

	public string GLOBAL_levelthreshold = "This setting is enabled above the threshold and disabled below the threshold. the threshold is between 0 and 1";

	public string doDebug = "Debug what is happening with the script. All errors are shown despite this setting";
	public string settleDeadZone = "(int FPS) This is a buffer added to the deadzone when the script is in settle mode. This is done to reduce quality changes if the framerate spikes up or down for more than a couple frames";
	public string disableVsync = "This setting is recommended because it will imporove the performance of aso.";
	public string fpsListLength = "(int FPS) The length of an array to keep the adjusted framerate more stable to avoid unneeded changes to the level.";
	public string useSmoothedFps = "Use an adjusted smoother fps for adjusting the quality level, this improves the performance of the script and it is highly recommended to keep it on. Otherwise the script may not perform as well as you might like";
	public string currentLevel = "(float 0.0-1.0) This is slider to adjust all of the settings relitive to it. This is changed automatically with 'auto level' enabled and it is changeable with an api call or manually selecting a point on the slider via the inspector";
	public string DesiredFps = "(int FPS) Desired framerate, ASO will target the fps to this when auto level is enabled";
	public string autoLevel = "Enables and disables ASO's automatic level setting functionality. Disable this when you want to just have ASO change settings when you want it(ex: you have your own menu with a slider that tells aso api to set the level)";
	public string agressiveness = "(int 0-100) Changes how much ASO auto level will change the level. More agressiveness will change the quality more drastically, at the possible cost more lag. Lower numbers are less reactive but will have less jittering.";
	public string deadZone = "(int FPS) How close to the desired framerate do you have to be to settle and stop changing settings.";
	public string dontDestroyOnLoad = "Prevent aso from being destroyed when game switches scenes";
	public string fpsType = "'Type' of fps to be used for auto leveling. Raw (Not recommended) is the current framerate on this frame, Adj(Recommended) is an adjusted fps, and avj which is the average over the length of smoothed fps frames";
	public string resetCamsTerrainsOnLoad = "Reset all cameras and terrains when a new scene loads. This option allows ASO to do this automatically. Enabled by default";
}
