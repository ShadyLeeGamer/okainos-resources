using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using Proyecto26;

public class CardDeckManager : CardGroupManager
{
    public readonly int CARDS_IN_DECK_QUANTITY_MAX = 20;
    public readonly int CARD_DECK_QUANTITY_MAX = 5;

    [SerializeField] Transform addNewDeckButton;

    [SerializeField] Transform deckSelectionList;
    [SerializeField] CardDeckItem cardDeckItemPrefab;

    [SerializeField] TMP_InputField cardDeckNameInputField;

    [SerializeField] TextMeshProUGUI cardsInDeckQuantityDisplay;
    [SerializeField] Color cardsInDeckQuantityFullColour, cardsInDeckQuantityNotFullColour;
    [SerializeField] float cardsInDeckQuantityDisplayNotFullAnimationSpeed;
    public int CardsInDeckQuantity { get; private set; }

    [SerializeField] RectTransform cardDeckList;

    public int CardDeckQuantity { get; private set; }
    CardDeckItem selectedCardDeckItem;
    CardDeckItem activeCardDeckItem;
    List<CardDeckItem> cardDeckItems = new List<CardDeckItem>();
    [SerializeField] protected CardInDeckItem cardItemPrefab;
    protected List<CardInDeckItem> cardItems = new List<CardInDeckItem>();

    CardCollectionManager cardCollectionManager;
    MatchmakingScreen matchmakingScreen;

    CardDeck currentCardDeck;

    public static CardDeckManager Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        cardCollectionManager = CardCollectionManager.Instance;
        matchmakingScreen = MatchmakingScreen.Instance;

        if (PersistentData.user.cardDecks.Count > 0)
            foreach (CardDeck cardDeck in PersistentData.user.cardDecks)
                AddNewDeckButton(cardDeck.code);
        else
            AddNewDeckButton("");
    }

    public void RefreshCardGroup(CardDeck CardDeck)
    {
        foreach (CardInDeckItem cardItem in cardItems.ToArray())
        {
            int quantity = cardItem.CardQuantity;
            for (int cardNo = 0; cardNo < quantity; cardNo++)
            {
                cardCollectionManager.AddCardToGroup(cardItem.CardData);
                RemoveCardFromDeck(cardItem.CardData);
            }
        }

        currentCardDeck = CardDeck;
        cardDeckNameInputField.text = CardDeck.name;

        if (CardDeck.code == null || CardDeck.code == "")
            return;

        ValidateCardDeck(CardDeck.DecodeCardDeckCode(), response =>
        {
            foreach (string evaluationLog in response.EvaluationLogs)
                Debug.Log(evaluationLog);

            for (int cardI = 0; cardI < response.DeckCards.Length; cardI++)
                for (int cardNo = 0; cardNo < response.DeckCards[cardI].quantity; cardNo++)
                {
                    AddCardToGroup(response.CardData[cardI]);
                    cardCollectionManager.RemoveCardFromGroup(response.CardData[cardI]);
                }
        });
    }

    struct CardDeckCodeValidationResponse
    {
        public CardInGroup[] DeckCards { get; private set; }
        public CardAsset.Data[] CardData { get; private set; }
        public string[] EvaluationLogs { get; private set; }

        public CardDeckCodeValidationResponse(CardInGroup[] deckCards, CardAsset.Data[] cardData, string[] evaluationLogs)
        {
            DeckCards = deckCards;
            CardData = cardData;
            EvaluationLogs = evaluationLogs;
        }
    }

    void ValidateCardDeck(CardInGroup[] pseudoDeckCards, Action<CardDeckCodeValidationResponse> response)
    {
        List<CardInGroup> validDeckCards = new List<CardInGroup>();
        List<CardAsset.Data> validDeckCardDatas = new List<CardAsset.Data>();
        List<string> evaluationLog = new List<string>();

        int totalDeckCardsNo = 0;
        // Validation check for each card
        foreach (CardInGroup pseudoDeckCard in pseudoDeckCards)
        {
            // Ignore errors
            if (pseudoDeckCard == null ||
                pseudoDeckCard.quantity == 0)
                continue;

            // Compare player's card collection against chosen deck cards
            foreach (CardInGroup collectionCard in PersistentData.user.cardCollection)
                if (pseudoDeckCard.id == collectionCard.id)
                    foreach (CardAsset.Data cardData in PersistentData.cardDatas)
                        if (pseudoDeckCard.id == cardData.id) // Check if the deck card exists in the player's collection
                        {
                            bool quantityIsEnough = true;
                            // Check if player has enough of the deck card
                            foreach (CardInCollectionItem cardinCollecitonItem in cardCollectionManager.GetCardItems())
                                if (cardinCollecitonItem.CardData.id == pseudoDeckCard.id)
                                    if (cardinCollecitonItem.CardQuantity < pseudoDeckCard.quantity)
                                    {
                                        int quantityDiff = pseudoDeckCard.quantity - cardinCollecitonItem.CardQuantity;
                                        pseudoDeckCard.quantity = pseudoDeckCard.quantity - quantityDiff;
                                        evaluationLog.Add("You don't have enough " + pseudoDeckCard.id + " cards! You're missing " + quantityDiff + ".");
                                        quantityIsEnough = false;
                                        break;
                                    }
                            // If player has enough of deck card, then check if there is too much of the same card
                            if (quantityIsEnough)
                            {
                                totalDeckCardsNo += pseudoDeckCard.quantity;
                                if (totalDeckCardsNo > CARDS_IN_DECK_QUANTITY_MAX) // 20
                                {
                                    // Failed validation
                                    validDeckCards = null;
                                    validDeckCardDatas = null;
                                    evaluationLog.Add("Total number of cards exceeds the max allowed of " + CARDS_IN_DECK_QUANTITY_MAX + ".");
                                    response(new CardDeckCodeValidationResponse(validDeckCards.ToArray(),
                                                                                validDeckCardDatas.ToArray(),
                                                                                evaluationLog.ToArray()));
                                    return;
                                }

                                // Add valid card
                                validDeckCards.Add(pseudoDeckCard);
                                validDeckCardDatas.Add(cardData);
                            }
                            break;
                        }

            // Check if card is missing from their collection or does not exist in the database
            if (!validDeckCards.Contains(pseudoDeckCard))
            {
                bool cardDoesNotExist = true;
                // Card is missing
                foreach (CardAsset.Data cardData in PersistentData.cardDatas)
                    if (pseudoDeckCard.id == cardData.id)
                    {
                        evaluationLog.Add("You don't have a " + pseudoDeckCard.id + " card in your collection!");
                        cardDoesNotExist = false;
                        break;
                    }
                // Card does not exist
                if (cardDoesNotExist)
                    evaluationLog.Add("Card \"" + pseudoDeckCard.id + "\" does not exist.");
            }
        }
        response(new CardDeckCodeValidationResponse(validDeckCards.ToArray(),
                                                    validDeckCardDatas.ToArray(),
                                                    evaluationLog.ToArray()));
    }

    /// <summary>
    /// Convert card deck code to list of cards through validation
    /// </summary>
    /// <returns>A list of deck cards</returns>
    public string GetCardDeckCode()
    {
        string cardDeckCode = "";
        if (cardItems.Count > 0)
        {
            foreach (CardInDeckItem cardItem in cardItems)
                cardDeckCode += new CardInGroup(cardItem.CardData.id, cardItem.CardQuantity).ToString(); // Returns id and quantity seperated by "*" and ","
            cardDeckCode.Remove(cardDeckCode.Length - 1, 1);
        }
        return cardDeckCode;
    }

    public override void AddCardToGroup(CardAsset.Data cardData)
    {
        if (CardsInDeckQuantity + 1 > CARDS_IN_DECK_QUANTITY_MAX)
            return;
        ChangeCardDeckQuanityDisplay(true);

        foreach (CardInDeckItem cardItem in cardItems)
            if (cardItem.CardData.id == cardData.id)
            {
                if (cardItem.CardQuantity < CardInDeckItem.CARD_QUANTITY_MAX)
                    cardItem.ChangeCardQuantity(true);
                return;
            }

        AddSoloCardToGroup(cardData, new CardItem.Type(CardItem.Type.Input.CardInDeck));
    }

    protected override void AddSoloCardToGroup(CardAsset.Data cardData, CardItem.Type cardItemInputType)
    {
        CardInDeckItem newCardItem = Instantiate(cardItemPrefab, cardDeckList);
        newCardItem.Initialise(cardData, cardItemInputType);
        cardItems.Add(newCardItem);
    }

    public void RemoveCardFromDeck(CardAsset.Data cardData)
    {
        ChangeCardDeckQuanityDisplay(false);

        foreach (CardInDeckItem cardItem in cardItems.ToArray())
            if (cardItem.CardData.id == cardData.id)
                if (cardItem.CardQuantity > 1)
                    cardItem.ChangeCardQuantity(false);
                else
                {
                    cardItems.Remove(cardItem);
                    Destroy(cardItem.gameObject);
                }
    }

    public void RemoveCardDeck(int cardDeckNo)
    {
        CardDeckQuantity--;

        List<CardDeck> deckCards = PersistentData.user.cardDecks;
        cardDeckItems.RemoveAt(cardDeckNo);
        PersistentData.user.cardDecks.RemoveAt(cardDeckNo);
        for (int i = 0; i < PersistentData.user.cardDecks.Count; i++)
            PersistentData.user.cardDecks[i].no = cardDeckItems[i].CardDeck.no = i;
        RestClient.Put(DB.DATABASE_USERS_URL + "/" + PersistentData.user.localId + "/cardDecks" + DB.DotJson(),
                       StringSerializationAPI.Serialize(typeof(CardDeck[]), deckCards));
        if (deckCards.Count > 0)
            SetActiveCardDeckItem(cardDeckItems[0]);
        else
        {
            PersistentData.user.selectedCardDeckNo = cardDeckNo;
            PersistentData.user.UpdateToCloud(isUpdated => { });
        }

        cardDeckItems[0].RefreshDeleteButton();
    }

    public void AddNewDeckButton(string cardDeckCode)
    {
        if (CardDeckQuantity + 1 > CARD_DECK_QUANTITY_MAX)
            return;
        CardDeckQuantity++;

        CardDeckItem newCardDeckItem = Instantiate(cardDeckItemPrefab, deckSelectionList);
        newCardDeckItem.Initialise(cardDeckItems.Count, cardDeckCode);
        cardDeckItems.Add(newCardDeckItem);
        addNewDeckButton.SetSiblingIndex(deckSelectionList.childCount - 1);

        cardDeckItems[0].RefreshDeleteButton();
    }

    public void SaveDeckButton()
    {
        if (CardsInDeckQuantity == CARDS_IN_DECK_QUANTITY_MAX)
            SaveDeck(ref currentCardDeck);
        else if (!cardsInDeckQuantityDisplayIsBeingAnimated)
            AnimateCardsInDeckQuantityDisplayNotFull();
    }

    public void SaveDeck(ref CardDeck cardDeck)
    {
        List<CardDeck> deckCards = PersistentData.user.cardDecks;
        deckCards[cardDeck.no].code = GetCardDeckCode();
        deckCards[cardDeck.no].name = cardDeckNameInputField.text;
        cardDeckItems[cardDeck.no].Refresh(cardDeck = deckCards[cardDeck.no]);
        RestClient
            .Put(DB.DATABASE_USERS_URL + "/" + PersistentData.user.localId + "/cardDecks" + DB.DotJson(),
                 StringSerializationAPI.Serialize(typeof(CardDeck[]), deckCards))
            .Then(response => matchmakingScreen.ChangeWindow(false));
    }

    public void SetSelectedCardDeckItem(CardDeckItem cardDeckItem)
    {
        if (selectedCardDeckItem)
            selectedCardDeckItem.SetClipboardOptionsWindowActive(false);
        if (cardDeckItem != selectedCardDeckItem)
        {
            selectedCardDeckItem = cardDeckItem;
            selectedCardDeckItem.SetClipboardOptionsWindowActive(true);
        }
        else
            selectedCardDeckItem = null;
    }

    public void SetActiveCardDeckItem(CardDeckItem cardDeckItem)
    {
        if (cardDeckItem != activeCardDeckItem)
        {
            if (activeCardDeckItem)
                activeCardDeckItem.SetActiveHighlightEnabled(false);
            activeCardDeckItem = cardDeckItem;
            activeCardDeckItem.SetActiveHighlightEnabled(true);
        }
        else
            activeCardDeckItem = null;
    }

    void ChangeCardDeckQuanityDisplay(bool inc)
    {
        CardsInDeckQuantity += inc ? 1 : -1;
        cardsInDeckQuantityDisplay.text = CardsInDeckQuantity + "/" + CARDS_IN_DECK_QUANTITY_MAX;
        cardsInDeckQuantityDisplay.color = CardsInDeckQuantity == CARDS_IN_DECK_QUANTITY_MAX
                                         ? cardsInDeckQuantityFullColour
                                         : Color.white;
    }

    void AnimateCardsInDeckQuantityDisplayNotFull()
    {
        cardsInDeckQuantityDisplayIsBeingAnimated = true;
        StartCoroutine(AnimateCardsInDeckQuantityDisplayNotFull(cardsInDeckQuantityNotFullColour, end =>
        {
            StartCoroutine(AnimateCardsInDeckQuantityDisplayNotFull(Color.white, end =>
            {
                cardsInDeckQuantityDisplayIsBeingAnimated = false;
            }));
        }));
    }

    bool cardsInDeckQuantityDisplayIsBeingAnimated;
    IEnumerator AnimateCardsInDeckQuantityDisplayNotFull (Color finalColour, Action<bool> end)
    {
        float percent = 0f;
        Color startColour = cardsInDeckQuantityDisplay.color;
        while (percent <= 1f)
        {
            cardsInDeckQuantityDisplay.color =
                Color.Lerp(startColour, finalColour, percent += cardsInDeckQuantityDisplayNotFullAnimationSpeed * Time.deltaTime);;
            yield return null;
        }
        end(true);
    }

    public List<CardInDeckItem> GetCardItems()
    {
        return cardItems;
    }

    public bool CanAddCardToDeck(string cardId)
    {
        if (CardsInDeckQuantity + 1 > CARDS_IN_DECK_QUANTITY_MAX)
            return false;
        foreach (CardInDeckItem cardInDeckItem in cardItems)
            if (cardInDeckItem.CardData.id == cardId)
                if (cardInDeckItem.CardQuantity == CardInDeckItem.CARD_QUANTITY_MAX)
                    return false;
        return true;
    }
}