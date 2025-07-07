using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-60)]
public class ObjectiveManager : MonoBehaviour
{
    #region singleton - instance

    public static ObjectiveManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }
    #endregion

    UI_Objective_Info objectiveUI;

    void Start()
    {
        ResetFuseCounter();

        //Action subscribes
        GamePlayManager.instance.OnStationArriveAction += EnableFuseFindingObjective;
        GamePlayManager.instance.OnPreDepartAction += DisableObjective;
    }

    #region Fusefinding

    int currentFuseCounter;
    int fuseCounterObjective = 1;

    public Action OnStartFuseFindingObjectiveAction; //역 도착시 목표 시작을 알리는 액션.
    public Action<int, int> OnFindFuseAction; //퓨즈를 찾을 때마다 호출되는 액션.
    public Action OnStartReturnToTheTrainObjectiveAction; //퓨즈를 모두 찾았을 때 복귀를 알리는 액션.
    public Action OnDisableAllObjectiveAction; //열차 출발등, 목표 표시가 필요없을 때 호출되는 액션.

    public void EnableFuseFindingObjective(float obj)
    {
        ResetFuseCounter();

        fuseCounterObjective = GamePlayManager.instance.currentStageInfo.fuseCount;
        OnFindFuseAction?.Invoke(currentFuseCounter, fuseCounterObjective);
        //UI 활성화 
    }

    public void FuseFound()
    {
        currentFuseCounter++; //찾은 퓨즈 개수 추가.

        if (currentFuseCounter >= fuseCounterObjective) //목표 달성 시
        {
            OnStartReturnToTheTrainObjective();
        }
        else
        {
            OnFindFuseAction?.Invoke(currentFuseCounter, fuseCounterObjective); //UI 업데이트.
        }
    }

    private void OnStartReturnToTheTrainObjective()
    {
        OnStartReturnToTheTrainObjectiveAction?.Invoke(); //열차 활성화.
    }

    /// <summary>
    /// 전투시작시 호출할 것.
    /// </summary>
    public void ResetFuseCounter()
    {
        currentFuseCounter = 0;
    }

    /// <summary>
    /// 역 출발시 호출.
    /// </summary>
    public void DisableObjective()
    {
        OnDisableAllObjectiveAction?.Invoke();
    }

    void OnDisable()
    {
        GamePlayManager.instance.OnStationArriveAction -= EnableFuseFindingObjective;
    }

    #endregion
}
