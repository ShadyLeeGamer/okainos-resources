using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[Serializable]
public struct TradeItem
{
    public int value;
    public enum ValueType { Salt, Pearls }
    public ValueType valueType;
}

public class TradingManager : MonoBehaviour
{
    [Serializable]
    public struct RarityTrade
    {
        public string name;
        public Color colour;

        public bool craftable;
        public TradeItem craftCost;
        public bool scrapable;
        public TradeItem scrapValue;
    }

    [SerializeField] TextMeshProUGUI getInBoostersDisplay;
    [Serializable]
    struct TradingOptionItem
    {
        public Button button;
        public TextMeshProUGUI valueDisplay;
        public Image valueIconDisplay;
    }
    [SerializeField] TradingOptionItem craftOptionItem, scrapOptionItem;
    [SerializeField] RarityTrade[] rarityTrades = new RarityTrade[SharedData.RarityMembersLength];

    [SerializeField] TextMeshProUGUI saltDisplay;
    
    CardCollectionSystem cardCollectionSystem;
    CardInfoPanel cardInfoPanel;

    CardAsset.Data currentCardData;
    AccountGUI accountGUI;

    public static TradingManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cardCollectionSystem = CardCollectionManager.Instance as CardCollectionSystem;
        cardInfoPanel = CardInfoPanel.Instance;
        accountGUI = AccountGUI.Instance;

        SetSaltDisplay(PersistentData.user.salt);
    }

    public void CraftCardButton()
    {
        User user = PersistentData.user;
        TradeItem craftCost = rarityTrades[(int)currentCardData.rarity].craftCost;
        if (user.salt >= craftCost.value)
        {
            cardCollectionSystem.AddCardToGroup(currentCardData);
            user.ModifyCardCollection(currentCardData, true, isUpdated =>
            {
                cardInfoPanel.cardDisplay.ChangeCardQuantity(true);
                /*cardInfoPanel.Refresh(cardInfoPanel.cardDisplay.CardData, cardInfoPanel.cardDisplay.ItemType);*/
                cardInfoPanel.cardDisplay.ItemType.status = CardItem.Type.Status.Available;
                Refresh(cardInfoPanel.cardDisplay.CardData, cardInfoPanel.cardDisplay.ItemType);
                cardInfoPanel.cardDisplay.ApplyStatus();
                /*                cardInfoPanel.cardDisplay.ItemType.status = CardItem.Type.Status.Available;
                                cardInfoPanel.cardDisplay.ApplyStatus();*/

                user.ModifyCurrency(-craftCost.value, craftCost.valueType.ToString().ToLower(), isUpdated =>
                {
                    if (craftCost.valueType == TradeItem.ValueType.Salt)
                        SetSaltDisplay(user.salt);
                    else
                        accountGUI.RefreshPearls();

                    cardCollectionSystem.Refresh();
                });
            });
        }
        else
            Debug.Log("not enough salt");
    }

    public void ScrapCardButton()
    {
        User user = PersistentData.user;
        cardCollectionSystem.RemoveCardFromGroup(currentCardData);

        if (cardInfoPanel.cardDisplay.CardQuantity == 1)
        {
            cardInfoPanel.cardDisplay.ItemType.status = CardItem.Type.Status.Unavailable;
            cardInfoPanel.cardDisplay.ApplyStatus();
            cardInfoPanel.cardDisplay.ChangeCardQuantity(false);
        }

        user.ModifyCardCollection(currentCardData, false, isUpdated =>
        {
            if (cardInfoPanel.cardDisplay.CardQuantity > 1)
                cardInfoPanel.cardDisplay.ChangeCardQuantity(false);

            if (cardInfoPanel.cardDisplay.CardQuantity == 0)
                cardInfoPanel.Close();

            TradeItem scrapValue = rarityTrades[(int)currentCardData.rarity].scrapValue;
            user.ModifyCurrency(scrapValue.value, scrapValue.valueType.ToString().ToLower(), isUpdated =>
            {
                if (scrapValue.valueType == TradeItem.ValueType.Salt)
                    SetSaltDisplay(user.salt);
                else
                    accountGUI.RefreshPearls();

                cardCollectionSystem.Refresh();
            });
        });
    }

    void SetSaltDisplay(int salt)
    {
        saltDisplay.text = salt + Emoji.Code("salt");
    }

    public void Refresh(CardAsset.Data cardData, CardItem.Type cardInGroupItemType)
    {
        currentCardData = cardData;

        RarityTrade rarityTrade = rarityTrades[(int)cardData.rarity];
        craftOptionItem.button.gameObject.SetActive(rarityTrade.craftable);
        craftOptionItem.valueDisplay.text = rarityTrade.craftable
                                          ? rarityTrade.craftCost.value + Emoji.Code(rarityTrade.craftCost.valueType.ToString().ToLower())
                                          : "";
        scrapOptionItem.button.gameObject.SetActive(rarityTrade.scrapable &&
                                                    cardInGroupItemType.status == CardItem.Type.Status.Available);
        getInBoostersDisplay.text = rarityTrade.craftable ? "Also found in Boosters!" : "Only found in Boosters!";
        getInBoostersDisplay.color = rarityTrade.colour;
        scrapOptionItem.valueDisplay.text = rarityTrade.scrapable
                                          ? rarityTrade.scrapValue.value + Emoji.Code(rarityTrade.scrapValue.valueType.ToString().ToLower())
                                          : "";
    }
}