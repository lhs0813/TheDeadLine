using UnityEngine;

public class WaitingRailEndPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.name}");

        if (other.CompareTag("Train Trigger"))
        {
            other.GetComponentInParent<TrainController>().MoveToWaitingRail();
        }
    }
}
