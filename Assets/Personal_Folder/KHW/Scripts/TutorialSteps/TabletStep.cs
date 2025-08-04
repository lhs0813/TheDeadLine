using System;
using System.Collections;
using Akila.FPSFramework;
using UnityEngine;
using UnityEngine.InputSystem;

public class TabletStep : TutorialStepBase
{
    CharacterInput input;
    public override void ExecuteTutorial()
    {
        TabletController.onTabletShownAction += ShowTabletOpenTutorialInstantly;
        TabletController.onTabletDisabledAction += ShowTabletClosedTutorial;

        //플레이어 멈추기
        input = FindAnyObjectByType<CharacterInput>();
        input.controls.Player.Disable();
        input.controls.Firearm.Disable();
        input.controls.Player.UseTablet.Enable();
        
        //UI 표출.
        TutorialTriggerUIController.Instance.ShowUI("5", 10000f); //그냥 tab을 눌렀더라도, tab이 조작법이라는 것은 표시필요.

        if (TabletController.isTabletActive) //이미 보고있었다면?
        {
            StartCoroutine(TabletOpenTutorialCoroutine(3f));
        }
        else
        {

        }



    }

    void OnDestroy()
    {
        TabletController.onTabletShownAction -= ShowTabletOpenTutorialInstantly;
        TabletController.onTabletDisabledAction -= ShowTabletClosedTutorial;        
    }

    private void ShowTabletOpenTutorialInstantly()
    {
        StartCoroutine(TabletOpenTutorialCoroutine(0f));

    }

    private IEnumerator TabletOpenTutorialCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        TutorialTriggerUIController.Instance.ShowUI("8", 4f);

        yield return new WaitForSeconds(4f);

        TutorialTriggerUIController.Instance.ShowUI("9", 3f);




    }

    private void ShowTabletClosedTutorial()
    {
        input.controls.Player.Enable();
        input.controls.Firearm.Enable();

        Destroy(gameObject, 15f);
    }

}
