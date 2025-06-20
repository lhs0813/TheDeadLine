using UnityEngine;

public class FuseBoxController : MonoBehaviour
{
    bool isAlreadyActivated = false;

    public void OnFuseActivated()
    {
        if (!isAlreadyActivated)
        {
            isAlreadyActivated = true;
            GamePlayManager.instance.FuseActivated();
        }

    }
}
