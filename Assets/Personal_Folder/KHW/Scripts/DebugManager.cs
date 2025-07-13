using System;
using DunGen;
using DunGen.Graph;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [Header("Console Texts")]
    public TextMeshProUGUI stateText;      // 마지막 명령어 결과 (한 줄)
    public TextMeshProUGUI stageInfoText;  // 맵 로드 완료 시 정보
    public TextMeshProUGUI updateText;     // 매 프레임 갱신 정보

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

    // 디버그 모드 활성화 상태
    private bool debugActive = false;

    void Start()
    {
        // 시작 시 완전히 숨겨두기
        debugCanvas.enabled = debugActive;
        GamePlayManager.instance.OnMapLoadFinishingAction += UpdateMapInfo;
    }

    void OnDestroy()
    {
        GamePlayManager.instance.OnMapLoadFinishingAction -= UpdateMapInfo;
    }

    /// <summary>
    /// 맵 로드 완료 시 한 번만 호출되어 stageInfoText를 갱신합니다.
    /// </summary>
    private void UpdateMapInfo(int stageIndex)
    {
        DungeonFlow flow = FindAnyObjectByType<Dungeon>().DungeonFlow;
        int gunCount       = FindObjectsByType<GunSpawner>(FindObjectsSortMode.None).Length;
        int skillChipCount = FindObjectsByType<DataChip_To_SkillPoint>(FindObjectsSortMode.None).Length;
        int fuseCount      = FindObjectsByType<FuseBoxController>(FindObjectsSortMode.None).Length;

        string gunProbs   = SpawnedGunBuilder.GetGradeProbabilitiesText(stageIndex);
        string enemyProbs = HordeSpawnBuilder.GetSpawnWeightsText(stageIndex);
        string enemyHPs   = EnemyConstants.GetZombieHPText(stageIndex);
        string enemyDMGs  = EnemyConstants.GetZombieDamageText(stageIndex);

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

        // F1~F4: 디버그 전용 키 입력 처리
        if (Input.GetKeyDown(KeyCode.F1) && !isStoryMode)
        {
            GamePlayManager.instance.currentMapIndex++;
            stageIndexDebugString = $"Next Station Number : {GamePlayManager.instance.currentMapIndex + 1}";
            SetStateString(stageIndexDebugString);
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

    private void UpdateEnemyCount()
    {
        currentEnemyCountString = $"EnemyCount : {EnemyPoolManager.Instance.activeEnemies.Count}";
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
