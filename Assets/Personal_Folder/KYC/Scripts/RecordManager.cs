using UnityEngine;

public class RecordManager : MonoBehaviour
{
    public static RecordManager Instance { get; private set; }

    // PlayerPrefs 키
    private const string StoryTimeKey = "StoryClearTime";
    private const string InfiniteStageKey = "InfiniteMaxStage";

    // 스토리 모드 타이머
    [SerializeField] private float _storyStartTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // —— 스토리 모드 시간 기록 ——
    /// <summary>
    /// 스토리 모드 시작 시 호출
    /// </summary>
    public void StartStoryTimer()
    {
        _storyStartTime = Time.time;
    }

    /// <summary>
    /// 엔딩 씬에서 탈출구 상호작용 시 호출
    /// </summary>
    public void StopStoryTimer()
    {
        float clearTime = Time.time - _storyStartTime;
        SaveStoryTime(clearTime);
    }

    private void SaveStoryTime(float time)
    {
        PlayerPrefs.SetFloat(StoryTimeKey, time);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 저장된 스토리 모드 클리어 시간(초). 저장된 값이 없으면 float.MaxValue 반환
    /// </summary>
    public float LoadStoryTime()
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
