using UnityEngine;
using System;

public class ShopManager : MonoBehaviour
{
    bool processingBuy;
    [Serializable]
    public struct BoosterItem
    {
        public TradeItem tradeItem;
        public int quantity;
    }
    [SerializeField] BoosterItem[] boosterItems;

    AccountGUI accountGUI;

    void Start()
    {
        accountGUI = AccountGUI.Instance;        
    }

    public void BuyBoosterItemButton(int boosterItemIndex)
    {
        if (!processingBuy)
        {
            processingBuy = true;
            BoosterItem boosterItem = boosterItems[boosterItemIndex];
            if (PersistentData.user.pearls >= boosterItem.tradeItem.value)
                PersistentData.user.ModifyBoosterChests(boosterItem.quantity, isUpdated =>
                    PersistentData.user.ModifyCurrency(-boosterItem.tradeItem.value, "pearls", isUpdated =>
                    {
                        accountGUI.RefreshBoosterChests();
                        accountGUI.RefreshPearls();

                        processingBuy = false;
                    }));
            else
                Debug.Log("not enough pearls");
        }
    }
}