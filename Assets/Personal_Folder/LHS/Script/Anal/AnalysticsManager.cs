using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }
    private bool _isInitialized = false;
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

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        _isInitialized = true;
    }

    public void log_storymode_round_progress(int round) // 매번 라운드를 지나 이동할 때, 로그를 남기면 몇스테이지까지 보통 진행하는지 확인해서, 시간과 진행도를 모두 확인 가능
    {
        if (!_isInitialized)
            return;

        CustomEvent log_storymode_round_progress = new CustomEvent("log_storymode_round_progress")
        {
            {"Round", round}
        };

        AnalyticsService.Instance.RecordEvent(log_storymode_round_progress);
        AnalyticsService.Instance.Flush();

        Debug.Log("스토리모드 점수 서버 전송");
    }

    public void log_endlessmode_round_progress(int round) // 매번 라운드를 지나 이동할 때, 로그를 남기면 몇스테이지까지 보통 진행하는지 확인해서, 시간과 진행도를 모두 확인 가능
    {
        if (!_isInitialized)
            return;

        CustomEvent log_endlessmode_round_progress = new CustomEvent("log_endlessmode_round_progress")
        {
            {"Round", round}
        };

        AnalyticsService.Instance.RecordEvent(log_endlessmode_round_progress);
        AnalyticsService.Instance.Flush();

        Debug.Log("무한모드 점수 서버 전송");
    }

    public void log_weapon_pick_name(string name) // 어떤 무기를 주웠는지 해당 무기 이름 출력;
    {
        if (!_isInitialized)
            return;

        CustomEvent log_weapon_pick_name = new CustomEvent("log_weapon_pick_name")
        {
            {"Name", name}
        };

        AnalyticsService.Instance.RecordEvent(log_weapon_pick_name);
        AnalyticsService.Instance.Flush();

        Debug.Log("획득한 총기 이름 전송");
    }

    public void log_storymode_skill_up_name(string name) // 스킬트리를 찍을 때 마다 해당 스킬이 무엇인지 전송;
    {
        if (!_isInitialized)
            return;

        CustomEvent log_storymode_skill_up_name = new CustomEvent("log_storymode_skill_up_name")
        {
            {"Name", name}
        };

        AnalyticsService.Instance.RecordEvent(log_storymode_skill_up_name);
        AnalyticsService.Instance.Flush();

        Debug.Log("획득한 총기 이름 전송");
    }

    public void log_endlessmode_skill_up_name(string name) // 스킬트리를 찍을 때 마다 해당 스킬이 무엇인지 전송;
    {
        if (!_isInitialized)
            return;

        CustomEvent log_endlessmode_skill_up_name = new CustomEvent("log_endlessmode_skill_up_name")
        {
            {"Name", name}
        };

        AnalyticsService.Instance.RecordEvent(log_endlessmode_skill_up_name);
        AnalyticsService.Instance.Flush();

        Debug.Log("획득한 총기 이름 전송");
    }

}