using Proyecto26;
using System;
using TMPro;
using UnityEngine;

public class FriendsManager : MonoBehaviour
{
    [SerializeField] TMP_InputField searchFriendInputField;

    [SerializeField] RectTransform friendsList;
    [SerializeField] FriendItem friendItemPrefab;
    public FriendItem selectedFriendItem;

    [SerializeField] RectTransform friendRequestsList;
    [SerializeField] FriendRequestItem friendRequestItemPrefab;

    public static FriendsManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void RefreshLists(Action<bool> isRefreshed)
    {
        ClearLists(isCleared =>
        {
            foreach (Friend friend in PersistentData.user.friends)
                DB.GetUserByLocalId(friend.localId, friendUser =>
                    AddFriendItemToList(friend, friendUser.username));

            DB.GetAllObjects<Friend>(
                DB.DATABASE_USERS_localId_FRIENDREQUESTS_URL(PersistentData.user.localId) + DB.DotJson(),
                friendRequests =>
                {
                    if (friendRequests != null)
                        foreach (Friend friendRequest in friendRequests)
                            DB.GetUserByLocalId(friendRequest.localId, friendUser =>
                                    AddFriendRequestItemToList(friendRequest, friendUser.username));

                    isRefreshed(true);
                });
        });
    }

    public void ClearLists(Action<bool> isCleared)
    {
        for (int i = 0; i < friendsList.childCount; i++)
            Destroy(friendsList.GetChild(i).gameObject);
        for (int i = 0; i < friendRequestsList.childCount; i++)
            Destroy(friendRequestsList.GetChild(i).gameObject);
        isCleared(true);
    }

    public void SetSelectedFriendItem(FriendItem friendItem)
    {
        if (selectedFriendItem)
            selectedFriendItem.SetWindowActive(false);
        if (friendItem != selectedFriendItem)
        {
            selectedFriendItem = friendItem;
            selectedFriendItem.SetWindowActive(true);
        }
        else
            selectedFriendItem = null;
    }

    void SendFriendRequest(Friend friend)
    {
        string localId = PersistentData.user.localId;
        RestClient
            .Put(DB.DATABASE_USERS_localId_FRIENDREQUESTS_URL(friend.localId) + "/" + localId + DB.DotJson(), new Friend(localId));
    }

    void AddFriendRequestItemToList(Friend friend, string friendUsername)
    {
        Instantiate(friendRequestItemPrefab, friendRequestsList).Initialise(friend, friendUsername);
    }

    void AddFriendItemToList(Friend friend, string friendUsername)
    {
        Instantiate(friendItemPrefab, friendsList).Initialise(friend, friendUsername);
    }

    public void SendRequestBtn()
    {
        string friendUsername = searchFriendInputField.text;
        TryAddingFriend(friendUsername,
            requestResult => print(requestResult),
            friend => SendFriendRequest(friend));
    }

    void TryAddingFriend(string friendUsername,
                         Action<Friend.RequestResult> requestResult, Action<Friend> addedFriend)
    {
        if (friendUsername == PersistentData.user.username)
        {
            requestResult(Friend.RequestResult.CantAddYourself);
            return;
        }

        DB.GetAllObjects<User>(DB.DATABASE_USERS_URL + DB.DotJson(), users =>
        {
            bool friendExists = false;
            foreach (User user in users)
                if (user.username == friendUsername)
                {
                    friendExists = true;
                    CheckIfFriendIsAlreadyAdded(friendUsername, isFriendAlready =>
                    {
                        if (!isFriendAlready)
                        {
                            Friend friend = new Friend(user.localId);
                            addedFriend(friend);
                            requestResult(Friend.RequestResult.Successful);
                            return;
                        }
                        else
                        {
                            requestResult(Friend.RequestResult.AlreadyFriends);
                            return;
                        }
                    });
                }
            if (!friendExists)
                requestResult(Friend.RequestResult.UserDoesNotExist);
        });
    }

    void CheckIfFriendIsAlreadyAdded(string friendUsername, Action<bool> isFriendAlready)
    {
        DB.GetAllObjects<User>(DB.DATABASE_USERS_URL + DB.DotJson(), users =>
        {
            foreach (User user in users)
                if (user.username == friendUsername)
                {
                    if (PersistentData.user.friends != null)
                    {
                        foreach (Friend friend in PersistentData.user.friends.ToArray())
                            if (friend.localId == user.localId)
                            {
                                isFriendAlready(true);
                                return;
                            }
                        isFriendAlready(false);
                    }
                    else
                        // NO FRIENDS FOUND.. THIS IS YOUR FIRST BESTO FRIENDO!
                        isFriendAlready(false);
                    break;
                }
        });
    }

    public void PostNewFriendToDatabase(Friend friend, Action<bool> addedFriend)
    {
        User user = PersistentData.user;
        // ADD THIS USER AS FRIEND OF FRIEND
        user.friends.Add(friend);
        user.UpdateToCloud(isUpdated =>
        {
            // ADD THIS USER AS FRIEND OF FRIEND
            RestClient
                .Get<User>(DB.DATABASE_USERS_URL + "/" + friend.localId + DB.DotJson())
                .Then(response =>
                {
                    User friendUser = response;
                    friendUser.friends.Add(new Friend(user.localId));

                    friendUser.UpdateToCloud(isUpdated => { });
                    addedFriend(true);
                });
        });
    }
}

[Serializable]
public class Friend
{
    public enum RequestResult { Successful, UserDoesNotExist, AlreadyFriends, CantAddYourself }

    public string localId;

    public Friend(string localId)
    {
        this.localId = localId;
    }
}