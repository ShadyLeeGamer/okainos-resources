using UnityEngine;

public class CardInfoPanel : MonoBehaviour
{
    public CardInCollectionItem cardDisplay;
    TradingManager tradingManager;

    public GameObject Content { get; private set; }

    public static CardInfoPanel Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        Content = transform.GetChild(0).gameObject;
    }

    public void Open(CardAsset.Data cardData, CardItem.Type cardInGroupItemType)
    {
        Content.SetActive(true);

        Refresh(cardData, cardInGroupItemType);
    }

    public void Close()
    {
        Content.SetActive(false);
    }

    public void Refresh(CardAsset.Data cardData, CardItem.Type cardInGroupItemType)
    {
        cardDisplay.Initialise(cardData, cardInGroupItemType);
        foreach (CardInGroup collectionCard in PersistentData.user.cardCollection)
            if (cardData.id == collectionCard.id)
                for (int i = 0; i < collectionCard.quantity - 1; i++)
                    cardDisplay.ChangeCardQuantity(true);

        tradingManager = TradingManager.Instance;
        tradingManager.Refresh(cardData, cardInGroupItemType);
    }
}