using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardInCollectionItem : CardInGroupItem
{
    [SerializeField] Image cardIllustration;
    [SerializeField] Image cardFamilyIcon;
    [SerializeField] Image cardLayout;

    CardInfoPanel cardInfoPanel;

    [SerializeField] Color unavailableColour;

    GameManager gameManager;

    protected override void Start()
    {
        base.Start();

        gameManager = GameManager.Instance;
    }

    public override void Initialise(CardAsset.Data cardData, Type cardItemInputType)
    {
        base.Initialise(cardData, cardItemInputType);

        if (ItemType.input == Type.Input.CardInCollection)
            cardInfoPanel = CardInfoPanel.Instance;
        ApplyStatus();

        cardIllustration.material = CardResources.BasicMaterial;
        cardFamilyIcon.sprite = CardResources.FamilyIcon;
        CardRarityResources.Layout cardRarityLayoutResources = CardResources.CardRarityResources.CardLayout;
        cardLayout.sprite = cardRarityLayoutResources.Sprite;
        if (cardData.rarity == SharedData.Rarity.Gold)
            cardLayout.material = cardRarityLayoutResources.Material;
        else
            cardLayout.material = null;
    }

    public void ApplyStatus()
    {
        cardIllustration.color = ItemType.status == Type.Status.Available
                               ? Color.white
                               : unavailableColour;
    }

    public override void OnPointerDown(PointerEventData eventData) { }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!eventData.IsPointerMoving())
            switch (ItemType.input)
            {
                case Type.Input.CardInSelection:
                    if (cardDeckManager.CanAddCardToDeck(CardData.id))
                    {
                        cardDeckManager.AddCardToGroup(CardData);
                        cardCollectionManager.RemoveCardFromGroup(CardData);
                    }
                    break;
                case Type.Input.CardInCollection:
                    DisplayInfo();
                    break;
                case Type.Input.CardInHand:
                    gameManager.PlayCard(this);
                    break;
                default:
                    break;
            }
    }
    void DisplayInfo()
    {
        cardInfoPanel.Open(CardData, new Type(Type.Input.CardInInfoPanel, ItemType.status));
    }
}