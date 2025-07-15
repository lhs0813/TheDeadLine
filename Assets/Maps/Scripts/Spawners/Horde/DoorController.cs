// DoorController.cs
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // 추가할 부분 ↓
    [SerializeField] private AudioSource audioSource;    // 문 소리를 재생할 AudioSource
    [SerializeField] private AudioClip openClip;         // 열릴 때 한 번 재생할 클립

    public void OpenDoor()
    {
        animator.ResetTrigger("Close");
        animator.SetTrigger("Open");

        // 사운드 재생
        if (audioSource != null && openClip != null)
            audioSource.PlayOneShot(openClip);
    }

    public void CloseDoor()
    {
        animator.ResetTrigger("Open");
        animator.SetTrigger("Close");
    }
}
