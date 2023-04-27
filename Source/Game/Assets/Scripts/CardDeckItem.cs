using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CardDeckItem : MonoBehaviour
{
    CardDeck cardDeck;
    public CardDeck CardDeck { get { return cardDeck; } set { cardDeck = value; } }

    [SerializeField] Image activeHighlight;

    [SerializeField] TextMeshProUGUI cardDeckNameDisplay;
    [SerializeField] GameObject clipboardOptions;
    [SerializeField] GameObject deleteButton;

    CardDeckManager manager;
    MatchmakingScreen matchmakingScreen;

    public void Initialise(int cardDeckNo, string cardDeckCode)
    {
        manager = CardDeckManager.Instance;
        matchmakingScreen = MatchmakingScreen.Instance;

        if (PersistentData.user.cardDecks == null)
            PersistentData.user.cardDecks = new List<CardDeck>();
        bool deckIsNew = cardDeckNo >= PersistentData.user.cardDecks.Count;
        if (deckIsNew)
            PersistentData.user.cardDecks.Add(new CardDeck(cardDeckNo));
        Refresh(new CardDeck(cardDeckCode,
                             deckIsNew ? "Deck " + (cardDeckNo + 1) :
                                         PersistentData.user.cardDecks[cardDeckNo].name,
                             cardDeckNo));
    }

    public void Refresh(CardDeck cardDeck)
    {
        this.cardDeck = cardDeck;
        cardDeckNameDisplay.text = cardDeck.name;
        if (PersistentData.user.selectedCardDeckNo == cardDeck.no)
            manager.SetActiveCardDeckItem(this);
        RefreshDeleteButton();
    }

    public void RefreshDeleteButton()
    {
        deleteButton.SetActive(PersistentData.user.cardDecks.Count > 1);
    }

    public void EditDeckButton()
    {
        manager.RefreshCardGroup(cardDeck);
        matchmakingScreen.ChangeWindow(true);
    }

    public void DeleteDeckButton()
    {
        manager.RemoveCardDeck(cardDeck.no);
        Destroy(gameObject);
    }

    public void SetActiveCardDeckButton()
    {
        if (PersistentData.user.selectedCardDeckNo != cardDeck.no)
        {
            PersistentData.user.selectedCardDeckNo = cardDeck.no;
            PersistentData.user.UpdateToCloud(isUpdated =>
            {
                manager.SetActiveCardDeckItem(this);
            });
        }
    }

    public void SetActiveHighlightEnabled(bool enabled)
    {
        activeHighlight.enabled = enabled;
    }

    public void ClipboardOptionsButton()
    {
        manager.SetSelectedCardDeckItem(this);
    }

    public void SetClipboardOptionsWindowActive(bool active)
    {
        clipboardOptions.SetActive(active);
    }

    public void CopyCardDeckCodeButton()
    {
        GUIUtility.systemCopyBuffer = cardDeck.code;
    }

    public void PasteCardDeckCodeButton()
    {
        cardDeck.code = GUIUtility.systemCopyBuffer;
        manager.SaveDeck(ref cardDeck);
    }
}