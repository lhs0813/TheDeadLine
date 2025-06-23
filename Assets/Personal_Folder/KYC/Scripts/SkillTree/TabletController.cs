using UnityEngine;

public class TabletController : MonoBehaviour
{
    public GameObject tabletVisual;   // 태블릿 모델 + UI (껐다 켰다 할 대상)
    public Transform cameraTransform;
    public float appearDistance = 0.5f;
    public float verticalOffset = -0.2f;
    public Vector3 offsetRotation;
    public GameObject weaponUI; // 🔫 총 관련 UI 오브젝트 (비활성화/활성화 대상)
    private bool isTabletVisible = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isTabletVisible)
                HideTablet();
            else
                ShowTablet();
        }

        if (isTabletVisible)
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
        isTabletVisible = true;
        tabletVisual.SetActive(true);              // 🔥 이 오브젝트만 켜고
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        if (weaponUI != null)
            weaponUI.SetActive(false); // 🔫 총 UI 숨기기
    }

    void HideTablet()
    {
        isTabletVisible = false;
        tabletVisual.SetActive(false);             // 🔥 이것만 끄고
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (weaponUI != null)
            weaponUI.SetActive(true); // 🔫 총 UI 다시 보이기
    }
}
