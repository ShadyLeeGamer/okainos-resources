using UnityEngine;
using Proyecto26;
using System.Collections.Generic;
using System.Collections;
using System;

public static class PersistentData
{
    public static User user;
    public static string idToken;
    public static CardAsset.Data[] cardDatas;

    public static void SetData(User user, string idToken, CardAsset.Data[] cardDatas)
    {
        PersistentData.user = user;
        if (user.cardCollection == null)
            user.cardCollection = new List<CardInGroup>();
        if (user.cardDecks == null)
            user.cardDecks = new List<CardDeck>();
        if (user.friends == null)
            user.friends = new List<Friend>();
        if (user.pearlDealsClaimed == null)
            user.pearlDealsClaimed = new PearlDealClaimed[4];
        PersistentData.idToken = idToken;
        PersistentData.cardDatas = cardDatas;
    }

    public static void CacheUserData()
    {
        PlayerPrefs.SetString("User_LocalId", user.localId);
        PlayerPrefs.SetString("User_Email", user.email);
        PlayerPrefs.SetInt("User_Pearls", user.pearls);
        PlayerPrefs.SetInt("User_Salt", user.salt);
        PlayerPrefs.SetString("User_Username", user.username);
       // PlayerPrefs.SetString("User_CollectionCards", User.collectionCards.ToString());
        //PlayerPrefs.SetString("User_DeckCards", User.deckCards.ToString());
    }

    public static void LoadCachedUserData()
    {
        user = GetCachedUserData();
    }

    public static User GetCachedUserData()
    {
        return new User(PlayerPrefs.GetString("User_LocalId"),
                        PlayerPrefs.GetString("User_Email"),
                        PlayerPrefs.GetInt("User_Pearls"),
                        PlayerPrefs.GetInt("User_Salt"),
                        PlayerPrefs.GetInt("User_BoosterChests"),
                        PlayerPrefs.GetString("User_Username"),
                        new List<CardInGroup>(),
                        new List<CardDeck>(),
                        new List<Friend>(),
                        PlayerPrefs.GetInt("User_IsWelcomed") == 1);
    }

    public static void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("all data cleared");
    }

    public static IEnumerator CheckForUpdatesOvertime()
    {
        while (true)
        {
            CheckForUpdates();
            Debug.Log("checked for updates. checking again in 10 secs..");
            yield return new WaitForSeconds(10f);
        }
    }

    public static void CheckForUpdates()
    {
        DB.GetObject<string>(DB.DATABASE_URL + "/version.json",
            versionResponse =>
            {
                if (VersionIsNewer(versionResponse))
                    UpdateAvailableGUI.Instance.ShowContent(versionResponse);
            });
    }

    static bool VersionIsNewer(string version2)
    {
        string[] versionNumbers1 = Application.version.Split('.');
        string[] versionNumbers2 = version2.Split('.');
        for (int i = 0; i < versionNumbers2.Length; i++)
            if (i <= versionNumbers1.Length - 1)
            {
                if (int.Parse(versionNumbers1[i]) < int.Parse(versionNumbers2[i]))
                    return true;
            }
            else if (int.Parse(versionNumbers2[i]) != 0)
                return true;
        return false;
    }
}
