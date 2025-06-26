using Akila.FPSFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class TabletController : MonoBehaviour
{
    public static bool isTabletActive = false;
    public static Action onTabletShownAction;
    public static Action onTabletDisabledAction;
    public InteractionsManager interactionsManager;
    public CharacterInput input;
    public Controls _control;

    public GameObject tabletVisual;
    public Transform cameraTransform;
    public float appearDistance = 0.5f;
    public float verticalOffset = -0.2f;
    public Vector3 offsetRotation;
    public GameObject weaponUI;

    public AudioSource openSounds;
    public AudioSource closeSounds;

    [Header("Virtual Cursor")]
    public RectTransform cursorImage;
    public RectTransform canvasRect;
    [SerializeField] private float gamepadCursorSpeed = 500f;

    [Header("UI Input")]
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    private Vector2 virtualCursorPos;

    void Start()
    {
        interactionsManager = FindAnyObjectByType<InteractionsManager>();
        input = FindAnyObjectByType<CharacterInput>();
        _control = new Controls();
    }

    void Update()
    {
        if (!FPSFrameworkCore.IsPaused)
        {
            if (input.TabletInput)
                {
                    if (!isTabletActive)
                        ShowTablet();
                    else
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

            
            if (_control.UI.Pause.triggered)
            {
                HideTablet();
            }
        }

        if (isTabletActive)
        {
            CheckVirtualCursorClick();
            CheckVirtualCursorHover();   // hover 감지
        }
    }

    void LateUpdate()
    {
        if (isTabletActive)
            UpdateVirtualCursor();
    }

    void ShowTablet()
    {
        //Activate ESC Control.
        _control.Enable();

        onTabletShownAction?.Invoke();

        isTabletActive = true;
        tabletVisual.SetActive(true);
        openSounds.Play();
        if (weaponUI != null)
            weaponUI.SetActive(false);

        EnableCursor();
        interactionsManager.isActive = false;

        // 초기 커서 위치 (중앙)
        virtualCursorPos = Vector2.zero;
        cursorImage.localPosition = virtualCursorPos;

        Debug.Log($"Rect size: {canvasRect.rect.size}, lossyScale: {canvasRect.lossyScale}, finalVisible: ({canvasRect.rect.width * canvasRect.lossyScale.x}, {canvasRect.rect.height * canvasRect.lossyScale.y})");
    }

    void HideTablet()
    {
        //Deactivate ESC Control.
        _control.Disable();

        onTabletDisabledAction?.Invoke();

        isTabletActive = false;
        tabletVisual.SetActive(false);
        closeSounds.Play();
        if (weaponUI != null)
            weaponUI.SetActive(true);

        DisableCursor();
        interactionsManager.isActive = true;
    }

    void EnableCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void DisableCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void UpdateVirtualCursor()
    {
        Vector2 inputDelta = Vector2.zero;

        if (Mouse.current != null)
            inputDelta += input.lookInput;

        inputDelta += input.lookInput * gamepadCursorSpeed * Time.unscaledDeltaTime;

        virtualCursorPos += inputDelta;

        Rect rect = canvasRect.rect;
        virtualCursorPos.x = Mathf.Clamp(virtualCursorPos.x, rect.xMin, rect.xMax);
        virtualCursorPos.y = Mathf.Clamp(virtualCursorPos.y, rect.yMin, rect.yMax);

        cursorImage.localPosition = virtualCursorPos;
    }

    void CheckVirtualCursorClick()
    {
        bool isClick = _control.Firearm.Fire.triggered;
            //itemInput.Controls.Firearm.Fire.triggered;
            // (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            // (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame); // A 버튼

            //

        if (!isClick) return;

        // 월드 좌표 → 화면 좌표
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, cursorImage.position);

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = screenPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }

    private GameObject lastHovered;

    void CheckVirtualCursorHover()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, cursorImage.position);

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = screenPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        GameObject currentHovered = results.Count > 0 ? results[0].gameObject : null;
        if (lastHovered != currentHovered)
        {
            // pointerExit
            if (lastHovered != null)
            {
                ExecuteEvents.Execute(lastHovered, pointerData, ExecuteEvents.pointerExitHandler);
            }

            // pointerEnter
            if (currentHovered != null)
            {
                ExecuteEvents.Execute(currentHovered, pointerData, ExecuteEvents.pointerEnterHandler);
            }

            lastHovered = currentHovered;
        }
    }

}
