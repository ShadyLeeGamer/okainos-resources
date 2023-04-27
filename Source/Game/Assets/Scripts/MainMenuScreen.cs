using UnityEngine;

public class MainMenuScreen : MonoBehaviour
{
    WelcomeBetaTesterGUI welcomeBetaTesterPopUpContent;

    public AccountGUI AccountGUI { get; private set; }

    public static MainMenuScreen Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AccountGUI = AccountGUI.Instance;

        AccountGUI.ShowContent();

        welcomeBetaTesterPopUpContent = WelcomeBetaTesterGUI.Instance;
        if (!PersistentData.user.isWelcomed)
        {
            welcomeBetaTesterPopUpContent.ShowContent();
            PersistentData.user.isWelcomed = true;
            PersistentData.user.UpdateToCloud(isUpdated => { });
        }
    }
}