using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class CardCollectionSystem : CardCollectionManager
{
    [SerializeField] TMP_Dropdown availabilityDropdown, tribeDropdown, rarityDropdown, priceDropdown, strengthDropdown, drawDropdown;
    [SerializeField] TMP_InputField searchInput;


    List<CardDataInGroup> availableCardDatasInGroup;
    List<CardDataInGroup> unavailableCardDatasInGroup;
    List<CardDataInGroup> allCardDatasInGroup;
    int[] priceOptionValues, strengthOptionValues, drawOptionValues;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        ResetCardDataInGroupLists();

        priceOptionValues = OptionValuesFromOptionText(priceDropdown.options);
        strengthOptionValues = OptionValuesFromOptionText(strengthDropdown.options);
        drawOptionValues = OptionValuesFromOptionText(drawDropdown.options);

        base.Start();
    }

    public void Refresh()
    {
        ResetCardDataInGroupLists();
        RefreshCardGroup();
    }

    void ResetCardDataInGroupLists()
    {
        availableCardDatasInGroup = new List<CardDataInGroup>();
        unavailableCardDatasInGroup = new List<CardDataInGroup>();
        foreach (CardAsset.Data cardData in PersistentData.cardDatas)
        {
            bool cardIsAvailable = false;
            foreach (CardInGroup collectionCard in PersistentData.user.cardCollection)
                if (collectionCard.id == cardData.id)
                {
                    availableCardDatasInGroup.Add(new CardDataInGroup(collectionCard, cardData));
                    cardIsAvailable = true;
                    break;
                }
            if (!cardIsAvailable)
                unavailableCardDatasInGroup.Add(new CardDataInGroup(new CardInGroup(cardData.id, 1), cardData));
        }
        allCardDatasInGroup = new List<CardDataInGroup>();
        allCardDatasInGroup.AddRange(availableCardDatasInGroup);
        if (availabilityDropdown.value == 1)
            allCardDatasInGroup.AddRange(unavailableCardDatasInGroup);
    }

    public void ResortByDropdownFilters()
    {
        ResetCardDataInGroupLists();

        foreach (CardDataInGroup cardDataInGroup in allCardDatasInGroup.ToArray())
        {
            if (tribeDropdown.value != tribeDropdown.options.Count - 1)
                if (cardDataInGroup.CardData.familly != (SharedData.Familly)tribeDropdown.value)
                    allCardDatasInGroup.Remove(cardDataInGroup);
            if (rarityDropdown.value != rarityDropdown.options.Count - 1)
                if (cardDataInGroup.CardData.rarity != (SharedData.Rarity)rarityDropdown.value)
                    allCardDatasInGroup.Remove(cardDataInGroup);

            if (priceDropdown.value != priceDropdown.options.Count - 1)
                if (!ComparePrimaryStatAndDropboxValueCondition(cardDataInGroup.CardData.price, priceOptionValues, priceDropdown))
                    allCardDatasInGroup.Remove(cardDataInGroup);
            if (strengthDropdown.value != strengthDropdown.options.Count - 1)
                if (!ComparePrimaryStatAndDropboxValueCondition(cardDataInGroup.CardData.strength, strengthOptionValues, strengthDropdown))
                    allCardDatasInGroup.Remove(cardDataInGroup);
            if (drawDropdown.value != drawDropdown.options.Count - 1)
                if (!ComparePrimaryStatAndDropboxValueCondition(cardDataInGroup.CardData.draw, drawOptionValues, drawDropdown))
                    allCardDatasInGroup.Remove(cardDataInGroup);
        }

        allCardDatasInGroup = allCardDatasInGroup
            .OrderByDescending(
                card => card.CardData.familly == (SharedData.Familly)tribeDropdown.value)
            .ThenByDescending(
                card => card.CardData.rarity == (SharedData.Rarity)rarityDropdown.value)
            .ThenBy(
                card => ComparePrimaryStatAndDropboxValueCondition(card.CardData.price, priceOptionValues, priceDropdown))
            .ThenBy(
                card => ComparePrimaryStatAndDropboxValueCondition(card.CardData.strength, strengthOptionValues, strengthDropdown))
            .ThenBy(
                card => ComparePrimaryStatAndDropboxValueCondition(card.CardData.draw, drawOptionValues, drawDropdown)).ToList();

        RefreshCardGroup();
    }

    public void ResortBySearchFilter()
    {
        ResetCardDataInGroupLists();

        allCardDatasInGroup = allCardDatasInGroup
            .OrderBy(card => LevenshteinDistance.Compute(card.CardData.name, searchInput.text)).ToList();

        RefreshCardGroup();
    }

    public override void RefreshCardGroup()
    {
        CardInGroup[] cardsInGroup = new CardInGroup[allCardDatasInGroup.Count];
        for (int i = 0; i < cardsInGroup.Length; i++)
            cardsInGroup[i] = allCardDatasInGroup[i].CardInGroup;
        AddCardsToGroup(cardsInGroup);
    }

    protected override void AddSoloCardToGroup(CardAsset.Data cardData, CardItem.Type cardItemInputType)
    {
        CardItem.Type.Status itemStatus = CardItem.Type.Status.Unavailable;
        foreach (CardInGroup collectionCard in PersistentData.user.cardCollection)
            if (collectionCard.id == cardData.id)
            {
                itemStatus = CardItem.Type.Status.Available;
                break;
            }

        base.AddSoloCardToGroup(cardData, new CardItem.Type(CardItem.Type.Input.CardInCollection, itemStatus));
    }

    int[] OptionValuesFromOptionText(List<TMP_Dropdown.OptionData> options)
    {
        int[] optionValues = new int[options.Count];
        for (int i = 0; i < optionValues.Length - 1; i++)
            optionValues[i] = int.Parse(options[i].text.Split("<"[0],"="[0],">"[0])[1]);
        return optionValues;
    }

    bool ComparePrimaryStatAndDropboxValueCondition(int primaryStat, int[] values, TMP_Dropdown dropdown)
    {
        string optionText = dropdown.options[dropdown.value].text;
        int value = values[dropdown.value];
        switch (optionText[0])
        {
            case '<':
                return primaryStat < value;
            case '=':
                return primaryStat == value;
            case '>':
                return primaryStat > value;
        }
        return false;
    }
    /*int ComparePrimaryStatAndDropboxValueCondition(int primaryStat, int[] values, TMP_Dropdown dropdown)
    {
        string optionText = dropdown.options[dropdown.value].text;
        int value = values[dropdown.value];
        if (optionText.Contains("<"))
            return primaryStat - value;
        else if (optionText.Contains("="))
            return Mathf.Abs(value - primaryStat);
        else if (optionText.Contains(">"))
            return value - primaryStat;
        return int.MaxValue;
    }*/

    struct CardDataInGroup
    {
        public CardInGroup CardInGroup { get; private set; }
        public CardAsset.Data CardData { get; private set; }

        public CardDataInGroup(CardInGroup cardInGroup, CardAsset.Data cardData)
        {
            CardInGroup = cardInGroup;
            CardData = cardData;
        }
    }
}

public static class LevenshteinDistance
{
    public static int Compute(string s, string t)
    {
        /*
          The difference between this impl. and the previous is that, rather 
           than creating and retaining a matrix of size s.length()+1 by t.length()+1, 
           we maintain two single-dimensional arrays of length s.length()+1.  The first, d,
           is the 'current working' distance array that maintains the newest distance cost
           counts as we iterate through the characters of String s.  Each time we increment
           the index of String t we are comparing, d is copied to p, the second int[].  Doing so
           allows us to retain the previous cost counts as required by the algorithm (taking 
           the minimum of the cost count to the left, up one, and diagonally up and to the left
           of the current cost count being calculated).  (Note that the arrays aren't really 
           copied anymore, just switched...this is clearly much better than cloning an array 
           or doing a System.arraycopy() each time  through the outer loop.)

           Effectively, the difference between the two implementations is this one does not 
           cause an out of memory condition when calculating the LD over two very large strings.  		
        */

        int n = s.Length; // length of s
        int m = t.Length; // length of t

        if (n == 0)
        {
            return m;
        }
        else if (m == 0)
        {
            return n;
        }

        int[] p = new int[n + 1]; //'previous' cost array, horizontally
        int[] d = new int[n + 1]; // cost array, horizontally
        int[] _d; //placeholder to assist in swapping p and d

        // indexes into strings s and t
        int i; // iterates through s
        int j; // iterates through t

        char t_j; // jth character of t

        int cost; // cost

        for (i = 0; i <= n; i++)
        {
            p[i] = i;
        }

        for (j = 1; j <= m; j++)
        {
            t_j = t[j - 1];
            d[0] = j;

            for (i = 1; i <= n; i++)
            {
                cost = s[i - 1] == t_j ? 0 : 1;
                // minimum of cell to the left+1, to the top+1, diagonally left and up +cost				
                d[i] = Mathf.Min(Mathf.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
            }

            // copy current distance counts to 'previous row' distance counts
            _d = p;
            p = d;
            d = _d;
        }

        // our last action in the above loop was to switch d and p, so p now 
        // actually has the most recent cost counts
        return p[n];
    }
}