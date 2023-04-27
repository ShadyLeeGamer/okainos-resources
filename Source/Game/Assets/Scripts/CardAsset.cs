using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Card Asset", menuName = "[] New Card Asset", order = 1)]
public class CardAsset : ScriptableObject
{
    /// <summary>
    /// The card data body that is sent to the Firebase DB
    /// </summary>
    /// 
    //public List<CardEffectConditionEdit> conditionsEdit;

    [Serializable]
    public class Data
    {
        [ReadOnly] public string name;

        [ReadOnly] public string id;

        public int price;
        public int strength;
        public int draw;

        [Serializable]
        public class EffectParametters
        {
            public SharedData.Effects effect;
            public SharedData.Familly famillyOfEffect;
            public int[] values;
            public Sprite effectIcon;
        }
        public EffectParametters[] effects;
        [Multiline] public string description;
        public SharedData.Familly familly;
        public SharedData.Rarity rarity;
        [HideInInspector] public CardEffects cardEffects;
    }
    public Data data;
    public List<CardEffectEdit> cardEffectsEdit;
/*    public Material basicMaterial;*/
    public Material[] skins;
/*    public GameObject battlePrefab;*/

    [Multiline] public string notes;

    public void Refresh()
    {
        data.name = data.id = name;

        data.cardEffects = new CardEffects();
        for (int i = 0; i < cardEffectsEdit.Count; i++)
        {
            cardEffectsEdit[i].Refresh();
            data.cardEffects.AddToList(cardEffectsEdit[i].CardEffect);
        }
    }

}

[Serializable]
public class CardEffects
{
    //
    // MAYBE INSTEAD STORE A SINGLE CARD EFFECT LIST
    //
    public List<CardEffect.Boost> boost = new List<CardEffect.Boost>();
    public List<CardEffect.Summon> summon = new List<CardEffect.Summon>();
    public List<CardEffect.Modify> modify = new List<CardEffect.Modify>();
    public List<CardEffect.Shield> shield = new List<CardEffect.Shield>();
    public List<CardEffect.Exchange> exchange = new List<CardEffect.Exchange>();
    public List<CardEffect.CountAs> countAs = new List<CardEffect.CountAs>();
    public List<CardEffect.Compare> compare = new List<CardEffect.Compare>();
    public List<CardEffect.Steal> steal = new List<CardEffect.Steal>();
    public List<CardEffect.HeadsOrTails> headsOrTails = new List<CardEffect.HeadsOrTails>();
    public List<CardEffect.Copy> copy = new List<CardEffect.Copy>();
    public List<CardEffect.Reset> reset = new List<CardEffect.Reset>();
    public List<CardEffect.Cancel> cancel = new List<CardEffect.Cancel>();

    public void AddToList(CardEffect cardEffect)
    {
        switch (cardEffect.tag)
        {
            case CardEffect.Tag.Boost:
                boost.Add((CardEffect.Boost)cardEffect);
                break;
            case CardEffect.Tag.Summon:
                summon.Add((CardEffect.Summon)cardEffect);
                break;
            case CardEffect.Tag.Modify:
                modify.Add((CardEffect.Modify)cardEffect);
                break;
            case CardEffect.Tag.Shield:
                shield.Add((CardEffect.Shield)cardEffect);
                break;
            case CardEffect.Tag.CountAs:
                countAs.Add((CardEffect.CountAs)cardEffect);
                break;
            case CardEffect.Tag.Exchange:
                exchange.Add((CardEffect.Exchange)cardEffect);
                break;
            case CardEffect.Tag.Compare:
                compare.Add((CardEffect.Compare)cardEffect);
                break;
            case CardEffect.Tag.Steal:
                steal.Add((CardEffect.Steal)cardEffect);
                break;
            case CardEffect.Tag.HeadsOrTails:
                headsOrTails.Add((CardEffect.HeadsOrTails)cardEffect);
                break;
            case CardEffect.Tag.Copy:
                copy.Add((CardEffect.Copy)cardEffect);
                break;
            case CardEffect.Tag.Reset:
                reset.Add((CardEffect.Reset)cardEffect);
                break;
            case CardEffect.Tag.Cancel:
                cancel.Add((CardEffect.Cancel)cardEffect);
                break;
        }
    }

    public List<CardEffect> GetCardEffects()
    {
        List<CardEffect> cardEffects = new List<CardEffect>();
        cardEffects.AddRange(boost);
        cardEffects.AddRange(summon);
        cardEffects.AddRange(modify);
        cardEffects.AddRange(shield);
        cardEffects.AddRange(exchange);
        cardEffects.AddRange(countAs);
        cardEffects.AddRange(compare);
        cardEffects.AddRange(steal);
        cardEffects.AddRange(headsOrTails);
        cardEffects.AddRange(copy);
        cardEffects.AddRange(reset);
        cardEffects.AddRange(cancel);
        return cardEffects;
    }

    [Serializable]
    public class Conditions
    {
        public List<CardEffect.Condition.OnTurn> onTurn = new List<CardEffect.Condition.OnTurn>();
        public List<CardEffect.Condition.Vanish> vanish = new List<CardEffect.Condition.Vanish>();
        public List<CardEffect.Condition.ForEvery> forEvery = new List<CardEffect.Condition.ForEvery>();
        public List<CardEffect.Condition.AtLeast> atLeast = new List<CardEffect.Condition.AtLeast>();
        public List<CardEffect.Condition.WhileOnBoard> whileOnBoard = new List<CardEffect.Condition.WhileOnBoard>();
        public List<CardEffect.Condition.Boarding> boarding = new List<CardEffect.Condition.Boarding>();

        public void AddToList(CardEffect.Condition cardEffectCondition)
        {
            switch (cardEffectCondition.tag)
            {
                case CardEffect.Condition.Tag.OnTurn:
                    GetList<CardEffect.Condition.OnTurn>()
                        .Add((CardEffect.Condition.OnTurn)cardEffectCondition);
                    break;
                case CardEffect.Condition.Tag.Vanish:
                    GetList<CardEffect.Condition.Vanish>()
                        .Add((CardEffect.Condition.Vanish)cardEffectCondition);
                    break;
                case CardEffect.Condition.Tag.ForEvery:
                    GetList<CardEffect.Condition.ForEvery>()
                        .Add((CardEffect.Condition.ForEvery)cardEffectCondition);
                    break;
                case CardEffect.Condition.Tag.AtLeast:
                    GetList<CardEffect.Condition.AtLeast>()
                        .Add((CardEffect.Condition.AtLeast)cardEffectCondition);
                    break;
                case CardEffect.Condition.Tag.WhileOnBoard:
                    GetList<CardEffect.Condition.WhileOnBoard>()
                        .Add((CardEffect.Condition.WhileOnBoard)cardEffectCondition);
                    break;
                case CardEffect.Condition.Tag.Boarding:
                    GetList<CardEffect.Condition.Boarding>()
                        .Add((CardEffect.Condition.Boarding)cardEffectCondition);
                    break;
            }
        }

        public IList<T> GetList<T>() where T : CardEffect.Condition
        {
            switch (typeof(T).Name)
            {
                case nameof(CardEffect.Condition.OnTurn):
                    return (IList<T>)onTurn;
                case nameof(CardEffect.Condition.Vanish):
                    return (IList<T>)vanish;
                case nameof(CardEffect.Condition.ForEvery):
                    return (IList<T>)forEvery;
                case nameof(CardEffect.Condition.AtLeast):
                    return (IList<T>)atLeast;
                case nameof(CardEffect.Condition.WhileOnBoard):
                    return (IList<T>)whileOnBoard;
                case nameof(CardEffect.Condition.Boarding):
                    return (IList<T>)boarding;
            }
            return null;
        }

        public List<CardEffect.Condition> GetConditions()
        {
            List<CardEffect.Condition> conditions = new List<CardEffect.Condition>();
            conditions.AddRange(onTurn);
            conditions.AddRange(vanish);
            conditions.AddRange(forEvery);
            conditions.AddRange(atLeast);
            conditions.AddRange(whileOnBoard);
            conditions.AddRange(boarding);
            return conditions;
        }
    }
}

public static class SharedData
{
    public enum Effects
    {
        Boost, MultiplyBoost, Initiative, Summon, RestrictiveSummon, DrawForEvery,
        BoostIfAtLeast, SelfBoost, SelfBoostForEvery, CountAs, Shield, ExchangeCard,
        ExchangeStrengthForEveryNewOpponent, RandomStrengthBetween, RandomPriceBetween,
        RandomDrawBetween, CopyStrengthAtTurn, Jackpot, BoostAtTurn
    };
    public enum Familly { Koi, Cuttlefish, Amphibian, Submariner, Pirate, Mermaid };
    public enum Rarity { Rock, Reef, CoralRuby, Gold };

    public static int FamilyMembersLength => Enum.GetValues(typeof(Familly)).Length;
    public static int RarityMembersLength => Enum.GetValues(typeof(Rarity)).Length;
}

[Serializable]
public class PrimaryStats
{
    public enum Stat { Strength, Price, Draw }
    public int strength, price, draw;

    public PrimaryStats()
    {
        strength = price = draw = 0;
    }

    public PrimaryStats(int strength, int price, int draw)
    {
        this.strength = strength;
        this.price = price;
        this.draw = draw;
    }

    [Serializable]
    public struct Delta
    {
        public Stat statToAffect;
        [Tooltip("Operation (+, -, *, /) and value")]
        public string deltaCode; // "+1" "*2" "-5"

        public Delta(Stat statToAffect, string deltaCode)
        {
            this.statToAffect = statToAffect;
            this.deltaCode = deltaCode;
        }

        public static void CalculateDelta(ref int currentValue, string deltaCode)
        {
            char opp = deltaCode[0];
            int deltaValue = int.Parse(deltaCode.Substring(1));
            switch (opp)
            {
                case '+':
                    currentValue += deltaValue;
                    break;
                case '-':
                    currentValue -= deltaValue;
                    break;
                case '*':
                    currentValue *= deltaValue;
                    break;
                case '/':
                    currentValue /= deltaValue;
                    break;
                case '=':
                    currentValue = deltaValue;
                    break;
            }
        }
    }
}