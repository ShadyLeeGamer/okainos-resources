using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Proyecto26;

public static class CardResourcesManager 
{
    public static Dictionary<string, CardResources> cardResourcesDictionary;
    public static void LoadCardResources()
    {
        cardResourcesDictionary = new Dictionary<string, CardResources>();

        Resources.LoadAsync("Cards");

        CardAsset.Data[] cardDatas = PersistentData.cardDatas;

        // Run courou
        StaticCoroutine.StartCoroutine(LoadAll(cardDatas));

        Debug.Log("Card resources reloaded!");
    }

    public static IEnumerator LoadAll(CardAsset.Data[] cardDatas)
    {
        for (int i = 0; i < cardDatas.Length; i++)
        {
            CardAsset.Data cardData = cardDatas[i];
            yield return new WaitForEndOfFrame();
            RescanCardDirectory(cardData);
        }
    }

    static void RescanCardDirectory(CardAsset.Data cardData)
    {
        // Directories
        string cardsDir = "Cards/";
        string cardFamilyDir = cardsDir + "Tribes/" + cardData.familly + "/";
        string cardDir = cardFamilyDir + cardData.name + "/";
        string cardRarityDir = cardsDir + "Rarities/" + cardData.rarity + "/";
        string cardMaterialDir = cardsDir + "Materials/";

        // Load from resources folder
        var basicMaterial =
            (Material)Resources.Load(cardDir + cardData.name + " Basic", typeof(Material));
        var battlePrefab = 
            (GameObject)Resources.Load(cardDir + cardData.name + " Battle", typeof(GameObject));

        var deckIllustrationSubject =
            (Sprite)Resources.Load(cardDir + cardData.name + " Subject", typeof(Sprite));
        var deckIllustrationBackground =
            (Sprite)Resources.Load(cardDir + cardData.name + " Background", typeof(Sprite));
        var familyIcon =
            (Sprite)Resources.Load(cardFamilyDir + cardData.familly + " Icon", typeof(Sprite)
            );

        var rarityLayoutSprite =
            (Sprite)Resources.Load(cardRarityDir + cardData.rarity + " Layout Sprite", typeof(Sprite));
        var rarityLayoutMaterial =
            (Material)Resources.Load(cardRarityDir + cardData.rarity + " Layout Material", typeof(Material));
        var raritySummonEFX =
            (GameObject)Resources.Load(cardRarityDir + cardData.rarity + " Summon EFX", typeof(GameObject));

        var familyMaterial =
            (Material)Resources.Load(cardMaterialDir + "Family", typeof(Material));

        // Contain resources
        var rarityResources = new CardRarityResources(new CardRarityResources.Layout(rarityLayoutSprite, rarityLayoutMaterial),
                                                      new CardRarityResources.EFX(raritySummonEFX));
        var resources = new CardResources(basicMaterial, battlePrefab, familyIcon, null, deckIllustrationSubject, deckIllustrationBackground, rarityResources, familyMaterial);

        // Assign resources with id in a list
        string id = "";
        foreach (CardAsset.Data cardDataFromDB in PersistentData.cardDatas)
            if (cardDataFromDB.name == cardData.name)
                id = cardDataFromDB.id;
        cardResourcesDictionary.Add(id, resources);
    }

    public static CardResources GetCardResources(string cardId)
    {
        return cardResourcesDictionary[cardId];
    }
}

public struct CardResources
{
    public Material BasicMaterial { get; private set; }
    public GameObject BattlePrefab { get; private set; }
    public Sprite FamilyIcon { get; private set; }
    public CardEffectResources[] Effects { get; private set; }

    public Sprite DeckIllustrationSubject { get; private set; }
    public Sprite DeckIllustrationBackground { get; private set; }

    public CardRarityResources CardRarityResources { get; private set; }

    public Material FamilyMaterial { get; private set; }

    public CardResources(Material basicMaterial, GameObject battlePrefab, Sprite familyIcon, CardEffectResources[] effects, Sprite deckIllustrationSubject, Sprite deckIllustrationBackground, CardRarityResources cardRarityResources, Material familyMaterial)
    {
        BasicMaterial = basicMaterial;
        BattlePrefab = battlePrefab;
        FamilyIcon = familyIcon;
        DeckIllustrationSubject = deckIllustrationSubject;
        DeckIllustrationBackground = deckIllustrationBackground;
        Effects = effects;
        CardRarityResources = cardRarityResources;
        FamilyMaterial = new Material(familyMaterial);
        FamilyMaterial.SetTexture("_BaseMap", FamilyIcon.texture);
    }
}

public struct CardRarityResources
{
    public Layout CardLayout { get; private set; }
    public EFX CardEFX { get; private set; }

    public CardRarityResources(Layout cardLayout, EFX cardEFX)
    {
        CardLayout = cardLayout;
        CardEFX = cardEFX;
    }

    public struct Layout
    {
        public Sprite Sprite { get; private set; }
        public Material Material { get; private set; }

        public Layout(Sprite sprite, Material material)
        {
            Sprite = sprite;
            Material = material;
        }
    }
    public struct EFX
    {
        public GameObject Summon { get; private set; }

        public EFX(GameObject summon)
        {
            Summon = summon;
        }
    }
}

public struct CardEffectResources
{
    public Sprite Icon { get; private set; }
    public string Description { get; private set; }

    public CardEffectResources(Sprite icon, string description)
    {
        Icon = icon;
        Description = description;
    }
}