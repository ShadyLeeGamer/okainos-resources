using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[Serializable]
public struct Game
{
    public int currentBattleTurnNo;
    public Player player, adversary;

    public void Setup()
    {
        player.Setup();
        adversary.Setup();
    }

    public void Update(Game gameDelta)
    {
        player = gameDelta.player;
        adversary = gameDelta.adversary;
    }

    public Player GetPlayerBySubject(CardEffect.Subject subject)
    {
        return subject == CardEffect.Subject.Player ? player : adversary;
    }

    [Serializable]
    public struct Player
    {
        public int coins, draws;
        public TextMeshProUGUI coinCounter, drawCounter;

        public CardSet cardSet;
        public CardInField cardInField;
        public List<CardEffectsManager.CardEffectMove> cardEffectMoves;

        public int boostCounter;

        public void Setup()
        {
            coins = 20;
            draws = 0;
            cardSet.Setup();
            cardInField.SetPlayer(this);

            RefreshDisplays();
        }

        public void RefreshDisplays()
        {
            RefreshCoinsDisplay();
            RefreshDrawsDisplay();
        }

        public void RefreshCoinsDisplay()
        {
            coinCounter.text = coins.ToString();
        }

        public void RefreshDrawsDisplay()
        {
            drawCounter.text = draws.ToString();
        }

        [Serializable]
        public class CardSet
        {
            public readonly int CARDS_IN_HAND_MAX = 6;

            [Serializable]
            public class CardHand
            {
                public CardItem cardItemPrefab;
                public RectTransform holder;
                public RectTransform discardPile;
                public List<CardItem> CardItems { get; set; }

                public void Add(CardAsset.Data cardData)
                {
                    CardItem newCardItem = UnityEngine.Object.Instantiate(cardItemPrefab, holder);
                    newCardItem.Initialise(cardData, new CardItem.Type(CardItem.Type.Input.CardInHand));
                    if (discardPile)
                        discardPile.SetSiblingIndex(holder.childCount - 1);

                    CardItems.Add(newCardItem);
                }

                public CardItem PickRandom()
                {
                    CardItem randomlyPickedCard = CardItems[UnityEngine.Random.Range(0, CardItems.Count)];
                    CardItems.Remove(randomlyPickedCard);
                    UnityEngine.Object.Destroy(randomlyPickedCard.gameObject);
                    return randomlyPickedCard;
                }
            }
            [Serializable]
            public class CardStack
            {
                public Stack<CardAsset.Data> Stack { get; set; }

                public void Add(CardAsset.Data cardData)
                {
                    Stack.Push(cardData);
                }
            }

            public CardHand hand;
            public CardStack deck, graveyard;

            public void Setup()
            {
                hand.CardItems = new List<CardItem>();
                deck.Stack = graveyard.Stack = new Stack<CardAsset.Data>();
                SetupDeck();
                ShuffleDeck();
                DrawCards(CARDS_IN_HAND_MAX);
            }

            void SetupDeck()
            {
                CardInGroup[] cardsInDeckUnshuffled = PersistentData.user.cardDecks[PersistentData.user.selectedCardDeckNo].DecodeCardDeckCode();
                foreach (CardInGroup cardInDeckUnshuffled in cardsInDeckUnshuffled)
                    foreach (CardAsset.Data cardData in PersistentData.cardDatas)
                        if (cardData.id == cardInDeckUnshuffled.id)
                            for (int i = 0; i < cardInDeckUnshuffled.quantity; i++)
                                deck.Add(cardData);
            }

            void ShuffleDeck()
            {
                deck.Stack =
                    new Stack<CardAsset.Data>(deck.Stack.OrderBy(i => Guid.NewGuid()));
            }

            public void DrawCards(int drawAmount)
            {
                for (int i = 0; i < drawAmount; i++)
                {
                    if (deck.Stack.Count == 0)
                        return;
                    CardAsset.Data nextCard = deck.Stack.Pop();
                    hand.Add(nextCard);
                }
            }
        }
    }
}

public class GameManager : MonoBehaviour
{
    const float TRY_INITIATE_BATTLE_REFRESH_RATE = 2f;

    public Game game;

    [SerializeField] float attackAnimSpeedForward, attackAnimSpeedBack;
    [SerializeField] float nextAttackDelay;
    bool cardsBattling;

    [SerializeField] float startAttackDelay;

    [SerializeField] GameObject attackEFXPrefab;

    public GameObject destroyEFXPrefab;
    public CardInWorldItem cardInFieldItemPrefab;
    public float revealToSpawnCardDelay;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        SetupGame();
    }

    void SetupGame()
    {
        game.Setup();

        StartCoroutine(TryInitiateTurns());
    }

    IEnumerator TryInitiateTurns()
    {
        while (!cardsBattling)
        {
            bool adversaryCardIsRevealed = game.adversary.cardInField.CurrentCardInFieldModel;
            if (game.adversary.cardInField.cardInFieldItem.gameObject.activeSelf || adversaryCardIsRevealed)
            {
                if (game.player.cardInField.CurrentCardInFieldModel)
                {
                    if (game.player.cardInField.cardInFieldItem)
                        game.player.cardInField.RevealThenSpawnCard(cardIsRevealed => { });
                    if (game.adversary.cardInField.cardInFieldItem && !adversaryCardIsRevealed)
                        game.adversary.cardInField.RevealThenSpawnCard(cardIsRevealed =>
                        {
                            StartCoroutine(StartInitiateTurns());
                        });
                    else
                        StartCoroutine(StartInitiateTurns());
                    yield break;
                }
            }
            else
            {
                game.adversary.cardInField.EnterCardItemFromHand(game.adversary.cardSet.hand.PickRandom());
                //CardEffectsManager.SetupCardEffectMoves(ref game, game.adversary.cardInField.CardData.cardEffects, out game.adversary.cardEffectMoves, true);
            }
            yield return new WaitForSeconds(TRY_INITIATE_BATTLE_REFRESH_RATE);
        }
    }

    IEnumerator StartInitiateTurns()
    {
        cardsBattling = true;
        yield return new WaitForSeconds(startAttackDelay);
        StartCoroutine(InitiateTurns());
    }

    IEnumerator InitiateTurns()
    {
        while (game.player.cardInField.CurrentCardInFieldModel &&
               game.adversary.cardInField.CurrentCardInFieldModel)
        {
            game.player.cardInField.turnNo++;
            game.adversary.cardInField.turnNo++;

            // EFFECT
            List<CardEffectsManager.CardEffectMove> playerCardEffectMoves = new List<CardEffectsManager.CardEffectMove>();
            playerCardEffectMoves.AddRange(game.player.cardEffectMoves.ToArray());
            for (int i = 0; i < game.player.cardEffectMoves.ToArray().Length; i++)
            {
                 game.player.cardEffectMoves.ToArray()[i].TryPlay(played =>
                 {
                 });
            }
            AttackOtherCardInField(game.player.cardInField, game.adversary.cardInField);
            AttackOtherCardInField(game.adversary.cardInField, game.player.cardInField);
            yield return new WaitForSeconds((1f / attackAnimSpeedForward) +
                                            (1f / attackAnimSpeedBack) + nextAttackDelay);
        }
        cardsBattling = false;
        StartCoroutine(TryInitiateTurns());
    }

    public void AttackOtherCardInField(CardInField cardInField, CardInField cardInAdversaryField)
    {
        Vector3 startDefaultPos = cardInField.DefaultPos;
        Vector3 adversaryDefaultPos = cardInAdversaryField.DefaultPos;
        Vector3 attackPos = (startDefaultPos + adversaryDefaultPos) / 2f;

        int damage = cardInField.primaryStats.strength;
        StartCoroutine(AnimateAttackOtherCardInField(cardInField, attackPos, attackAnimSpeedForward, end =>
        {
            cardInAdversaryField.ModifyPrimaryStat(
                new PrimaryStats.Delta(PrimaryStats.Stat.Strength, "-" + damage));
            Instantiate(attackEFXPrefab, cardInAdversaryField.cardInFieldHolder.position, attackEFXPrefab.transform.rotation);

            StartCoroutine(AnimateAttackOtherCardInField(cardInField, startDefaultPos, attackAnimSpeedBack, end =>
            {
            }));
        }));
    }

    IEnumerator AnimateAttackOtherCardInField(CardInField attackingCardInField, Vector3 targetPos, float attackAnimSpeed, Action<bool> end)
    {
        float percent = 0f;
        Vector3 startPos = attackingCardInField.cardInFieldHolder.position;
        while (percent <= 1f)
        {
            attackingCardInField.cardInFieldHolder.position =
                Vector3.Lerp(startPos, targetPos, percent += attackAnimSpeed * Time.deltaTime);
            yield return null;
        }
        end(true);
    }

    public void PlayCard(CardInCollectionItem cardInHandItem)
    {
        game.player.cardInField.EnterCardItemFromHand(cardInHandItem);
        CardEffectsManager.SetupCardEffectMoves(game.player.cardInField.CardData.cardEffects, out game.player.cardEffectMoves);

        game.player.cardSet.hand.CardItems.Remove(cardInHandItem);
        Destroy(cardInHandItem.gameObject);
    }
}