using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;

[Serializable]
public class CardInField : CardInGroupItem
{
    public int turnNo;
    public Game.Player Player { get { return player; } set { player = value; } }
    Game.Player player;

    public Transform cardInFieldHolder;
    public GameObject CurrentCardInFieldModel { get; private set; }
    public CardInWorldItem cardInFieldItem;
    public Vector3 DefaultPos { get; private set; }

    CardRarityResources.EFX cardRarityEFXResources;
    GameObject destroyEFX;
    CardInWorldItem cardInFieldItemPrefab;
    float revealToSpawnCardDelay;

    GameManager gameManager;

    protected override void Start()
    {
        base.Start();
        gameManager = GameManager.Instance;
        destroyEFX = gameManager.destroyEFXPrefab;
        cardInFieldItemPrefab = gameManager.cardInFieldItemPrefab;
        revealToSpawnCardDelay = gameManager.revealToSpawnCardDelay;
    }

    public void SetPlayer(Game.Player player)
    {
        this.player = player;
    }

    public override void Initialise(CardAsset.Data cardData, Type type)
    {
        base.Initialise(cardData, type);
        DefaultPos = cardInFieldHolder.position;
        cardRarityEFXResources = CardResources.CardRarityResources.CardEFX;
    }

    public void EnterCardItemFromHand(CardItem cardItem)
    {
        CardData = cardItem.CardData;
        ItemType = cardItem.ItemType;
        turnNo = 0;

        if (CurrentCardInFieldModel)
            Destroy(CurrentCardInFieldModel);

        if (cardInFieldItem)
            cardInFieldItem.gameObject.SetActive(true);
        else
            SpawnCard();
    }

    public void RevealThenSpawnCard(Action<bool> cardIsRevealed)
    {
        cardInFieldItem.Initialise(CardData, ItemType);
        cardInFieldItem.Reveal(end =>
        {
            StartCoroutine(SpawnCardFromItem(cardIsSpawned =>
            {
                cardIsRevealed(true);
            }));
        });
    }

    IEnumerator SpawnCardFromItem(Action<bool> cardIsSpawned)
    {
        yield return new WaitForSeconds(revealToSpawnCardDelay);
        cardInFieldItem.gameObject.SetActive(false);
        SpawnCard();
        cardIsSpawned(true);
    }

    void SpawnCard()
    {
        Initialise(CardData, ItemType);

        CurrentCardInFieldModel = Instantiate(CardResources.BattlePrefab, cardInFieldHolder);
        GameObject summonEFX = cardRarityEFXResources.Summon;
        if (summonEFX)
            Instantiate(summonEFX, CurrentCardInFieldModel.transform.position, summonEFX.transform.rotation);
    }

    public void RefreshPrimaryStatDisplays()
    {
        RefreshPriceDisplay();
        RefreshStrengthDisplay();
        RefreshDrawDisplay();
    }
    public void RefreshPriceDisplay()
    {
        cardPriceDisplay.text = primaryStats.price.ToString();
    }
    public void RefreshStrengthDisplay()
    {
        cardStrengthDisplay.text = primaryStats.strength.ToString();
    }
    public void RefreshDrawDisplay()
    {
        cardDrawDisplay.text = primaryStats.draw.ToString();
    }

    void ResetCard()
    {
        primaryStats = new PrimaryStats();
        RefreshPrimaryStatDisplays();
        cardNameDisplay.text = descriptionDisplay.text = "";

        if (cardInFieldItem)
            ResetCardItem();
    }

    void ResetCardItem()
    {
        cardInFieldItem.transform.rotation = Quaternion.Euler(0, 180, 0);
    }



    public void ModifyPrimaryStat(PrimaryStats.Delta primaryStatDelta)
    {
        switch (primaryStatDelta.statToAffect)
        {
            case PrimaryStats.Stat.Strength:
                PrimaryStats.Delta.CalculateDelta(ref primaryStats.strength, primaryStatDelta.deltaCode);
                RefreshStrengthDisplay();
                if (primaryStats.strength <= 0)
                    StartCoroutine(Destroy());
                break;
            case PrimaryStats.Stat.Price:
                PrimaryStats.Delta.CalculateDelta(ref primaryStats.price, primaryStatDelta.deltaCode);
                RefreshPriceDisplay();
                break;
            case PrimaryStats.Stat.Draw:
                PrimaryStats.Delta.CalculateDelta(ref primaryStats.draw, primaryStatDelta.deltaCode);
                RefreshDrawDisplay();
                break;
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(.3f);
        player.coins -= primaryStats.price;
        player.draws += primaryStats.draw;
        player.RefreshDisplays();

        int playerCardsInHandCount = player.cardSet.hand.CardItems.Count;
        if (playerCardsInHandCount < player.cardSet.CARDS_IN_HAND_MAX)
        {
            int drawAmountToMax = player.cardSet.CARDS_IN_HAND_MAX - playerCardsInHandCount;
            if (drawAmountToMax > player.draws)
                drawAmountToMax = player.draws;
            int playerCardsLeftInDeck = player.cardSet.deck.Stack.Count;
            if (drawAmountToMax > playerCardsLeftInDeck)
                drawAmountToMax = playerCardsLeftInDeck;

            player.cardSet.DrawCards(drawAmountToMax);
            player.draws -= drawAmountToMax;
        }

        Instantiate(destroyEFX, cardInFieldHolder.position, destroyEFX.transform.rotation);
        Destroy(CurrentCardInFieldModel);
        player.cardSet.graveyard.Add(CardData);
        ResetCard();
    }

    public override void OnPointerDown(PointerEventData eventData) { }
    public override void OnPointerUp(PointerEventData eventData) { }
    public override void OnPointerEnter(PointerEventData eventData) { }
}