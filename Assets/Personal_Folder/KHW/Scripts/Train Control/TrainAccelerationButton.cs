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

    void DisableAccelerationButton()
    {
        GetComponent<BoxCollider>().enabled = false;        
    }
}
