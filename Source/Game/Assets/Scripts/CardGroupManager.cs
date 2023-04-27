using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGroupManager : MonoBehaviour
{
    [SerializeField] protected RectTransform cardGroupList;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        RefreshCardGroup();
    }

    public virtual void RefreshCardGroup()
    {
    }

    public virtual void AddCardsToGroup(CardInGroup[] collectionCards)
    {
    }

    public virtual void AddCardToGroup(CardAsset.Data cardData)
    {
    }

    protected virtual void AddSoloCardToGroup(CardAsset.Data cardData, CardItem.Type cardItemInputType)
    {
    }

    public virtual void RemoveCardFromGroup(CardAsset.Data cardData)
    {
    }
}
