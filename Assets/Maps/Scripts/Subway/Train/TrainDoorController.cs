using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

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
        StartCoroutine(godMode());
    }

    IEnumerator godMode()
    {
        yield return new WaitForSeconds(1.0f);
        Player_Manager.Instance.playerIsGod = false;
        
    }

    public void CloseDoor()
    {
        foreach (var door in trainDoorAnimator)
        {
            door.SetTrigger(DoorCloseTriggerHash);
        }
        Player_Manager.Instance.playerIsGod = true;
    }
}
