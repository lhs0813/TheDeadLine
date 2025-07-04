using UnityEngine;

public class TutorialFuseTrigger : MonoBehaviour
{
    bool isAlreadyTriggered = false;

    void Start()
    {
        isAlreadyTriggered = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (isAlreadyTriggered) return;

        GamePlayManager.instance.GoFirstCombatState();
        ObjectiveManager.instance.EnableFuseFindingObjective(0);
        isAlreadyTriggered = true;
    }
}
