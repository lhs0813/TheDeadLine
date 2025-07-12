using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class Objective_Mission : MonoBehaviour
{
    Animator anim;

    // ① TextMeshPro 대신 LocalizeStringEvent 컴포넌트로 바꿉니다
    [SerializeField] private LocalizeStringEvent missionTextEvent;

    // 포맷 문자열 캐싱은 Smart Formatter 쓸 때 필요 없으므로 지워도 됩니다
    // private string fuseFormat;

    private void Start()
    {
        GamePlayManager.instance.OnTrainArriveAction += ObjectiveMission;
        anim = GetComponent<Animator>();

        // fuseFormat 캐싱 대신, missionTextEvent는 Inspector에서 세팅
    }

    void ObjectiveMission()
    {
        int fuseCount = GamePlayManager.instance.currentStageInfo.fuseCount;

        // ② 퓨즈 개수 업데이트
        UpdateFuseUI(fuseCount);

        anim.SetTrigger("On");
    }

    // 이 메서드를 같은 파일에 추가하세요
    private void UpdateFuseUI(int count)
    {
        // 매번 새로운 값 설정
        missionTextEvent.StringReference.Arguments = new object[] { count };
        // 강제 갱신 (필수)
        missionTextEvent.RefreshString();
    }
}
