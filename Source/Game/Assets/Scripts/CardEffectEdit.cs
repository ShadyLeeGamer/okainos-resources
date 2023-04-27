using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardEffectEdit
{
    public CardEffect.Tag effectTag;

    [SerializeField] CardEffect.Boost boost;
    [SerializeField] CardEffect.Summon summon;
    [SerializeField] CardEffect.Modify modify;
    [SerializeField] CardEffect.Shield shield;
    [SerializeField] CardEffect.Exchange exchange;
    [SerializeField] CardEffect.CountAs countAs;
    [SerializeField] CardEffect.Compare compare;
    [SerializeField] CardEffect.Steal steal;
    [SerializeField] CardEffect.HeadsOrTails headsOrTails;
    [SerializeField] CardEffect.Copy copy;
    [SerializeField] CardEffect.Reset reset;
    [SerializeField] CardEffect.Cancel cancel;

    public CardEffect CardEffect
    {
        get
        {
            switch (effectTag)
            {
                case CardEffect.Tag.Boost:
                    return boost;
                case CardEffect.Tag.Summon:
                    return summon;
                case CardEffect.Tag.Modify:
                    return modify;
                case CardEffect.Tag.Shield:
                    return shield;
                case CardEffect.Tag.Exchange:
                    return exchange;
                case CardEffect.Tag.CountAs:
                    return countAs;
                case CardEffect.Tag.Compare:
                    return compare;
                case CardEffect.Tag.Steal:
                    return steal;
                case CardEffect.Tag.HeadsOrTails:
                    return headsOrTails;
                case CardEffect.Tag.Copy:
                    return copy;
                case CardEffect.Tag.Reset:
                    return reset;
                case CardEffect.Tag.Cancel:
                    return cancel;
            }
            return null;
        }
    }

    public void Refresh()
    {
        CardEffect.tag = effectTag;
        CardEffect.conditions = new CardEffects.Conditions();
        foreach (CardEffectConditionEdit conditionEdit in conditionsEdit)
        {
            conditionEdit.RefreshTag();
            CardEffect.conditions.AddToList(conditionEdit.Condition);
        }
    }
    public List<CardEffectConditionEdit> conditionsEdit;
}

[Serializable]
public class CardEffect
{
    public enum Tag
    {
        Boost, // ADD TO STRENGTH COUNTER FOR NEXT CARD
        Summon, // REPLACE WITH ANOTHER
        Modify, // MODIFY PRIMARY VALUE (STRENGTH, PRICE OR DRAW)
        Shield, // PROTECT FROM DAMAGE
        Exchange, // EXCHANGE WITH OPPONENT
        CountAs,
        Compare,
        Steal,
        HeadsOrTails,
        Copy,
        Reset,
        Cancel
    }
    [HideInInspector] public Tag tag;

    [Serializable]
    public abstract class Condition
    {
        public enum Tag
        {
            OnTurn, Vanish, ForEvery, AtLeast, WhileOnBoard, Boarding
        }
        [HideInInspector] public Tag tag;

        public abstract bool IsTrue(CardEffectsManager.CardEffectMove effectMove, Game.Player effectSubject);

        [Serializable]
        public class OnTurn : Condition
        {
            public enum OnTurnValue { Specific }
            public int specificTurn;

            public override bool IsTrue(CardEffectsManager.CardEffectMove effectMove, Game.Player effectSubject)
            {
                Debug.Log(GetType());
                if (effectSubject.cardInField.turnNo != specificTurn)
                {
                    if (effectSubject.cardInField.turnNo > specificTurn)
                    {
                        CardEffectsManager.OnTurnAdversaryEvent -= effectMove.Effect.Apply;
                        effectSubject.cardEffectMoves.Remove(effectMove);
                    }
                    return false;
                }
                Debug.Log(effectSubject.cardInField.turnNo);
                Debug.Log(specificTurn);
                return true;
            }
        }

        [Serializable]
        public class Vanish : Condition
        {
            public override bool IsTrue(CardEffectsManager.CardEffectMove effectMove, Game.Player effectSubject)
            {
                return true;
            }
        }

        [Serializable]
        public class ForEvery : Condition
        {
            public enum ForEveryValue { TribeDeathCounter, HeadsCounter, TailsCounter }
            public ForEveryValue forEveryValue;
            public int no;
            [Tooltip("-1 for none")]
            public int max;
            public override bool IsTrue(CardEffectsManager.CardEffectMove effectMove, Game.Player effectSubject)
            {
                return true;
            }
        }

        [Serializable]
        public class AtLeast : Condition
        {
            public enum AtLeastValue { TribeDeathCounter }
            public AtLeastValue atLeastValue;
            public int no;
            public override bool IsTrue(CardEffectsManager.CardEffectMove effectMove, Game.Player effectSubject)
            {
                return true;
            }
        }

        [Serializable]
        public class WhileOnBoard : Condition
        {
            public override bool IsTrue(CardEffectsManager.CardEffectMove effectMove, Game.Player effectSubject)
            {
                return true;
            }
        }

        [Serializable]
        public class Boarding : Condition
        {
            public override bool IsTrue(CardEffectsManager.CardEffectMove effectMove, Game.Player effectSubject)
            {
                return true;
            }
        }
    }

    [HideInInspector] public CardEffects.Conditions conditions;
    public enum Specific
    {
        Add, // ADD TO COUNTER
        Multiply, // MULTIPLY COUNTER
        NotReceiver, // DON'T RECEIVE FROM GIVERS
        Limited, // LIMIT EFFECT FOR SPECIFIC CARDS
        TribeExclusive, // LIMIT EFFECT FOR SPECIFIC TRIBE
        Card, // EFFECT THAT IS CONCERNING THE CARD
        PrimaryValue, // EFFECT PRIMARY VALUE
        Lottery // RANDOM NUMBER BETWEEN HEADS AND TAILS COUNTER RANGES
    }
    //public Specific[] specifics;
    public enum Subject
    {
        Player,
        OtherPlayer
    }
    public Subject subject;

    public virtual void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
    {
        Debug.Log(GetType());
    }

    [Serializable]
    public class Boost : CardEffect
    {
        public string strengthDeltaCode;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
            PrimaryStats.Delta.CalculateDelta(ref effectSubject.boostCounter, strengthDeltaCode);
            applied(true);
        }
    }

    [Serializable]
    public class Summon : CardEffect
    {
        public enum CardToSummon { Specific, Random }
        public CardToSummon cardToSummon;
        public string specificCardToSummon;
        public PrimaryStats.Delta[] summonedCardPrimaryStatDeltas;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class Modify : CardEffect
    {
        public enum ValueToModify { PrimaryStat, Tribe }
        public ValueToModify valueToModify;
        public PrimaryStats.Delta primaryStatDelta;
        public SharedData.Familly familyDelta;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class Shield : CardEffect
    {
        public int maxBlocks;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class Exchange : CardEffect
    {
        public enum ValueToExchange { Cards, Strength, Price, Draw }
        public ValueToExchange valueToExchange;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class CountAs : CardEffect
    {
        public enum ValueToCountAs { CardQuantity }
        public ValueToCountAs valueToCountAs;
        public int cardQuanity;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class Compare : CardEffect
    {
        public enum ValueToCompare { PrimaryStat }
        public PrimaryStats.Stat primaryStatsToCompare;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class Steal : CardEffect
    {
        public enum ValueToSteal { PrimaryStat }
        public ValueToSteal valueToSteal;
        public PrimaryStats.Delta primaryStatDelta;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class HeadsOrTails : CardEffect
    {
        public enum ValueToBet { PrimaryStat, None, CardEffects }
        public BetSide betForHeads;
        public BetSide betForTails;

        [Serializable]
        public struct BetSide
        {
            public ValueToBet valueToBet;
            public PrimaryStats.Delta primaryStatDelta;
            public bool forEach;
            public CardEffects cardEffectsToApply;
        }

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class Copy : CardEffect
    {
        public enum ValueToCopy { PrimaryStat }

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class Reset : CardEffect
    {
        public enum ValueToReset { TribeDeathCounter }
        public ValueToReset valueToReset;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }

    [Serializable]
    public class Cancel : CardEffect
    {
        public enum ValueToCancel { SpecificCardEffect, AllCardEffects }
        public ValueToCancel valueToCancel;
        public Tag specificCardEffectToCancel;

        public override void Apply(ref CardEffectsManager.CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied)
        {
            base.Apply(ref effectMove, ref effectSubject, applied);
        }
    }
}