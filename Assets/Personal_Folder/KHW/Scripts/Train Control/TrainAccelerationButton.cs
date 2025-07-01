using System;
using DunGen;
using UnityEngine;

public class TrainAccelerationButton : MonoBehaviour
{
    void Start()
    {
        MapGenerationManager.Instance.runtimeDungeon.Generator.OnGenerationComplete += EnableAccelerationButton;
        GetComponent<BoxCollider>().enabled = false;
        //GamePlayManager.instance.OnStationArriveAction +=
    }

    private void EnableAccelerationButton(DungeonGenerator generator)
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    void EnableAccelerationButton(float f)
    {

    }

    public void DisableAccelerationButton()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// GamePlayManager 활성화 상태를받음.
    /// </summary>
    public void UseAccelerationButton()
    {
        GamePlayManager.instance.AccelerationControl();

        DisableAccelerationButton();
    }

    void OnDisable()
    {
        MapGenerationManager.Instance.runtimeDungeon.Generator.OnGenerationComplete -= EnableAccelerationButton;
    }
}
