using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TrainDoorController : MonoBehaviour
{
    List<Animator> trainDoorAnimator;
    int DoorOpenTriggerHash;
    int DoorCloseTriggerHash;


    void Awake()
    {
        trainDoorAnimator = GetComponentsInChildren<Animator>().ToList();
    }

    void Start()
    {
        InitializeAnimationHash();
    }

    void InitializeAnimationHash()
    {
        DoorOpenTriggerHash = Animator.StringToHash("T_Open");
        DoorCloseTriggerHash = Animator.StringToHash("T_Close");
    }

    public void OpenDoor()
    {
        foreach (var door in trainDoorAnimator)
        {
            door.SetTrigger(DoorOpenTriggerHash);
        }
    }

    public void CloseDoor()
    {
        foreach (var door in trainDoorAnimator)
        {
            door.SetTrigger(DoorCloseTriggerHash);
        }
    }
}
