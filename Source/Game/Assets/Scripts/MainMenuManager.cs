using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    MainMenuScreen screen;

    public static MainMenuManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        screen = MainMenuScreen.Instance;

        PersistentData.CacheUserData();

        screen.AccountGUI.Refresh();

        CardResourcesManager.LoadCardResources();

        PersistentData.CheckForUpdates();
    }

    public void LoadScene(string sceneName)
    {
        SceneLoader.LoadSceneAsync(sceneName);
    }
}