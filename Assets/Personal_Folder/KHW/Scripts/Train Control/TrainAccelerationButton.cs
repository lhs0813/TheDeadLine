using UnityEngine;

public class TrainAccelerationButton : MonoBehaviour
{
    void Start()
    {
        GamePlayManager.instance.OnStationDepartAction += EnableAccelerationButton;
        //GamePlayManager.instance.OnStationArriveAction +=
    }

    void EnableAccelerationButton(float f)
    {
        GetComponent<BoxCollider>().enabled = true;
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
        GamePlayManager.instance.OnStationDepartAction -= EnableAccelerationButton;
    }
}
