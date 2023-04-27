using UnityEngine;
using System.Collections.Generic;

public class CardCollectionManager : CardGroupManager
{
    [SerializeField] GameObject noCardsFoundInCollectionDisplay;
    [SerializeField] protected CardInCollectionItem cardItemPrefab;
    protected List<CardInCollectionItem> cardItems = new List<CardInCollectionItem>();

    public static CardCollectionManager Instance { get; protected set; }

    protected override void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        RefreshCardGroup();
    }

    public override void RefreshCardGroup()
    {
        AddCardsToGroup(PersistentData.user.cardCollection.ToArray());
    }

    public override void AddCardsToGroup(CardInGroup[] collectionCards)
    {
        if (noCardsFoundInCollectionDisplay)
            noCardsFoundInCollectionDisplay.SetActive(collectionCards.Length == 0);
        foreach (CardInCollectionItem cardItem in cardItems.ToArray())
        {
            cardItems.Remove(cardItem);
            Destroy(cardItem.gameObject);
        }

        foreach (CardInGroup cardInGroup in collectionCards)
            foreach (CardAsset.Data cardData in PersistentData.cardDatas)
                if (cardInGroup.id == cardData.id)
                {
                    // ADD CARDS WITH 0 QUANTITY
                    int quantity = cardInGroup.quantity == 0 ? 1 : cardInGroup.quantity;
                    for (int no = 0; no < quantity; no++)
                        AddCardToGroup(cardData);
                }
    }

    public override void AddCardToGroup(CardAsset.Data cardData)
    {
        foreach (CardInCollectionItem cardItem in cardItems)
            if (cardItem.CardData.id == cardData.id)
            {
                cardItem.ChangeCardQuantity(true);
                return;
            }

        AddSoloCardToGroup(cardData, new CardItem.Type(CardItem.Type.Input.CardInSelection));
    }

    protected override void AddSoloCardToGroup(CardAsset.Data cardData, CardItem.Type cardItemInputType)
    {
        CardInCollectionItem newCardItem = Instantiate(cardItemPrefab, cardGroupList);
        newCardItem.Initialise(cardData, cardItemInputType);
        cardItems.Add(newCardItem);
    }

    public override void RemoveCardFromGroup(CardAsset.Data cardData)
    {
        foreach (CardInCollectionItem cardItem in cardItems.ToArray())
            if (cardItem.CardData.id == cardData.id)
                if (cardItem.CardQuantity > 1)
                    cardItem.ChangeCardQuantity(false);
                else
                {
                    cardItems.Remove(cardItem);
                    Destroy(cardItem.gameObject);
                }
    }

    public List<CardInCollectionItem> GetCardItems()
    {
        return cardItems;
    }
}