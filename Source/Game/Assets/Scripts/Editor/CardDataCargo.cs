using UnityEngine;
using Proyecto26;
using System.Collections.Generic;

public static class CardDataCargo
{
    static CardAsset[][] cardGroups = new CardAsset[SharedData.FamilyMembersLength][];

    public static void RefreshCardGroups()
    {
        for (int i = 0; i < cardGroups.Length; i++)
            cardGroups[i] = CardDatabase.GetFamilyCards((SharedData.Familly)i);
    }

    public static void SendAllCardDataToCloud()
    {
        for (int groupI = 0; groupI < cardGroups.Length; groupI++)
            for (int cardI = 0; cardI < cardGroups[groupI].Length; cardI++)
                SendCardDataToCloud(cardGroups[groupI][cardI]);
    }

    public static void RetrieveAllCardDataFromCloud()
    {
        for (int groupI = 0; groupI < cardGroups.Length; groupI++)
            for (int cardI = 0; cardI < cardGroups[groupI].Length; cardI++)
                RetrieveCardDataFromCloud(cardGroups[groupI][cardI]);
    }

    public static void SendCardDataToCloud(CardAsset cardAsset)
    {
        //CardDatabase.RescanCardAsset(cardAsset);

        cardAsset.Refresh();

        RestClient
            .Put(DB.DATABASE_CARDS_URL + "/" + cardAsset.data.id + DB.DotJson(false), cardAsset.data)
            .Then(response => Debug.Log(cardAsset.name + " card data successfully updated to DB"))
            .Catch(error => Debug.LogWarning(cardAsset.name + " card update failed: " + error));
    }

    public static void RetrieveCardDataFromCloud(CardAsset cardAsset)
    {
        RestClient
            .Get<CardAsset.Data>(DB.DATABASE_CARDS_URL + "/" + cardAsset.data.id + DB.DotJson(false))
            .Then(cardData =>
            {
                cardAsset.data = cardData;
                Debug.Log(cardAsset.name + " card data successfully fetched from DB");
            })
            .Catch(error => Debug.LogWarning(cardAsset.name + " card fetch failed: " + error));
    }

    public static void RefreshAllReadOnlyCardData()
    {
        Debug.Log(cardGroups.Length);
        for (int groupI = 0; groupI < cardGroups.Length; groupI++)
            for (int cardI = 0; cardI < cardGroups[groupI].Length; cardI++)
                cardGroups[groupI][cardI].Refresh();
    }

    public static CardAsset[] GetCardsByIds(string[] cardIds)
    {
        RefreshCardGroups();

        List<CardAsset> cards = new List<CardAsset>();
        for (int groupI = 0; groupI < cardGroups.Length; groupI++)
            for (int cardI = 0; cardI < cardGroups[groupI].Length; cardI++)
                for (int i = 0; i < cardIds.Length; i++)
                    if (cardIds[i] == cardGroups[groupI][cardI].data.id)
                        cards.Add(cardGroups[groupI][cardI]);
        return cards.ToArray();
    }
}