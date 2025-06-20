using Akila.FPSFramework;
using UnityEngine;

public class FuseBoxController : MonoBehaviour
{
    public void OnFuseActivated()
    {
        GamePlayManager.instance.FuseActivated();
        FindAnyObjectByType<UI_Electricity_Info>().GetComponent<Animator>().SetTrigger("On");
        Destroy(gameObject);
    }
}
