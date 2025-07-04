using UnityEngine;

public class TutorialTrainDoorTrigger : MonoBehaviour
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


        FindAnyObjectByType<TrainDoorController>().OpenDoor();
    }
}
