using UnityEngine;
using Proyecto26;
using TMPro;
using System;

public class FriendRequestItem : MonoBehaviour
{
    Friend friend;
    [SerializeField] TextMeshProUGUI usernameDisplay;

    FriendsManager manager;

    public void Initialise(Friend friend, string friendUsername)
    {
        this.friend = friend;
        usernameDisplay.text = friendUsername;

        manager = FriendsManager.Instance;
    }

    public void AcceptBtn()
    {
        manager.PostNewFriendToDatabase(friend, addedFriend =>
            RemoveFromList(itemRemoved => manager.RefreshLists(isRefreshed => { })));
    }

    public void DeclineBtn()
    {
        RemoveFromList(itemRemoved => { });
    }

    void RemoveFromList(Action<bool> itemRemoved)
    {
        RestClient
            .Delete(DB.DATABASE_USERS_localId_FRIENDREQUESTS_URL(PersistentData.user.localId) + "/" + friend.localId + DB.DotJson())
            .Then(response =>
            {
                Destroy(gameObject);
                itemRemoved(true);
            })
            .Catch(Debug.LogError);
    }
}