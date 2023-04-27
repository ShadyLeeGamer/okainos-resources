using UnityEngine;
using TMPro;

public class PersistentObject : MonoBehaviour
{
    public static PersistentObject Instance { get; private set; }

    [SerializeField] TextMeshProUGUI escapingIndicator;
    [SerializeField] TextMeshProUGUI gameVersion;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        gameVersion.text = "Okainos v" + Application.version;
        StartCoroutine(PersistentData.CheckForUpdatesOvertime());
    }

    float escapeThresholdTime;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
            PersistentData.ClearAllData();

        if (Input.GetKey(KeyCode.Escape))
        {
            if (escapeThresholdTime == 0)
            {
                escapeThresholdTime = Time.time + 2;
                escapingIndicator.text = "Escaping...";
            }
            if (Time.time > escapeThresholdTime)
                Application.Quit();
        }
        else if (escapeThresholdTime != 0)
        {
            escapeThresholdTime = 0;
            escapingIndicator.text = "";
        }
    }
}