using UnityEngine;
using Cinemachine;
using System;
using Akila.FPSFramework;

public class RotateOnTrigger : MonoBehaviour
{
    [Header("Laptop Actions")]
    public static Action onLapTop;
    public static Action offLapTop;

    [Header("Gameobject References")]

    public GameObject needToHideUI;
    public GameObject needToHideUI2;
    public GameObject targetObject;        // 회전시킬 오브젝트
    public GameObject skillUIWithElectricity; //전력 공급이 있을 때의 화면
    public GameObject skillUIWithNoElectricity; //전력 공급이 없을 때의 화면

    [Header("Laptop Variables")]
    public float openAngle = -20f;         // 열릴 때 X축 회전 각도
    private float closeAngle = 89.7f;          // 닫힐 때 X축 회전 각도
    public float rotationSpeed = 200f;      // 회전 속도

    public bool isLapTopOn = false;
    private bool isPlayerInside = false;
    private bool electricityCharged = true;

    [Header("Cinemachine Virtual Cameras")]
    [SerializeField] private CinemachineVirtualCamera fpsCam;
    [SerializeField] private CinemachineVirtualCamera laptopCam;
    [SerializeField] private MonoBehaviour playerControlScript;


    // 0620 김현우 수정 : 전력공급 여부에 따라서, 열릴 시 로딩하는 캔버스를 결정하도록 함.

    private void Start()
    {
        //TODO : 시작 위치에 따라 다름.
        skillUIWithElectricity.SetActive(true);
        skillUIWithNoElectricity.SetActive(false);

        //Action 구독.
        GamePlayManager.instance.OnElectricityOnAction += ActivateLaptop; //전력 공급시 스킬 UI를 로딩할 수 있도록.
        GamePlayManager.instance.OnElectricityOffAction += DeactivateLaptop; //전력 끊길 시 스킬 UI를 대체하도록.
        GamePlayManager.instance.OnStationDepartAction += SetLaptopNotUsable;
        GamePlayManager.instance.OnStationArriveAction += SetLaptopUsable;
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

    private void ActivateLaptop()
    {
        electricityCharged = true;
    }

    private void DeactivateLaptop()
    {
        electricityCharged = false;
    }

    private void SetLaptopNotUsable(float f) //노트북과의 상호작용을 차단.
    {
        GetComponent<Pickable>().enabled = false;
    }

    private void SetLaptopUsable(float f) //노트북과의 상호작용을 허용.
    {
        GetComponent<Pickable>().enabled = true;
    }

    public void LabtopOn()
    {
        ///노트북 열릴 시의 Input 비활성화 로직들 수행.
        onLapTop?.Invoke();

        isPlayerInside = true;

        if (electricityCharged)
        {
            skillUIWithElectricity.SetActive(true);
            skillUIWithNoElectricity.SetActive(false);
        }
        else
        {
            skillUIWithElectricity.SetActive(false);
            skillUIWithNoElectricity.SetActive(true);            
        }

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


        skillUIWithElectricity.SetActive(false);
        skillUIWithNoElectricity.SetActive(false);


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

    void OnDisable()
    {
        GamePlayManager.instance.OnElectricityOnAction -= ActivateLaptop;
        GamePlayManager.instance.OnElectricityOffAction -= DeactivateLaptop; 
        GamePlayManager.instance.OnStationDepartAction -= SetLaptopNotUsable;
        GamePlayManager.instance.OnStationArriveAction -= SetLaptopUsable;        
    }
}
