using System.Threading.Tasks;
using UnityEngine;

public class TrainWaitStartPoint : MonoBehaviour
{
    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train Trigger"))
            await SendWaitState();
    }

    private async Task SendWaitState()
    { 
        await GamePlayManager.instance.GoWaitingState();
    }

}
