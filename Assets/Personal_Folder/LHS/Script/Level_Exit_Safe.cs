using UnityEngine;

public class Level_Exit_Safe : MonoBehaviour
{
    public GameObject SafePosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player 트리거 감지됨");
            other.transform.position = SafePosition.transform.position;
        }
    }
}
