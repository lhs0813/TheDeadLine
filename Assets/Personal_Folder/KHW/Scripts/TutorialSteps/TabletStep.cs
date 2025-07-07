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
        TabletController.onTabletShownAction += ShowTabletOpenTutorial;
        TabletController.onTabletDisabledAction += ShowTabletClosedTutorial;

        //플레이어 멈추기
        input = FindAnyObjectByType<CharacterInput>();
        input.controls.Player.Disable();
        input.controls.Firearm.Disable();
        input.controls.Player.UseTablet.Enable();

        //UI 표출.
        TutorialTriggerUIController.Instance.ShowUI("5", 10000f);

    }

    void OnDestroy()
    {
        TabletController.onTabletShownAction -= ShowTabletOpenTutorial;
        TabletController.onTabletDisabledAction -= ShowTabletClosedTutorial;        
    }

    private void ShowTabletOpenTutorial()
    {
        StartCoroutine(TabletOpenTutorialCoroutine());

    }

    private IEnumerator TabletOpenTutorialCoroutine()
    {
        TutorialTriggerUIController.Instance.ShowUI("8", 5f);

        yield return new WaitForSeconds(5f);

        TutorialTriggerUIController.Instance.ShowUI("9", 5f);


    }

    private void ShowTabletClosedTutorial()
    {
        input.controls.Player.Enable();
        input.controls.Firearm.Enable();

        Destroy(gameObject);
    }

}
