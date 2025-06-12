using UnityEngine;
using Cinemachine;
using System;

public class RotateOnTrigger : MonoBehaviour
{
    public static Action onLapTop;
    public static Action offLapTop;

    public GameObject needToHideUI;
    public GameObject needToHideUI2;
    public GameObject targetObject;        // 회전시킬 오브젝트
    public float openAngle = -20f;         // 열릴 때 X축 회전 각도
    private float closeAngle = 89.7f;          // 닫힐 때 X축 회전 각도
    public float rotationSpeed = 200f;      // 회전 속도
    public GameObject skillUI;

    public bool isLapTopOn = false;

    private bool isPlayerInside = false;

    [Header("Cinemachine Virtual Cameras")]
    [SerializeField] private CinemachineVirtualCamera fpsCam;
    [SerializeField] private CinemachineVirtualCamera laptopCam;
    [SerializeField] private MonoBehaviour playerControlScript;



    private void Start()
    {
        skillUI.SetActive(false);
    }
    void Update()
    {
        if (targetObject == null)
            return;

        // 현재 X축 회전값 (보정 포함)
        float currentX = targetObject.transform.rotation.eulerAngles.x;
        if (currentX > 180f) currentX -= 360f;

        // 목표 회전값 결정
        float targetX = isPlayerInside ? openAngle : closeAngle;

        // 부드럽게 회전
        float newX = Mathf.MoveTowards(currentX, targetX, rotationSpeed * Time.deltaTime);
        Vector3 newRotation = new Vector3(
            newX,
            targetObject.transform.rotation.eulerAngles.y,
            targetObject.transform.rotation.eulerAngles.z
        );
        targetObject.transform.rotation = Quaternion.Euler(newRotation);
    }

    public void LabtopOn()
    {
        onLapTop?.Invoke();

        isPlayerInside = true;
        skillUI.SetActive(true);
        isLapTopOn = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        needToHideUI.SetActive(false);
        needToHideUI2.SetActive(false);
        // 🔄 카메라 전환
        fpsCam.Priority = 0;
        laptopCam.Priority = 20;

        // 🎮 플레이어 조작 비활성화
        playerControlScript.enabled = false;
    }


    public void LabtopOff()
    {
        offLapTop?.Invoke();

        isPlayerInside = false;
        skillUI.SetActive(false);
        isLapTopOn = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 🔄 카메라 원래대로
        fpsCam.Priority = 20;
        laptopCam.Priority = 0;
        needToHideUI.SetActive(true);
        needToHideUI2.SetActive(true);
        // 🎮 플레이어 조작 복구
        playerControlScript.enabled = true;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }*/

    /*private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }*/
}
