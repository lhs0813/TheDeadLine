using UnityEngine;
using Steamworks;

public class AchieveMent_Manager : MonoBehaviour
{
    public static AchieveMent_Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Steam 초기화 확인
        if (!SteamClient.IsValid)
        {
            Debug.LogError("SteamClient가 초기화되지 않았습니다.");
        }


    }

    public void AddZombieKill()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_ZOMBIE_KILLS", 1);
        SteamUserStats.StoreStats();
    }

    public void GetSkillChip()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_DATACHIP_COUNT", 1);
        SteamUserStats.StoreStats();
    }

    public void AddLegendaryCount()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_LEGENDARY_COUNT", 1);
        SteamUserStats.StoreStats();
    }

    public void FullUpgrade()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_FULL_UPGRADE", 1);
        SteamUserStats.StoreStats();
    }

    public void CampaignClear()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_CAMPAIGN_CLEAR", 1);
        SteamUserStats.StoreStats();
    }

    public void CampaignFastClear()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_CAMPAIGN_FAST_CLEAR", 1);
        SteamUserStats.StoreStats();
    }

    public void EndlessClear5()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_ENDLESS_5_CLEAR", 1);
        SteamUserStats.StoreStats();
    }

    public void EndlessClear10()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_ENDLESS_10_CLEAR", 1);
        SteamUserStats.StoreStats();
    }

    public void EndlessClear20()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_ENDLESS_20_CLEAR", 1);
        SteamUserStats.StoreStats();
    }

    public void ACHIEVEMENT_CREDIT()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            Debug.LogWarning("SteamClient가 유효하지 않거나 로그인되어 있지 않음");
            return;
        }
        SteamUserStats.AddStat("STAT_CREDIT", 1);
        SteamUserStats.StoreStats();
    }



}
