using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void LoadSceneAsync(SceneIndexes sceneIndex)
    {
        SceneManager.LoadSceneAsync((int)sceneIndex);
    }

    public static void LoadSceneAsync(int sceneBuildIndex)
    {
        SceneManager.LoadSceneAsync(sceneBuildIndex);
    }

    public static void LoadSceneAsync(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}

public enum SceneIndexes
{
    Register = 0,
    Main_Menu = 1,
    Matchmaking = 2,
    Card_Collection = 3,
    Shop = 4,
    Booster_Opening = 5
}