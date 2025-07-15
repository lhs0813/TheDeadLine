using Steamworks;
using UnityEngine;

public class AchieveMent_Manager : MonoBehaviour
{
    public static AchieveMent_Manager Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddZombieKill()
    {
        if (!SteamClient.IsValid) return;

        int kills = SteamUserStats.GetStatInt("STAT_ZOMBIE_KILLS");

        kills++;

        SteamUserStats.SetStat("STAT_ZOMBIE_KILLS", kills);
        SteamUserStats.StoreStats();

        Debug.Log("아니 좀비 죽여서 도전과제 했냐고");
    }
}