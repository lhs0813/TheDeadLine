using UnityEngine;

public class TrainStopPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train Trigger"))
        {
            Debug.Log("열차가 플랫폼에 도착");
            GamePlayManager.instance.GoCombatState();
        }

        Destroy(gameObject, 5f);
    }
}
