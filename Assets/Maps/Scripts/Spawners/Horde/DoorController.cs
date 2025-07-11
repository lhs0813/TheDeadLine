// DoorController.cs
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void OpenDoor()
    {
        animator.ResetTrigger("Close");
        animator.SetTrigger("Open");
    }

    public void CloseDoor()
    {
        animator.ResetTrigger("Open");
        animator.SetTrigger("Close");
    }
}
