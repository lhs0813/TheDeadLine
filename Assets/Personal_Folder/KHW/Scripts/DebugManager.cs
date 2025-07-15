using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akila.FPSFramework;
using DunGen;
using DunGen.Graph;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  // for Text

public class DebugManager : MonoBehaviour
{
    [Header("Console Texts")]
    public TextMeshProUGUI stateText;      // 마지막 명령어 결과 (한 줄)
    public TextMeshProUGUI stageInfoText;  // 맵 로드 완료 시 정보
    public TextMeshProUGUI updateText;     // 매 프레임 갱신 정보
    public TextMeshProUGUI gunTypeText; //총 종류별 개수 표시할 텍스트

    [Header("Debug UI Canvas")]
    public Canvas debugCanvas;             // 토글할 디버그용 Canvas

    [Header("Control Stage Index.")]
    public bool isStoryMode;
    private string stageIndexDebugString = "";

    [Header("Make Player Invincible")]
    private bool isPlayerGod = false;
    private string godModeString = "";

    [Header("Make Skill Point 9999")]
    private string skillPointString = "";

    [Header("Get Weapon Set")]
    public GameObject weaponSetPrefab;
    private string weaponString = "";

    // 맵 로드 후 정보
    private string mapInfoString = "";
    // 매 프레임 갱신할 적 개수
    private string currentEnemyCountString = "";

    private string spawnedGunCountString = "";

    // 디버그 모드 활성화 상태
    private bool debugActive = false;

    [Header("Stage Input Field")]
    public TMP_InputField stageInputField;
    private bool isTypingStage = false;

    void Start()
    {
        // 시작 시 완전히 숨겨두기
        debugCanvas.enabled = debugActive;
        GamePlayManager.instance.OnMapLoadFinishingAction += UpdateMapInfo;
        GamePlayManager.instance.OnMapLoadFinishingAction += UpdateGunCountInfo;

        // 입력 끝났을 때 호출될 메서드 연결
        stageInputField.onEndEdit.AddListener(OnStageInputSubmitted);
        stageInputField.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        GamePlayManager.instance.OnMapLoadFinishingAction -= UpdateMapInfo;
        GamePlayManager.instance.OnMapLoadFinishingAction -= UpdateGunCountInfo;

        stageInputField.onEndEdit.RemoveListener(OnStageInputSubmitted);
    }

    /// <summary>
    /// 맵 로드 완료 시 한 번만 호출되어 stageInfoText를 갱신합니다.
    /// </summary>
    private void UpdateMapInfo(int stageIndex)
    {
        DungeonFlow flow = FindAnyObjectByType<Dungeon>().DungeonFlow;
        int gunCount = FindObjectsByType<GunSpawner>(FindObjectsSortMode.None).Length;
        int skillChipCount = FindObjectsByType<DataChip_To_SkillPoint>(FindObjectsSortMode.None).Length;
        int fuseCount = FindObjectsByType<FuseBoxController>(FindObjectsSortMode.None).Length;

        string gunProbs = SpawnedGunBuilder.GetGradeProbabilitiesText(stageIndex);
        string enemyProbs = HordeSpawnBuilder.GetSpawnWeightsText(stageIndex);
        string enemyHPs = EnemyConstants.GetZombieHPText(stageIndex);
        string enemyDMGs = EnemyConstants.GetZombieDamageText(stageIndex);

        mapInfoString =
            $"Map Loaded - Stage {stageIndex}\n" +
            $"Flow: {flow.name}\n" +
            $"Gun Spawners: {gunCount}\n" +
            $"Skill Chips: {skillChipCount}\n" +
            $"Fuses: {fuseCount}\n\n" +
            $"-- Gun Probabilities --\n{gunProbs}\n\n" +
            $"-- Enemy Spawn Weights --\n{enemyProbs}\n\n" +
            $"-- Enemy HP --\n{enemyHPs}\n\n" +
            $"-- Enemy Damage --\n{enemyDMGs}";

        stageInfoText.text = mapInfoString;
    }

    void UpdateGunCountInfo(int i)
    {
        // 1) 모든 PickableEffect 찾기
        var pickableEffects = FindObjectsByType<PickableEffect>(FindObjectsSortMode.None);

        // 2) 이름 → (count, grade) 딕셔너리 초기화
        var dict = new Dictionary<string, (int count, int grade)>();

        foreach (var effect in pickableEffects)
        {
            var pickable = effect.GetComponent<Pickable>();
            if (pickable == null) continue;

            string name = pickable.Name;

            Firearm firearm = pickable.item.GetComponent<Firearm>();
            int grade = (firearm != null) ? firearm.gradeNum : -1;
            //Debug.Log($"[Debug] {pickable.Name} 의 firearm: {firearm}, gradeNum: {grade}");


            if (dict.ContainsKey(name))
            {
                // 이미 있으면 개수만 증가
                var entry = dict[name];
                dict[name] = (entry.count + 1, entry.grade);
            }
            else
            {
                // 새로 추가
                dict[name] = (1, grade);
            }
        }

        // 3) grade 오름차순(0→1→2), 동일 grade 내에서는 이름순 정렬
        var ordered = dict
            .OrderBy(kv => kv.Value.grade)
            .ThenBy(kv => kv.Key);

        // 4) 색상 매핑
        string GradeToColor(int g)
        {
            switch (g)
            {
                case 1: return "blue";    // 등급0 → 파랑
                case 2: return "purple";  // 등급1 → 보라
                case 3: return "orange";  // 등급2 → 오렌지
                default: return "white";
            }
        }

        // 5) 문자열 조합
        var sb = new StringBuilder();
        foreach (var kv in ordered)
        {
            string color = GradeToColor(kv.Value.grade);
            sb.AppendLine($"<color={color}>{kv.Key}: {kv.Value.count}</color>");
        }

        // 6) UI에 반영
        spawnedGunCountString = sb.ToString();
        gunTypeText.text = spawnedGunCountString;
    }

    void Update()
    {
        // ‵ 키를 누르면 debugActive 토글, Canvas 표시/숨김
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            debugActive = !debugActive;
            debugCanvas.enabled = debugActive;
            // 디버그를 켰을 때 맨 처음 상태 텍스트 초기화
            if (debugActive) SetStateString("Debug Mode Activated");
        }

        // 디버그 모드가 꺼져 있으면 이후 로직은 모두 건너뜀
        if (!debugActive)
            return;

        // 아직 입력중이라면 숫자 입력만 받고, F1 로직은 막기
        if (isTypingStage)
            return;

        // F1 누르면 입력 필드 활성화
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isTypingStage = true;
            stageInputField.text = "";
            stageInputField.gameObject.SetActive(true);
            stageInputField.ActivateInputField();
            return;
        }


        if (Input.GetKeyDown(KeyCode.F2))
        {
            isPlayerGod = !isPlayerGod;
            FindAnyObjectByType<Player_Manager>().playerIsGod = isPlayerGod;
            godModeString = isPlayerGod ? "GodMode Activated" : "GodMode Deactivated";
            SetStateString(godModeString);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            FindAnyObjectByType<SkillTreeManager>().availablePoints = 9999;
            skillPointString = "Obtain 9999 Skill Points";
            SetStateString(skillPointString);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            var player = FindAnyObjectByType<Player_Manager>();
            Instantiate(weaponSetPrefab, player.transform.position, player.transform.rotation);
            weaponString = "Summon All Weapons.";
            SetStateString(weaponString);
        }

        // 매 프레임: 적 수 갱신 & 화면 갱신
            UpdateEnemyCount();
        UpdateDebugString();
    }

    private void OnStageInputSubmitted(string input)
    {
        // Enter나 focus 잃었을 때 호출
        if (int.TryParse(input, out int newStage) && newStage > 0)
        {
            // 인스펙터에선 0부터 시작하니 -1
            GamePlayManager.instance.currentMapIndex = newStage - 1;
            stageIndexDebugString = $"Station Number : {newStage}";
            SetStateString(stageIndexDebugString);
        }
        else
        {
            SetStateString("Invalid Stage Number");
        }

        // 입력 필드 감추기
        stageInputField.gameObject.SetActive(false);
        isTypingStage = false;
    }

    private void UpdateEnemyCount()
    {
        currentEnemyCountString = $"EnemyCount : {EnemyPoolManager.Instance.activeEnemies.Count}\n"
                                + $"NormalZombie : {EnemyPoolManager.Instance.GetActiveCount(EnemyType.Normal)}\n"
                                + $"TankZombie : {EnemyPoolManager.Instance.GetActiveCount(EnemyType.Big)}\n"
                                + $"BombZombie : {EnemyPoolManager.Instance.GetActiveCount(EnemyType.Bomb)}\n"
                                + $"FastZombie : {EnemyPoolManager.Instance.GetActiveCount(EnemyType.Fast)}";

    }

    private void UpdateDebugString()
    {
        // F1으로 바뀐 스테이지 인덱스 텍스트가 있으면 그걸 위에, 없으면 적 개수만
        string info = string.IsNullOrEmpty(stageIndexDebugString)
            ? currentEnemyCountString
            : $"{stageIndexDebugString}\n{currentEnemyCountString}";

        updateText.text = info;
    }

    private void SetStateString(string newString)
    {
        stateText.text = newString;
    }
}
