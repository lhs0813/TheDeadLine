using Akila.FPSFramework;
using UnityEngine;

public class FuseBoxController : MonoBehaviour
{
    public void OnFuseActivated()
    {
        GamePlayManager.instance.FuseActivated();
    }
}
