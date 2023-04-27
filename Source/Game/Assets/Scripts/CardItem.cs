using UnityEngine;

public class CardItem : MonoBehaviour
{
    public CardAsset.Data CardData { get; protected set; }
    public PrimaryStats primaryStats;

    public class Type
    {
        public enum Input { CardInSelection, CardInCollection, CardInDeck, CardInInfoPanel, CardInBooster, CardInHand, CardInField }
        public Input input = Input.CardInSelection;
        public enum Status { Available, Unavailable }
        public Status status = Status.Available;

        public Type(Input input = Input.CardInSelection, Status status = Status.Available)
        {
            this.input = input;
            this.status = status;
        }
    }
    public Type ItemType { get; set; }

   public CardResources CardResources { get; private set; }

    public virtual void Initialise(CardAsset.Data cardData, Type type)
    {
        CardData = cardData;
        primaryStats = new PrimaryStats(cardData.strength, cardData.price, cardData.draw);
        ItemType = type;
        CardResources = CardResourcesManager.GetCardResources(cardData.id);
    }
}