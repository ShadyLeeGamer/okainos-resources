using UnityEngine;

public class BoosterChestItem : MonoBehaviour
{
    BoosterSystem boosterSystem;

    void Awake()
    {
        boosterSystem = BoosterSystem.Instance;
    }

    public void Select()
    {
        boosterSystem.Refresh();
    }
}