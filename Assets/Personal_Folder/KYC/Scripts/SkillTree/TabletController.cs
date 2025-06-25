using Akila.FPSFramework;
using UnityEngine;

public class TabletController : MonoBehaviour
{
    public static bool isTabletActive = false; //0625 김현우 수정 : inputaction 전역 참조를 위한 사용중 flag. 이름 변경함.
    public InteractionsManager interactionsManager;
    public CharacterInput input;
    public GameObject tabletVisual;   // 태블릿 모델 + UI (껐다 켰다 할 대상)
    public Transform cameraTransform;
    public float appearDistance = 0.5f;
    public float verticalOffset = -0.2f;
    public Vector3 offsetRotation;
    public GameObject weaponUI; // 🔫 총 관련 UI 오브젝트 (비활성화/활성화 대상)


    public AudioSource openSounds;
    public AudioSource closeSounds;

    void Start()
    {
        interactionsManager = FindAnyObjectByType<InteractionsManager>(); //InteractionsManager와의 flag설정을 위함.
        input = FindAnyObjectByType<CharacterInput>();
    }

    void Update()
    {
        if (input.TabletInput)
        {
            if (!isTabletActive)
            {
                ShowTablet();
            }
            else
            {
                HideTablet();
            }
        }

        if (isTabletActive)
        {
            Vector3 targetPos = cameraTransform.position
                + cameraTransform.forward * appearDistance
                + cameraTransform.up * verticalOffset;
            tabletVisual.transform.position = targetPos;
            tabletVisual.transform.rotation = Quaternion.LookRotation(cameraTransform.forward) * Quaternion.Euler(offsetRotation);
        }
    }

    void ShowTablet()
    {
        isTabletActive = true;
        tabletVisual.SetActive(true);              // 🔥 이 오브젝트만 켜고

        openSounds.Play();
        if (weaponUI != null)
            weaponUI.SetActive(false); // 🔫 총 UI 숨기기

        EnableCursor();

        interactionsManager.isActive = false; // 0625 김현우 : 다른 객체와의 상호작용을 불가능하게 변경.
    }

    void HideTablet()
    {
        isTabletActive = false;
        tabletVisual.SetActive(false);             // 🔥 이것만 끄고

        closeSounds.Play();

        if (weaponUI != null)
            weaponUI.SetActive(true); // 🔫 총 UI 다시 보이기

        DisableCursor();

        interactionsManager.isActive = false; // 0625 김현우 : 다른 객체와의 상호작용을 가능하게 변경.
    }

    void EnableCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void DisableCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void ManageCursor()
    {

    }
}
