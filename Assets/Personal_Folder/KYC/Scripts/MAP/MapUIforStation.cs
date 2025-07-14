using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MapUIForStation : MonoBehaviour
{
    [Header("Alternate 전용 씬 이름")]
    [SerializeField] private string alternateSceneName = "EndlessModeScene";

    [Header("Default 모드용 컨테이너")]
    [SerializeField] private GameObject defaultMapImage;

    [Header("Alternate 모드용 컨테이너")]
    [SerializeField] private GameObject alternateMapImage;

    [Header("Default 모드용 오브젝트들")]
    [SerializeField] private GameObject[] mapUIObjects;

    [Header("Endless 모드용 인덱스 텍스트 (AlternateMapImage의 자식)")]
    [SerializeField] private TextMeshProUGUI endlessIndexText;

    private void Start()
    {
        // 텍스트는 처음에 비활성화
        if (endlessIndexText != null)
            endlessIndexText.gameObject.SetActive(false);

        UpdateMapUI(GamePlayManager.instance.currentMapIndex);
    }

    private void OnEnable()
    {
        if (GamePlayManager.instance != null)
            GamePlayManager.instance.OnMapLoadFinishingAction += UpdateMapUI;
    }

    private void OnDisable()
    {
        if (GamePlayManager.instance != null)
            GamePlayManager.instance.OnMapLoadFinishingAction -= UpdateMapUI;
    }

    private void UpdateMapUI(int index)
    {
        bool isAlternate = SceneManager.GetActiveScene().name == alternateSceneName;

        // 1) Default 레이어 토글
        defaultMapImage?.SetActive(!isAlternate);
        alternateMapImage?.SetActive(isAlternate);

        // 2) Default 모드일 때만 Index0~Index10 켜기
        if (!isAlternate)
        {
            index = Mathf.Clamp(index, 0, mapUIObjects.Length - 1);
            for (int i = 0; i < mapUIObjects.Length; i++)
                mapUIObjects[i].SetActive(i == index);

            // 텍스트는 숨기기
            endlessIndexText?.gameObject.SetActive(false);
        }
        else
        {
            // 3) Endless 모드: 텍스트 켜고 'index - 5' 표시
            int display = GamePlayManager.instance.currentMapIndex - 2;
            endlessIndexText?.gameObject.SetActive(true);
            endlessIndexText.text = $"Station {display}";
        }
    }
}
