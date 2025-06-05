using UnityEngine;

public class TrainStopPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train Trigger"))
        {
            other.GetComponent<TrainController>().TrainArrive();
        }

        Destroy(gameObject);
    }
}
