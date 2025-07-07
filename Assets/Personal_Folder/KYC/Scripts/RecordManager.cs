using UnityEngine;

public class RecordManager : MonoBehaviour
{
    public static RecordManager Instance { get; private set; }

    private const string StoryTimeKey = "StoryClearTime";
    private const string InfiniteStageKey = "InfiniteMaxStage";

    private float _storyStartTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void StartStoryTimer()
    {
        _storyStartTime = Time.time;
    }

    /// <summary>
    /// 스토리 모드 클리어 시 호출.
    /// 기록 갱신 여부 비교 후 true 최단 기록을 세이브,
    /// 항상 이번 세션의 clearTime을 리턴.
    /// </summary>
    public float StopStoryTimer()
    {
        float clearTime = Time.time - _storyStartTime;

        // 이전 최단 기록 로드
        float prevBest = PlayerPrefs.GetFloat(StoryTimeKey, float.MaxValue);
        // 이번 기록이 더 짧으면 갱신
        if (clearTime < prevBest)
        {
            PlayerPrefs.SetFloat(StoryTimeKey, clearTime);
            PlayerPrefs.Save();
        }

        return clearTime;
    }

    /// <summary>
    /// 저장된 최단 스토리 모드 클리어 타임을 반환.
    /// 기록 없으면 float.MaxValue
    /// </summary>
    public float GetBestStoryTime()
    {
        return PlayerPrefs.GetFloat(StoryTimeKey, float.MaxValue);
    }

    // —— 무한 모드 스테이지 기록 ——
    /// <summary>
    /// 무한 모드에서 죽었을 때 호출
    /// </summary>
    /// <param name="stage">현재 도달한 스테이지 번호</param>
    public void RecordInfiniteStage(int stage)
    {
        int prevBest = LoadInfiniteStage();
        if (stage > prevBest)
        {
            PlayerPrefs.SetInt(InfiniteStageKey, stage);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 저장된 최고 무한 모드 스테이지 (없으면 0)
    /// </summary>
    public int LoadInfiniteStage()
    {
        return PlayerPrefs.GetInt(InfiniteStageKey, 0);
    }
}
