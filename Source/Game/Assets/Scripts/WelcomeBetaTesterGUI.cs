using UnityEngine;

public class WelcomeBetaTesterGUI : MonoBehaviour
{
    [SerializeField] GameObject content;

    public static WelcomeBetaTesterGUI Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ShowContent()
    {
        content.SetActive(true);
    }
}
