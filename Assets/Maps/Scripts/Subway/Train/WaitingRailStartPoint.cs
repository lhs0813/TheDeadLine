using UnityEngine;

public class WaitingRailStartPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train Trigger"))
        {
            other.GetComponent<TrainController>().MoveToWaitingRail();
        }
    }
}
