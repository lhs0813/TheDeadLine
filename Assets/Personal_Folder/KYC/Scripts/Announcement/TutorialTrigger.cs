using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    public string triggerID;
    private GameObject _uiToShow;
    private Coroutine _hideCoroutine;

    private void Start()
    {
        _uiToShow = TutorialTriggerManager.Instance.GetUIByID(triggerID);
        if (_uiToShow != null)
            _uiToShow.SetActive(false); // 시작 시 꺼두기 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GetComponent<TutorialStepBase>().ExecuteTutorial();
        //TutorialTriggerUIController.Instance.ShowUI(triggerID);
    }
}
