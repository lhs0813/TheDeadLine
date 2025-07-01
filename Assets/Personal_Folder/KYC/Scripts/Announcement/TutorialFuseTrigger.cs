using UnityEngine;

public class TutorialFuseTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ObjectiveManager.instance.EnableFuseFindingObjective(0);
    }
}
