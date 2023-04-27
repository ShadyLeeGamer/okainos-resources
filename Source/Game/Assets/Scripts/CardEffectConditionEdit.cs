using System;

[Serializable]
public class CardEffectConditionEdit
{
    public CardEffect.Condition.Tag conditionTag;

    public CardEffect.Condition.OnTurn onTurn;
    public CardEffect.Condition.Vanish vanish;
    public CardEffect.Condition.ForEvery forEvery;
    public CardEffect.Condition.AtLeast atLeast;
    public CardEffect.Condition.WhileOnBoard whileOnBoard;
    public CardEffect.Condition.Boarding boarding;

    public CardEffect.Condition Condition
    {
        get
        {
            switch (conditionTag)
            {
                case CardEffect.Condition.Tag.OnTurn:
                    return onTurn;
                case CardEffect.Condition.Tag.Vanish:
                    return vanish;
                case CardEffect.Condition.Tag.ForEvery:
                    return forEvery;
                case CardEffect.Condition.Tag.AtLeast:
                    return atLeast;
                case CardEffect.Condition.Tag.WhileOnBoard:
                    return whileOnBoard;
                case CardEffect.Condition.Tag.Boarding:
                    return boarding;
            }
            return null;
        }
    }

    public void RefreshTag()
    {
        Condition.tag = conditionTag;
    }
}

// NOTES:
// .HAVE DIFFERENT ENUMS
// .MAYBE USE EVENTS.. ADDING CARD EFFECT FUNCTIONS TO DIFFERENT EVENTS

