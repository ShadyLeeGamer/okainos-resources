using UnityEngine;
using System;
using System.Collections.Generic;

public static class CardEffectsManager
{
    public delegate void ActionRef<EffectMove, EffectSubject>(ref CardEffectMove effectMove, ref Game.Player effectSubject, Action<bool> applied);
    public static event ActionRef<CardEffectMove, Game.Player> OnTurnPlayerEvent,
                                                               OnVanishPlayerEvent;
    public static event ActionRef<CardEffectMove, Game.Player> OnTurnAdversaryEvent,
                                                               OnVanishAdversaryEvent;

    [Serializable]
    public class CardEffectMove
    {
        public CardEffect Effect { get; set; }
        bool ReverseSubject { get; set; }

        GameManager gameManager;

        public CardEffectMove(CardEffect effect, bool reverseSubject)
        {
            gameManager = GameManager.Instance;

            Effect = effect;
            ReverseSubject = reverseSubject;
            if (ReverseSubject)
                Effect.subject = Effect.subject == CardEffect.Subject.Player
                               ? CardEffect.Subject.OtherPlayer
                               : CardEffect.Subject.Player;

            OnTurnPlayerEvent += effect.Apply;
        }

        public void TryPlay(Action<bool> played)
        {
            ref Game.Player effectSubject = ref (Effect.subject == CardEffect.Subject.Player
                                          ? ref GameManager.Instance.game.player
                                          : ref GameManager.Instance.game.adversary);
            foreach (CardEffect.Condition effectCondition in Effect.conditions.GetConditions())
                if (!effectCondition.IsTrue(this, effectSubject))
                    return;
            CardEffectMove thisCardEffectMove = this;

            OnTurnPlayerEvent(ref thisCardEffectMove, ref effectSubject, applied =>
            {
                played(true);
            });
        }
    }

    public static void SetupCardEffectMoves(CardEffects cardEffects, out List<CardEffectMove> cardEffectMoves, bool reverseSubject = false)
    {
        cardEffectMoves = new List<CardEffectMove>();
        List<CardEffect> cardEffectsList = cardEffects.GetCardEffects();
        for (int i = 0; i < cardEffectsList.Count; i++)
            SetupCardEffectMove(cardEffectsList[i], cardEffectMoves, reverseSubject);
    }

    public static void SetupCardEffectMove(CardEffect cardEffect, List<CardEffectMove> cardEffectMoves, bool reverseSubject = false)
    {
        cardEffectMoves.Add(new CardEffectMove(cardEffect, reverseSubject));
    }
}