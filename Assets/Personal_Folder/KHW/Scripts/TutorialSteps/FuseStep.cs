using Akila.FPSFramework;
using System.Collections;
using UnityEngine;

public class FuseStep : TutorialStepBase
{
    [SerializeField] GameObject trainTriggerObject;
    CharacterInput input;


    public override void ExecuteTutorial()
    {
        StartCoroutine(FuseTutorialCoroutine());
    }

    private IEnumerator FuseTutorialCoroutine()
    {
        input = FindAnyObjectByType<CharacterInput>();
        input.controls.Player.Disable();
        input.controls.Firearm.Disable();

        TutorialTriggerUIController.Instance.ShowUI("6", 7f);

        GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(7f);

        input.controls.Player.Enable();
        input.controls.Firearm.Enable();
        TutorialTriggerUIController.Instance.ShowUI("10", 7f);

        Destroy(gameObject);
    }



}
