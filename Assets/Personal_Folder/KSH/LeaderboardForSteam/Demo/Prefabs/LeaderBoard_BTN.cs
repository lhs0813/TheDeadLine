using UnityEngine;

public class LeaderBoard_BTN : MonoBehaviour
{
    public GameObject globalLeaderBoard;
    public GameObject friendsLeaderBoard;
    
    public void ClickGlobal()
    {
        globalLeaderBoard.SetActive(true);
        friendsLeaderBoard.SetActive(false);
    }

    public void ClickFriends()
    {
        globalLeaderBoard.SetActive(false);
        friendsLeaderBoard.SetActive(true);
    }
}
