using UnityEngine;
using Proyecto26;

public static class UserUpdater
{
    // 12/09/21
    // REMOVED TO PREVENT ACCIDENTAL UNWANTED CHANGES
    /*public static void AddAllCardsToCollection()
    {
        DB.GetAllObjects<CardAsset.Data>(DB.DATABASE_CARDS_URL + DB.DotJson(false), cardDatas =>
        {
            DB.GetAllObjects<User>(DB.DATABASE_USERS_URL + DB.DotJson(false), allUsers =>
            {
                CardInGroup[] newCards = new CardInGroup[cardDatas.Length];
                for (int i = 0; i < newCards.Length; i++)
                    newCards[i] = new CardInGroup(cardDatas[i].id, 1);

                foreach (User user in allUsers)
                    RestClient
                        .Put(DB.DATABASE_USERS_URL + "/" + user.localId + "/cardCollection" + DB.DotJson(false),
                             StringSerializationAPI.Serialize(typeof(CardInGroup[]), newCards))
                        .Catch(Debug.LogError);
            });
        });
    }*/

    public static void AddAllCardsToSpecificCollection()
    {
        DB.GetAllObjects<CardAsset.Data>(DB.DATABASE_CARDS_URL + DB.DotJson(false), cardDatas =>
        {
            CardInGroup[] newCards = new CardInGroup[cardDatas.Length];
            for (int i = 0; i < newCards.Length; i++)
                newCards[i] = new CardInGroup(cardDatas[i].id, 1);

            RestClient
                .Put(DB.DATABASE_USERS_URL + "/apy0Nxbu5UYYH3yvK5u6va4Zpbd2/cardCollection" + DB.DotJson(false),
                     StringSerializationAPI.Serialize(typeof(CardInGroup[]), newCards))
                .Catch(Debug.LogError);
        });
    }

    public static void UpdateProperties()
    {
        DB.GetAllObjects<User>(DB.DATABASE_USERS_URL + DB.DotJson(false), allUsers =>
        {
            foreach (User user in allUsers)
                    RestClient
                        .Put(DB.DATABASE_USERS_URL + "/" + user.localId + DB.DotJson(false),
                             user)
                        .Catch(Debug.LogError);
        });
    }

    public static void GETTHEDATA()
    {
        DB.GetObject<string>(DB.DATABASE_URL + "/" + DB.DotJson(false), allUsers =>
        {
            Debug.Log(allUsers);
        });
    }
}


[System.Serializable]
public class data
{
    public string text;
}

