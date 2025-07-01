using UnityEngine;

public class MapUIForStation : MonoBehaviour
{
    [SerializeField] private GameObject[] mapUIObjects; // 총 11개 오브젝트 등록 (0~10)
    void Start()
    {
        UpdateMapUI(GamePlayManager.instance.currentMapIndex);
    }
    private void OnEnable()
    {
        // GamePlayManager에서 맵 로딩 완료될 때 호출되도록 연결
        if (GamePlayManager.instance != null)
        {
            GamePlayManager.instance.OnMapLoadFinishingAction += UpdateMapUI;
        }
    }

    private void OnDisable()
    {
        if (GamePlayManager.instance != null)
        {
            GamePlayManager.instance.OnMapLoadFinishingAction -= UpdateMapUI;
        }
    }

    private void UpdateMapUI(int index)
    {
        // 인덱스가 범위 초과하지 않도록 방어코드
        index = Mathf.Clamp(index, 0, mapUIObjects.Length - 1);

        for (int i = 0; i < mapUIObjects.Length; i++)
        {
            mapUIObjects[i].SetActive(i == index);
        }
    }
}
