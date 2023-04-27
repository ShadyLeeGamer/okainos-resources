using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PearlDealsGUI : MonoBehaviour
{
    [Serializable]
    public struct PearlDeal
    {
        public enum Condition { AddFriend, BuyAPatreonTier, BuyUsACoffee }
        public Condition condition;
        public int claimValue;
        public int compareValue;
        public TextMeshProUGUI conditionDisplay;
        public Button claimButton;
        public bool IsClaimed { get; set; }
        public bool IsClaimable 
        {
            get
            {
                switch (condition)
                {
                    case Condition.AddFriend:
                        return PersistentData.user.friends.Count >= compareValue;
                    case Condition.BuyAPatreonTier:
                        return PersistentData.user.isAPatron;
                    case Condition.BuyUsACoffee:
                        return PersistentData.user.boughtUsACoffee;
                }
                return false;
            }
        }

        public void Refresh()
        {
            claimButton.targetGraphic.enabled = !IsClaimed;
            claimButton.enabled = !IsClaimed;

            if (IsClaimable)
            {
                conditionDisplay.text = IsClaimed ? "CLAIMED" : "CLAIM READY";

                string conditionMet = "";
                switch (condition)
                {
                    case Condition.AddFriend:
                        conditionMet = "\n<size=70%>(Add " + compareValue + (compareValue > 1 ? " friends" : " friend") + ")</size>";
                        break;
                    case Condition.BuyAPatreonTier:
                        conditionMet = "\n<size=70%>(Buy a Patreon tier)</size>"; ;
                        break;
                    case Condition.BuyUsACoffee:
                        conditionMet = "\n<size=70%>(Buy us a coffee!)</size>";
                        break;
                }
                conditionDisplay.text += conditionMet;
            }
        }
    }

    [SerializeField] GameObject content;
    [SerializeField] PearlDeal[] pearlDeals;

    AccountGUI accountGUI;

    public static PearlDealsGUI Instance { get; private set; }

    void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    void Start()
    {
        accountGUI = AccountGUI.Instance;
    }

    public void ShowContent()
    {
        Refresh();
        content.SetActive(true);
    }

    public void Refresh()
    {
        for (int i = 0; i < pearlDeals.Length; i++)
        {
            if (PersistentData.user.pearlDealsClaimed[i].isClaimed)
                pearlDeals[i].IsClaimed = true;
            pearlDeals[i].Refresh();
        }
    }

    public void TryClaimPearlDeal(int index)
    {
        PearlDeal pearlDeal = pearlDeals[index];

        if (PersistentData.user.pearlDealsClaimed[index].isClaimed == false)
            if (pearlDeal.IsClaimable)
                ClaimPearlDeal(index);
            else
                switch (pearlDeal.condition)
                {
                    case PearlDeal.Condition.AddFriend:
                        print("need " + (pearlDeal.compareValue - PersistentData.user.friends.Count) + " more friends!");
                        break;
                    case PearlDeal.Condition.BuyAPatreonTier:
                        Application.OpenURL("https://www.patreon.com/Okainos");
                        break;
                    case PearlDeal.Condition.BuyUsACoffee:
                        Application.OpenURL("https://www.buymeacoffee.com/Okainos");
                        break;
                }
        else
            print("already claimed!");
    }

    void ClaimPearlDeal(int index)
    {
        PearlDeal pearlDeal = pearlDeals[index];
        PearlDealClaimed pearlDealClaimed = PersistentData.user.pearlDealsClaimed[index];
        int claim = pearlDealClaimed.customClaim == 0 ? pearlDeal.claimValue : pearlDealClaimed.customClaim;
        PersistentData.user.ModifyCurrency(pearlDeal.claimValue, "pearls", isUpdated =>
        {
            PersistentData.user.pearlDealsClaimed[index].isClaimed = pearlDeal.IsClaimed = true;
            PersistentData.user.UpdateToCloud(isUpdated => { });
            accountGUI.RefreshPearls();
            pearlDeal.Refresh();
        });
    }
}