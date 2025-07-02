using Akila.FPSFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;

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
    public GameObject interactionHud;

    private Animator _tabletAnimator;

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
        _tabletAnimator = tabletVisual.GetComponent<Animator>();
        
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
        {
            interactionHud.SetActive(false);
            weaponUI.SetActive(false);
        }



        interactionsManager.isActive = false;

        // 초기 커서 위치 (중앙)
        virtualCursorPos = Vector2.zero;
        cursorImage.localPosition = virtualCursorPos;
        EnableCursor();

        Debug.Log($"Rect size: {canvasRect.rect.size}, lossyScale: {canvasRect.lossyScale}, finalVisible: ({canvasRect.rect.width * canvasRect.lossyScale.x}, {canvasRect.rect.height * canvasRect.lossyScale.y})");
    }

    void HideTablet()
    {
        _tabletAnimator.SetTrigger("Off");

        StartCoroutine(delayHide());
    }

    private IEnumerator delayHide()
    {
        yield return new WaitForSeconds(0.2f);

        //Deactivate ESC Control.
        _control.Disable();

        onTabletDisabledAction?.Invoke();

        isTabletActive = false;
        tabletVisual.SetActive(false);
        closeSounds.Play();
        if (weaponUI != null)
        {
            weaponUI.SetActive(true);
            interactionHud.SetActive(true);
        }



        interactionsManager.isActive = true;
        DisableCursor();
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
        Vector2 mouseDelta = Vector2.zero;
        if (Mouse.current != null)
            // 마우스는 픽셀 단위 델타를 그대로 사용
            mouseDelta = Mouse.current.delta.ReadValue();

        Vector2 stickDelta = Vector2.zero;
        if (Gamepad.current != null)
        {
            // 스틱은 프레임당 속도 * dt 로 보정
            stickDelta = Gamepad.current.rightStick.ReadValue()
                       * gamepadCursorSpeed
                       * Time.unscaledDeltaTime;
        }

        // 마우스가 움직였으면 그걸 우선, 아니면 스틱
        Vector2 inputDelta = mouseDelta.sqrMagnitude > 0.0f
                           ? mouseDelta
                           : stickDelta;

        virtualCursorPos += inputDelta;

        // 캔버스 범위 안으로 클램프
        Rect rect = canvasRect.rect;
        virtualCursorPos.x = Mathf.Clamp(virtualCursorPos.x, rect.xMin, rect.xMax);
        virtualCursorPos.y = Mathf.Clamp(virtualCursorPos.y, rect.yMin, rect.yMax);

        cursorImage.localPosition = virtualCursorPos;
    }

    void CheckVirtualCursorClick()
    {
        bool isFire  = _control.Firearm.Fire.triggered;
        bool isAim   = _control.Firearm.Aim.triggered;

        // if neither just pressed, bail
        if (!isFire && !isAim) return;

        // world → screen
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            Camera.main, 
            cursorImage.position
        );

        var pointerData = new PointerEventData(eventSystem)
        {
            position = screenPos,
            // choose right button for Aim, left for Fire
            button   = isAim 
                    ? PointerEventData.InputButton.Right 
                    : PointerEventData.InputButton.Left,
            clickCount = 1,
            eligibleForClick = true,
            clickTime = Time.unscaledTime
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            // this will fire IPointerClickHandler.OnPointerClick(evt)
            ExecuteEvents.Execute(
                result.gameObject, 
                pointerData, 
                ExecuteEvents.pointerClickHandler
            );
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
