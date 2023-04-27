using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;

public static class DB
{
    // PATHS
    public const string DATABASE_URL =
        "DATABASE URL;
    public const string DATABASE_USERS_URL =
        DATABASE_URL + "/users"; // + USERS SUB FOLDER
    public static string DATABASE_USERS_localId_FRIENDREQUESTS_URL(string localId)
    {
        return DATABASE_USERS_URL + "/" + localId + "/friendRequests";
    }
    public const string DATABASE_CARDS_URL =
        DATABASE_URL + "/cards";

    public static string DotJson(bool secureAuth = true)
    {
        return ".json" + (secureAuth ? "?auth=" + PersistentData.idToken : "");
    }

    public static string DotJson(string idToken)
    {
        return ".json?auth=" + idToken;
    }

    public const string WEB_API_KEY =
        "WEB API KEY";

    // ENDPOINT URLS
    //  ALL FOUND IN https://firebase.google.com/docs/projects/api/reference/rest
    public const string SIGN_UP_URL =
        "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + WEB_API_KEY;
    public const string SIGN_IN_URL =
        "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + WEB_API_KEY;
    public const string VERIFY_EMAIL_URL =
        "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + WEB_API_KEY;
    public const string GET_USER_DATA_URL =
        "https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=" + WEB_API_KEY;
    public const string SIGN_IN_OAUTH_URL =
        "https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key=" + WEB_API_KEY;
    public const string CREATE_AUTH_URI_URL =
       "https://identitytoolkit.googleapis.com/v1/accounts:createAuthUri?key=" + WEB_API_KEY;

    public static IEnumerator CheckInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        action(www.error == null);
    }

    public static void PostUser(string email, string username, string localId, string idToken, Action<User> postedUser)
    {
        GetUserByLocalId(localId, user =>
        {
            int pearls = 175;
            int salt = 0;
            int boosterChests = 0;
            List<CardInGroup> cardCollection = new List<CardInGroup>();
            List<CardDeck> cardDecks = new List<CardDeck>();
            List<Friend> friends = new List<Friend>();
            bool isWelcomed = false;
            PearlDealClaimed[] pearlDealsClaimed = new PearlDealClaimed[4];
            bool isAPatron = false;
            bool boughtUsACoffee = false;
            string closedBetaTesterItchIoKeyURL = "";
            int selectedCardDeckNo = 0;
            if (user != null)
            {
                pearls = user.pearls;
                salt = user.salt;
                boosterChests = user.boosterChests;
                cardCollection = user.cardCollection;
                cardDecks = user.cardDecks;
                friends = user.friends;
                isWelcomed = user.isWelcomed;
                pearlDealsClaimed = user.pearlDealsClaimed;
                isAPatron = user.isAPatron;
                boughtUsACoffee = user.boughtUsACoffee;
                closedBetaTesterItchIoKeyURL = user.closedBetaTesterItchIoKeyURL;
                selectedCardDeckNo = user.selectedCardDeckNo;
            }
            RestClient
                .Put(DATABASE_USERS_URL + "/" + localId + ".json?auth=" + idToken,
                     PersistentData.user = new User(localId, email, pearls, salt, boosterChests, username, cardCollection, cardDecks, friends, isWelcomed, pearlDealsClaimed, isAPatron, boughtUsACoffee, closedBetaTesterItchIoKeyURL, selectedCardDeckNo))
                .Then(response =>
                {
                    PersistentData.idToken = idToken;

                    postedUser(PersistentData.user);
                })
                .Catch(Debug.LogWarning);

        }, false);
    }

    public static void GetUserByLocalId(string localId, Action<User> getUser, bool secureAuth = true)
    {
        GetObject<User>(DATABASE_USERS_URL + "/" + localId + DotJson(secureAuth), user =>
        {
            if (user != null)
                getUser(user);
            else
                getUser(null);
        });
    }

    public static void GetObject<T>(string path, Action<T> getObject)
    {
        RestClient
            .Get(path)
            .Then(response =>
            {
                getObject(Serialiser.DeserialiseDataFromJson<T>(response.Text));
            })
            .Catch(error =>
            {
                Debug.LogWarning(path);
                Debug.LogWarning(error);
                getObject(default);
            });
    }

    public static void GetAllObjects<T>(string path, Action<T[]> allObjects)
    {
        RestClient
            .Get(path)
            .Then(response =>
            {
                Dictionary<string, T> objectsDictionary =
                    Serialiser.DeserialiseArrayFromJson<T>(response.Text);
                allObjects(new List<T>(objectsDictionary.Values).ToArray());
            })
            .Catch(error =>
            {
                Debug.LogWarning(path);
                Debug.LogWarning(error);
                allObjects(null);
            });
    }
}

[Serializable]
public class PearlDealClaimed
{
    public bool isClaimed = false;
    public int customClaim = 0;

    public PearlDealClaimed()
    {
        isClaimed = false;
        customClaim = 0;
    }
}

[Serializable]
public class User
{
    public string email;
    public string localId;
    public int pearls;
    public int salt;
    public int boosterChests;
    public string username;
    public List<CardInGroup> cardCollection;
    public List<CardDeck> cardDecks;
    public List<Friend> friends;
    public bool isWelcomed;
    public PearlDealClaimed[] pearlDealsClaimed = new PearlDealClaimed[4];
    public bool isAPatron;
    public bool boughtUsACoffee;
    public string closedBetaTesterItchIoKeyURL;
    public int selectedCardDeckNo;

    public User() { }
    public User(string localId = "", string email = "", int pearls = 0, int salt = 0, int boosterChests = 0, string username = "", List<CardInGroup> cardCollection = null, List<CardDeck> cardDecks = null, List<Friend> friends = null, bool isWelcomed = false, PearlDealClaimed[] pearlDealsClaimed = null, bool isAPatron = false, bool boughtUsACoffee = false, string closedBetaTesterItchIoKeyURL = "", int selectedCardDeckNo = 0)
    {
        this.email = email;
        this.localId = localId;
        this.pearls = pearls;
        this.salt = salt;
        this.boosterChests = boosterChests;
        this.username = username;
        this.cardCollection = cardCollection != null ? cardCollection : new List<CardInGroup>();
        this.cardDecks = cardDecks != null ? cardDecks : new List<CardDeck>();
        this.friends = friends != null ? friends : new List<Friend>();
        this.isWelcomed = isWelcomed;
        this.pearlDealsClaimed = pearlDealsClaimed != null ? pearlDealsClaimed : new PearlDealClaimed[4];
        this.isAPatron = isAPatron;
        this.boughtUsACoffee = boughtUsACoffee;
        this.closedBetaTesterItchIoKeyURL = closedBetaTesterItchIoKeyURL;
        this.selectedCardDeckNo = selectedCardDeckNo;
    }

    public void ModifyCardCollection(CardAsset.Data cardData, bool add, Action<bool> isUpdated)
    {
        if (cardCollection.Count > 0)
        {
            foreach (CardInGroup collectionCard in cardCollection)
                if (collectionCard.id == cardData.id)
                {
                    if (add)
                        collectionCard.quantity++;
                    else if (collectionCard.quantity > 1)
                        collectionCard.quantity--;
                    else
                        cardCollection.Remove(collectionCard);

                    RestClient
                        .Put(DB.DATABASE_USERS_URL + "/" + localId + "/cardCollection" + DB.DotJson(),
                             StringSerializationAPI.Serialize(typeof(CardInGroup[]), cardCollection))
                        .Then(response => { isUpdated(true); });
                    return;
                }
        }

        if (add)
            cardCollection.Add(new CardInGroup(cardData.id, 1));
        else
            Debug.LogWarning("Tried to remove a non-existing card in collection");

        RestClient
            .Put(DB.DATABASE_USERS_URL + "/" + localId + "/cardCollection" + DB.DotJson(),
                 StringSerializationAPI.Serialize(typeof(CardInGroup[]), cardCollection))
            .Then(response => { isUpdated(true); });

    }

    public void ModifyCurrency(int currencyDelta, string currenyName, Action<bool> isUpdated)
    {
        int currency = default;
        switch (currenyName)
        {
            case "salt":
                currency = salt += currencyDelta;
                break;
            case "pearls":
                currency = pearls += currencyDelta;
                break;
        }
        RestClient
            .Put(DB.DATABASE_USERS_URL + "/" + localId + "/" + currenyName + DB.DotJson(),
                 StringSerializationAPI.Serialize(typeof(int), currency))
            .Then(response => { isUpdated(true); });
    }

    public void ModifyBoosterChests(int boosterChestDelta, Action<bool> isUpdated)
    {
        boosterChests += boosterChestDelta;
        RestClient
            .Put(DB.DATABASE_USERS_URL + "/" + localId + "/boosterChests" + DB.DotJson(),
                 StringSerializationAPI.Serialize(typeof(int), boosterChests))
            .Then(response => { isUpdated(true); });
    }

    void UpdateData()
    {
        RestClient
            .Put(DB.DATABASE_USERS_URL + "/" + localId + "/boosterChests" + DB.DotJson(),
                 StringSerializationAPI.Serialize(typeof(int), boosterChests));
    }

    public void UpdateToCloud(Action<bool> isUpdated)
    {
        RestClient
            .Put(DB.DATABASE_USERS_URL + "/" + localId + DB.DotJson(),
                 this)
            .Then(response => isUpdated(true));
    }
}

/*[Serializable]
public class CardGroup
{
    public CardInGroup[] cards;

    public CardGroup() { }

    public CardGroup(string[] ids)
    {
        cards = new CardInGroup[ids.Length];

        for (int i = 0; i < cards.Length; i++)
            cards[i] = new CardInGroup(ids[i], 1);
    }

    [Serializable]
    public class CardInGroup
    {
        public string id;
        public int quantity;

        public CardInGroup(string id, int quantity)
        {
            this.id = id;
            this.quantity = quantity;
        }

        public override string ToString()
        {
            return id + "*" + quantity + ",";
        }
    }
}*/

[Serializable]
public class CardInGroup
{
    public string id;
    public int quantity;

    public CardInGroup(string id, int quantity)
    {
        this.id = id;
        this.quantity = quantity;
    }

    public override string ToString()
    {
        return id + "*" + quantity + ",";
    }
}

[Serializable]
public class CardDeck
{
    public string code;
    public string name;
    public int no;

    public CardDeck(int no)
    {
        this.no = no;
        code = "";
        name = "Deck " + (no + 1);
    }

    public CardDeck(string code, string name, int no)
    {
        this.code = code;
        this.name = name;
        this.no = no;
    }

    public CardInGroup[] DecodeCardDeckCode()
    {
        string[] cardIdsAndQuantities = code.Split(',', '*');
        var qseudoDeckCards = new CardInGroup[cardIdsAndQuantities.Length / 2];
        for (int i = 0; i < qseudoDeckCards.Length; i++)
        {
            int quantity;
            if (int.TryParse(cardIdsAndQuantities[(i * 2) + 1], out quantity))
                if (quantity <= 20)
                    qseudoDeckCards[i] = new CardInGroup(cardIdsAndQuantities[i * 2],
                                                                   quantity);
        }
        return qseudoDeckCards;
    }
}

// RESPONSE PAYLOAD FROM GET USER DATA
//https://firebase.google.com/docs/reference/rest/auth/#section-get-account-info
[Serializable]
public class EmailConfirmationInfo
{
    public UserInfo[] users; // USER ACC ASSOCIATED WITH THE GIVEN FIREBASE ID TOKEN
}

// RESPONSE PAYLOAD FROM GET USER DATA
//https://firebase.google.com/docs/reference/rest/auth/#section-get-account-info
[Serializable]
public class UserInfo
{
    public string email;
    public bool emailVerified; // HAS EMAIL BEEN VERIFIED
}