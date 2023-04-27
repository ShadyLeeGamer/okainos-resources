using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BoosterSystem : MonoBehaviour
{
    [Serializable]
    public struct RarityChance
    {
        public string name;
        public Color colour;

        [Serializable]
        public struct EFX
        {
            public GameObject particle;
        }
        public EFX efx;
    }

    [SerializeField] RarityChance[] rarityChances = new RarityChance[SharedData.FamilyMembersLength];

    [SerializeField] List<CardInWorldItem> cardInBoosterItems;
    [SerializeField] CardInWorldItem cardInBoosterItemPrefab;

    [SerializeField] BoosterChest boosterChest;
    [SerializeField] Transform boosterChestList;
    [SerializeField] BoosterChestItem boosterChestItemPrefab;
    List<BoosterChestItem> boosterChestItems = new List<BoosterChestItem>();

    [Serializable]
    public struct Booster
    {
        public string name;
        public Gradient rarityProbabilities;
    }
    [SerializeField] Booster[] boosters;

    AccountGUI accountGUI;

    const int CARD_QUANTITY = 3;

    [SerializeField] int openedCardItemSpacingOffsetX = 0;
    [SerializeField] float openedCardItemPosZ = 1.5f;
    [SerializeField] float nextOpenedCardItemDelay = .5f;

    public static BoosterSystem Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        accountGUI = AccountGUI.Instance;

        for (int i = 0; i < PersistentData.user.boosterChests; i++)
            AddBoosterChestItemsToList();

        Refresh();
    }

    public void Refresh()
    {
        foreach (CardInWorldItem cardInBoosterItem in cardInBoosterItems.ToArray())
        {
            cardInBoosterItems.Remove(cardInBoosterItem);
            Destroy(cardInBoosterItem.gameObject);
        }

        boosterChest.gameObject.SetActive(boosterChestItems.Count != 0);
    }

    void AddBoosterChestItemsToList()
    {
        BoosterChestItem newBoosterChestItem = Instantiate(boosterChestItemPrefab, boosterChestList);
        boosterChestItems.Add(newBoosterChestItem);
    }

    public IEnumerator OpenBooster(int boosterIndex)
    {
        for (int cardNo = CARD_QUANTITY - 1; cardNo >= 0; cardNo--)
        {
            List<CardAsset.Data> randomCardDatas = new List<CardAsset.Data>();
            Color randomColour = boosters[boosterIndex].rarityProbabilities.Evaluate(UnityEngine.Random.value);
            RarityChance rarityChance = default;
            for (int rarityI = 0; rarityI < rarityChances.Length; rarityI++)
                if (randomColour == rarityChances[rarityI].colour)
                {
                    CardAsset.Data[] cardDatas = PersistentData.cardDatas;
                    for (int cardDataI = 0; cardDataI < cardDatas.Length; cardDataI++)
                        if (cardDatas[cardDataI].rarity == (SharedData.Rarity)rarityI)
                        {
                            rarityChance = rarityChances[rarityI];
                            randomCardDatas.Add(cardDatas[cardDataI]);
                        }
                }
            CardAsset.Data openedCardData = randomCardDatas[UnityEngine.Random.Range(0, randomCardDatas.Count)];
            var newCardInBoosterItemPos = new Vector3(cardNo + openedCardItemSpacingOffsetX, 0, openedCardItemPosZ);
            CardInWorldItem newCardInBoosterItem =
                Instantiate(cardInBoosterItemPrefab, newCardInBoosterItemPos, Quaternion.identity);
            cardInBoosterItems.Add(newCardInBoosterItem);
            newCardInBoosterItem
                .Initialise(openedCardData, new CardItem.Type(CardItem.Type.Input.CardInBooster), rarityChance.efx);
            PersistentData.user.ModifyCardCollection(openedCardData, true, isUpdated => { });

            yield return new WaitForSeconds(nextOpenedCardItemDelay);
        }

        PersistentData.user.ModifyBoosterChests(-1, isUpdated =>
        {
            accountGUI.RefreshBoosterChests();
            RemoveBoosterChestItemFromList(boosterChestItems[boosterIndex]);
        });
    }

    void RemoveBoosterChestItemFromList(BoosterChestItem boosterChestItem)
    {
        boosterChestItems.Remove(boosterChestItem);
        Destroy(boosterChestItem.gameObject);
    }
}