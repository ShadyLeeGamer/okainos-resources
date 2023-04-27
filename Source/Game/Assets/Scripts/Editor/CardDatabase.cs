using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

public static class CardDatabase
{
    const string CARD_DATABASE_DIRECTORY_NAME = "Card Database";

    static string CardDatabaseTribesDirectoryPath
    {
        get
        {
            string fullPath =
                (Directory.GetDirectories(
                    Application.dataPath, CARD_DATABASE_DIRECTORY_NAME, SearchOption.AllDirectories)[0]
                + @"\Tribes").Replace('\\', '/');
            return fullPath.Substring(fullPath.IndexOf(@"Assets/"));
        }
    }

    public static void RescanTribeDirectories()
    {
        foreach (string tribeDirectory in Directory.GetDirectories(CardDatabaseTribesDirectoryPath))
            RescanCardDirectory(tribeDirectory);

        Debug.Log("Tribe directories successfully scanned!");
    }

    static CardAssetFileInfo GetCardAssetFileInfo(string cardGUID)
    {
        string cardAssetPath = AssetDatabase.GUIDToAssetPath(cardGUID);
        return new CardAssetFileInfo(GetCardAssetMetaFileInfo(cardAssetPath).CreationTime,
                                     CardAssetPathToCardAsset(cardAssetPath));
    }

    static CardAssetFileInfo GetCardAssetFileInfo(CardAsset cardAsset)
    {
        string cardAssetPath = AssetDatabase.GetAssetPath(cardAsset);
        return new CardAssetFileInfo(GetCardAssetMetaFileInfo(cardAssetPath).CreationTime, cardAsset);
    }

    static FileInfo GetCardAssetMetaFileInfo (string cardAssetPath)
    {
        return new FileInfo(AssetDatabase.GetTextMetaFilePathFromAssetPath(cardAssetPath));
    }

    static CardAsset CardAssetPathToCardAsset(string cardAssetPath)
    {
        return AssetDatabase.LoadMainAssetAtPath(cardAssetPath) as CardAsset;
    }

    static void RescanCardDirectory(string cardDir)
    {
        string[] cardAssetGUIDs = FindCardAssetGUIDs(new[] { cardDir });

        CardAssetFileInfo[] cardAssetFileInfos = new CardAssetFileInfo[cardAssetGUIDs.Length];
        for (int i = 0; i < cardAssetGUIDs.Length; i++)
            cardAssetFileInfos[i] = GetCardAssetFileInfo(cardAssetGUIDs[i]);

        cardAssetFileInfos = cardAssetFileInfos.OrderBy(fileInfo => fileInfo.CreationTime).ToArray();

        for (int creationOrder = 0; creationOrder < cardAssetFileInfos.Length; creationOrder++)
            AssignIdToCardAsset(cardAssetFileInfos[creationOrder]);
    }

    public static void RescanCardAsset(CardAsset cardAsset)
    {
        AssignIdToCardAsset(GetCardAssetFileInfo(cardAsset));
    }

    static void AssignIdToCardAsset(CardAssetFileInfo cardAssetFileInfo)
    {
        cardAssetFileInfo.Card.data.id
            = cardAssetFileInfo.Card.data.familly.ToString().Substring(0, 3).ToLower()
            + cardAssetFileInfo.CreationTime;
    }

    public static CardAsset[] GetFamilyCards(SharedData.Familly family)
    {
        string[] cardGUIDs = FindCardAssetGUIDs(new[] { GetCardFamilyDirectory(family) });
        CardAsset[] cards = new CardAsset[cardGUIDs.Length];
        for (int i = 0; i < cardGUIDs.Length; i++)
            cards[i] = CardAssetPathToCardAsset(AssetDatabase.GUIDToAssetPath(cardGUIDs[i]));
        return cards;
    }

    static string[] FindCardAssetGUIDs(string[] tribeDirs)
    {
        return AssetDatabase.FindAssets("t:cardasset", tribeDirs);
    }

    static string GetCardFamilyDirectory(SharedData.Familly family)
    {
        return CardDatabaseTribesDirectoryPath + "/" + family.ToString();
    }

    struct CardAssetFileInfo
    {
        public string CreationTime { get; set; }
        public CardAsset Card { get; set; }

        public CardAssetFileInfo(DateTime creationTime, CardAsset card)
        {
            DateTime t = creationTime;
            CreationTime = (t.Year + "" +
                            t.Month + "" +
                            t.Day + "" +
                            t.Hour + "" +
                            t.Minute + "" +
                            t.Second + "" +
                            t.Millisecond).ToString();
            Card = card;
        }
    }
}