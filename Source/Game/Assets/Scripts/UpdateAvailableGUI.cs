using UnityEngine;
using TMPro;

public class UpdateAvailableGUI : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] TextMeshProUGUI updateAvailableTitle;

    public static UpdateAvailableGUI Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ShowContent(string newVersion)
    {
        Refresh(newVersion);
        content.SetActive(true);
    }

    void Refresh(string newVersion)
    {
        updateAvailableTitle.text = "Update " + newVersion + " available!";
    }

    
    public void OpenDownloadPage()
    {
        Application.OpenURL(PersistentData.user.closedBetaTesterItchIoKeyURL);
    }
}