using UnityEngine;

public class SocialLinker : MonoBehaviour
{
    [Header("URLs")]
    [Tooltip("초대 링크가 아니라면 서버 주소로 교체하세요")]
    public string discordUrl = "https://discord.gg/3SgmFVPcQ2";
    public string twitterUrl = "https://x.com/RottenApple_170";
    public string steamUrl = "https://store.steampowered.com/app/3827830/The_DeadLine/";

    // Inspector에서 함수 연결용
    public void OpenDiscord()
    {
        Application.OpenURL(discordUrl);
    }

    public void OpenTwitter()
    {
        Application.OpenURL(twitterUrl);
    }
    public void OpenSteam()
    {
        Application.OpenURL(steamUrl);
    }
}
