using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardInDeckItem : CardInGroupItem
{
    public const int CARD_QUANTITY_MAX = 3;

    [SerializeField] Image cardDeckIllustrationSubject,
                           cardDeckIllustrationBackground;

    public override void Initialise(CardAsset.Data cardData, Type cardItemInputType)
    {
        base.Initialise(cardData, cardItemInputType);

        CardResources cardResources = CardResourcesManager.GetCardResources(cardData.id);
        cardDeckIllustrationSubject.sprite = cardResources.DeckIllustrationSubject;
        cardDeckIllustrationBackground.sprite = cardResources.DeckIllustrationBackground;
    }

    public override void ChangeCardQuantity(bool increase)
    {
        base.ChangeCardQuantity(increase);
        numberOfCardsDisplay.text += "/" + CARD_QUANTITY_MAX;
    }

    public override void OnPointerDown(PointerEventData eventData) { }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!eventData.IsPointerMoving())
        {
            cardCollectionManager.AddCardToGroup(CardData);
            cardDeckManager.RemoveCardFromDeck(CardData);
        }
    }
    public override void OnPointerEnter(PointerEventData eventData) { }
}