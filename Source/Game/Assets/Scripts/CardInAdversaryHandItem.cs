using UnityEngine;

public class CardInAdversaryHandItem : MonoBehaviour
{
    public CardAsset.Data CardData { get; set; }
    public CardResources CardResources { get; private set; }

    public void Initialise(CardAsset.Data cardData)
    {
        CardData = cardData;
        CardResources = CardResourcesManager.GetCardResources(cardData.id);
    }
}