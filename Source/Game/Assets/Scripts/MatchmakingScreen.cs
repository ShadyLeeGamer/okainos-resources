using UnityEngine;

public class MatchmakingScreen : MonoBehaviour
{
    [SerializeField] GameObject cardSelectionWindow;
    [SerializeField] GameObject characterSelectionWindow;

    public static MatchmakingScreen Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void ChangeWindow(bool toCardSelectionWindow)
    {
        cardSelectionWindow.SetActive(toCardSelectionWindow);
        characterSelectionWindow.SetActive(!toCardSelectionWindow);
    }

    public void Battle()
    {
        PersistentObject.Instance.gameObject.SetActive(false);
    }
}