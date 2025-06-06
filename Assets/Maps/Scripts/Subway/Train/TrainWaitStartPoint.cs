using UnityEngine;

public class TrainWaitStartPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train Trigger"))
        {
            other.GetComponentInParent<TrainController>().MoveToWaitingRail();
        }
    }
}
