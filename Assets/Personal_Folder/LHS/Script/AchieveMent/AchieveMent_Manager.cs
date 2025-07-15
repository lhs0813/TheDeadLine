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
        /*if (!SteamManager.Initialized) return;

        int kills;
        SteamUserStats.GetStat("STAT_ZOMBIE_KILLS", out kills); // ✅ 반드시 int로
        kills++;

        SteamUserStats.SetStat("STAT_ZOMBIE_KILLS", kills);
        SteamUserStats.StoreStats();*/
    }


}
