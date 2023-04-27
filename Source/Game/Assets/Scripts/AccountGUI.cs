using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class AccountGUI : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] GameObject friendsWindow;

    [SerializeField] TextMeshProUGUI usernameDisplay, pearlsDisplay, boosterChestsDisplay;

    public static AccountGUI Instance { get; private set; }

    FriendsManager friendsManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        friendsManager = FriendsManager.Instance;
    }

    public void ShowContent()
    {
        content.SetActive(true);
    }

    public void ToggleFriendsWindow()
    {
        bool windowIsActive = friendsWindow.activeSelf;
        if (!windowIsActive)
            friendsManager.RefreshLists(isRefreshed => { friendsWindow.SetActive(true); });
        else
            friendsManager.ClearLists(isCleared => { friendsWindow.SetActive(false); });
    }

    public void Refresh()
    {
        RefreshUsername();
        RefreshPearls();
        RefreshBoosterChests();
        //friendsManager.RefreshLists(isRefreshed => { });
    }

    public void RefreshUsername()
    {
        usernameDisplay.text = PersistentData.user.username;
    }

    public void RefreshPearls()
    {
        pearlsDisplay.text = PersistentData.user.pearls.ToString();
    }

    public void RefreshBoosterChests()
    {
        boosterChestsDisplay.text = PersistentData.user.boosterChests.ToString();
    }
}