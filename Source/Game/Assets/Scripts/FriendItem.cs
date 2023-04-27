using UnityEngine;
using TMPro;
using Proyecto26;

public class FriendItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI friendUsernameDisplay;
    [SerializeField] GameObject window;

    Friend friend;

    FriendsManager manager;

    public void Initialise(Friend friend, string friendUsername)
    {
        this.friend = friend;
        friendUsernameDisplay.text = friendUsername;

        manager = FriendsManager.Instance;
    }

    public void FriendItemBtn()
    {
        manager.SetSelectedFriendItem(this);
    }

    public void SetWindowActive(bool value)
    {
        window.SetActive(value);
    }

    public void RemoveFriendBtn()
    {
        PersistentData.user.friends.Remove(friend);
        PersistentData.user.UpdateToCloud(isUpdated => { });

        RestClient
            .Get<User>(DB.DATABASE_USERS_URL + "/" + friend.localId + DB.DotJson())
            .Then(response =>
            {
                User friendUser = response;
                foreach (var friend in friendUser.friends.ToArray())
                {
                    if (friend.localId == PersistentData.user.localId)
                    {
                        friendUser.friends.Remove(friend);
                        friendUser.UpdateToCloud(isUpdated => { });

                        Destroy(gameObject);
                    }
                }
            });
    }
}