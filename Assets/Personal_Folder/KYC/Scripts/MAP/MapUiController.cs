using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class MapUIController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI statusText; // 상단 상태 텍스트 (Train Going, Arriving at 등)
    public TextMeshProUGUI centerStationText; // 가운데 크게 보이는 역
    public TextMeshProUGUI[] stationNameTexts; // 오른쪽에 뜨는 3개 역 (index, index+1, index+2)

    [Header("Station Names")]
    public List<string> stationPool = new List<string>(); // 전체 역 이름 리스트

    private void Start()
    {
        UpdateUI();
        // Combat 끝났을 때 역 이름 갱신
        GamePlayManager.instance.OnPreDepartAction += UpdateStationNames;

        // 상태 갱신
        GamePlayManager.instance.OnStationDepartAction += _ => UpdateUI();
        GamePlayManager.instance.OnStationArriveAction += _ => UpdateUI();
        GamePlayManager.instance.OnDangerAction += UpdateUI;
    }

    private void Update()
    {
        UpdateStateLabel();
    }

    private void UpdateUI()
    {
        UpdateStateLabel();
        UpdateStationNames();
    }

    private void UpdateStateLabel()
    {
        string label = GetLabelForState(GamePlayManager.instance.currentGameState);
        statusText.text = label;
    }

    private string GetLabelForState(GameState state)
    {
        return state switch
        {
            GameState.PreDeparting or GameState.Departing or GameState.Danger => "Next Station",
            GameState.Waiting => "Train Going",
            GameState.Entering => "Arriving at",
            GameState.Combat => "Current Station",
            _ => ""
        };
    }

    private void UpdateStationNames()
    {
        int index = GamePlayManager.instance.currentMapIndex;

        // 🔁 상태가 "Next Station"일 경우, 다음 역을 중심에 표시
        GameState state = GamePlayManager.instance.currentGameState;
        int centerIndex = (state == GameState.PreDeparting || state == GameState.Departing || state == GameState.Danger)
            ? index + 1
            : index;

        // 중앙역 텍스트
        centerStationText.text = GetStationName(centerIndex);

        // 오른쪽 역 3개 텍스트
        for (int i = 0; i < stationNameTexts.Length; i++)
        {
            int targetIndex = index + i;
            stationNameTexts[i].text = GetStationName(targetIndex);
        }
    }

    private string GetStationName(int index)
    {
        if (index >= 0 && index < stationPool.Count)
            return stationPool[index];
        else
            return "???";
    }
}
