using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }
    private bool _isInitialized = false;

    private Dictionary<string, int> _pickedWeaponCounts = new Dictionary<string, int>();
    private Dictionary<string, int> _pickedSkillCounts = new Dictionary<string, int>();

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
        //AnalyticsService.Instance.Flush();

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
        //AnalyticsService.Instance.Flush();

        Debug.Log("무한모드 점수 서버 전송");
    }

    public void WeaponPick_Dictionary(string weaponName)
    {
        if (!_isInitialized)
            return;

        if (_pickedWeaponCounts.ContainsKey(weaponName))
            _pickedWeaponCounts[weaponName]++;
        else
            _pickedWeaponCounts[weaponName] = 1;

        Debug.Log($"무기 획득 기록: {weaponName}");
    }

    public void log_send_weapon_pick_summary()
    {
        if (!_isInitialized || _pickedWeaponCounts.Count == 0)
            return;

        CustomEvent log_send_weapon_pick_summary = new CustomEvent("log_send_weapon_pick_summary");

        foreach (var entry in _pickedWeaponCounts)
        {
            log_send_weapon_pick_summary[$"weapon_{entry.Key}"] = entry.Value;
        }

        AnalyticsService.Instance.RecordEvent(log_send_weapon_pick_summary);
        //AnalyticsService.Instance.Flush();

        Debug.Log("무기 요약 이벤트 전송 완료");

        _pickedWeaponCounts.Clear();
    }

    /*public void log_weapon_pick_name(string name) // 어떤 무기를 주웠는지 해당 무기 이름 출력;
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
    }*/

    public void SkillnPick_Dictionary(string Skill_Id)
    {
        if (!_isInitialized)
            return;

        if (_pickedSkillCounts.ContainsKey(Skill_Id))
            _pickedSkillCounts[Skill_Id]++;
        else
            _pickedSkillCounts[Skill_Id] = 1;

        Debug.Log($"스킬포인트 투자 기록: {Skill_Id}");
    }

    public void log_skill_pick_summary()
    {
        if (!_isInitialized || _pickedSkillCounts.Count == 0)
            return;

        CustomEvent log_skill_pick_summary = new CustomEvent("log_skill_pick_summary");

        foreach (var entry in _pickedSkillCounts)
        {
            log_skill_pick_summary[$"weapon_{entry.Key}"] = entry.Value;
        }

        AnalyticsService.Instance.RecordEvent(log_skill_pick_summary);
        //AnalyticsService.Instance.Flush();

        Debug.Log("스킬포인트 요약 이벤트 전송");

        _pickedSkillCounts.Clear();
    }

    public void log_storymode_clear_time(float clearTime)
    {
        if (!_isInitialized)
            return;

        CustomEvent log_storymode_clear_time = new CustomEvent("log_storymode_clear_time")
        {
            {"clearTime", clearTime}
        };

        AnalyticsService.Instance.RecordEvent(log_storymode_clear_time);
        //AnalyticsService.Instance.Flush();

        Debug.Log("스토리 모드 클리어 타임 전송 완료");
    }

}