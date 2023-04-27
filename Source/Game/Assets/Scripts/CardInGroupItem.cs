using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
 public abstract class CardInGroupItem : CardItem, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    public TMP_Text cardNameDisplay,
                    cardPriceDisplay,
                    cardDrawDisplay,
                    cardStrengthDisplay,
                    numberOfCardsDisplay,
                    descriptionDisplay;

    public int CardQuantity { get; protected set; }

    public CardAsset defaultCard;

    protected CardCollectionManager cardCollectionManager;
    protected CardDeckManager cardDeckManager;

    protected virtual void Start()
    {
        cardCollectionManager = CardCollectionManager.Instance;
        cardDeckManager = CardDeckManager.Instance;
    }

    public override void Initialise(CardAsset.Data cardData, Type type)
    {
        base.Initialise(cardData, type);

        cardNameDisplay.text = cardData.name;
        cardPriceDisplay.text = primaryStats.price.ToString();
        cardDrawDisplay.text = primaryStats.draw.ToString();
        cardStrengthDisplay.text = primaryStats.strength.ToString();

        if (numberOfCardsDisplay != null)
        {
            numberOfCardsDisplay.text = (CardQuantity = 0).ToString();
            if (type.status == Type.Status.Available)
                ChangeCardQuantity(true);
        }

        if (descriptionDisplay != null)
            descriptionDisplay.text = cardData.description;
    }

    public virtual void ChangeCardQuantity(bool increase)
    {
        if (numberOfCardsDisplay != null)
            numberOfCardsDisplay.text = (CardQuantity += (increase ? 1 : -1)).ToString();
    }

    public virtual void OnPointerDown(PointerEventData eventData) { }
    public virtual void OnPointerUp(PointerEventData eventData) { }
    public virtual void OnPointerEnter(PointerEventData eventData) { }
    public virtual void OnPointerExit(PointerEventData eventData) { }
}