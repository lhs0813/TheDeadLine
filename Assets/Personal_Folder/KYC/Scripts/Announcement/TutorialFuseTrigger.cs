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

        ObjectiveManager.instance.EnableFuseFindingObjective(0);
        FindAnyObjectByType<TrainDoorController>().OpenDoor();
        isAlreadyTriggered = true;
    }
}
