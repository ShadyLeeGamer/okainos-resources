using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PersistentScreen : MonoBehaviour
{
    public static PersistentScreen Instance { get; private set; }

    AccountGUI accountGUI;
    PearlDealsGUI pearlDealsGUI;
    UpdateAvailableGUI updateAvailableGUI;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        accountGUI = AccountGUI.Instance;
        pearlDealsGUI = PearlDealsGUI.Instance;
        updateAvailableGUI = UpdateAvailableGUI.Instance;
    }
}