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
    public static TabletController Instance { get; private set; }

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
    private GameObject lastHovered;

    public bool isTabletPossible = true;

    void Awake()
    {
        Instance = this;
        // Initialize input actions
        _control = new Controls();
    }

    void OnEnable()
    {
        // Enable necessary action maps
        _control.Player.Enable();
        _control.Firearm.Enable();
    }

    void Start()
    {
        interactionsManager = FindAnyObjectByType<InteractionsManager>();
        input = FindAnyObjectByType<CharacterInput>();
        _tabletAnimator = tabletVisual.GetComponent<Animator>();
    }

    void OnDisable()
    {
        // Clean up action maps to avoid lingering callbacks
        _control.UI.Pause.Disable();
        _control.Firearm.Disable();
        _control.Player.Disable();
        _control.Dispose();
    }

    void Update()
    {
        if (!FPSFrameworkCore.IsPaused)
        {
            if (input.TabletInput)
            {
                if (!isTabletPossible)
                    return;

                if (!isTabletActive)
                    ShowTablet();
                else
                    HideTablet();
            }
        }

        if (isTabletActive)
        {
            // Position and orient tablet in front of camera
            Vector3 targetPos = cameraTransform.position
                + cameraTransform.forward * appearDistance
                + cameraTransform.up * verticalOffset;

            tabletVisual.transform.position = targetPos;
            tabletVisual.transform.rotation = Quaternion.LookRotation(cameraTransform.forward)
                * Quaternion.Euler(offsetRotation);

            // Hide tablet on Pause button
            if (_control.UI.Pause.triggered)
                HideTablet();

            // Handle UI cursor events
            CheckVirtualCursorClick();
            CheckVirtualCursorHover();
        }
    }

    void LateUpdate()
    {
        if (isTabletActive)
            UpdateVirtualCursor();
    }

    void ShowTablet()
    {
        // Activate ESC control
        _control.UI.Pause.Enable();
        onTabletShownAction?.Invoke();

        isTabletActive = true;
        tabletVisual.SetActive(true);
        openSounds.Play();
        if (weaponUI)
        {
            interactionHud.SetActive(false);
            weaponUI.SetActive(false);
        }

        interactionsManager.isActive = false;

        // 초기 커서 위치 (중앙)
        virtualCursorPos = Vector2.zero;
        cursorImage.localPosition = virtualCursorPos;
        EnableCursor();
    }

    void HideTablet()
    {
        _tabletAnimator.SetTrigger("Off");
        StartCoroutine(DelayHide());
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(0.2f);

        // Deactivate ESC control
        _control.UI.Pause.Disable();
        onTabletDisabledAction?.Invoke();

        isTabletActive = false;
        tabletVisual.SetActive(false);
        closeSounds.Play();
        if (weaponUI)
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
        Vector2 mouseDelta = Mouse.current?.delta.ReadValue() ?? Vector2.zero;
        Vector2 stickDelta = Gamepad.current?
            .rightStick.ReadValue() * gamepadCursorSpeed * Time.unscaledDeltaTime
            ?? Vector2.zero;

        Vector2 inputDelta = (mouseDelta.sqrMagnitude > 0f) ? mouseDelta : stickDelta;

        virtualCursorPos += inputDelta;
        Rect rect = canvasRect.rect;
        virtualCursorPos.x = Mathf.Clamp(virtualCursorPos.x, rect.xMin, rect.xMax);
        virtualCursorPos.y = Mathf.Clamp(virtualCursorPos.y, rect.yMin, rect.yMax);
        cursorImage.localPosition = virtualCursorPos;
    }

    void CheckVirtualCursorClick()
    {
        bool isFire = _control.Firearm.Fire.triggered;
        bool isAim = _control.Firearm.Aim.triggered;
        if (!isFire && !isAim) return;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            Camera.main, cursorImage.position);

        var pointerData = new PointerEventData(eventSystem)
        {
            position = screenPos,
            button = isAim ? PointerEventData.InputButton.Right : PointerEventData.InputButton.Left,
            clickCount = 1,
            eligibleForClick = true,
            clickTime = Time.unscaledTime
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        foreach (var result in results)
        {
            ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }

    void CheckVirtualCursorHover()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, cursorImage.position);
        var pointerData = new PointerEventData(eventSystem) { position = screenPos };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        var currentHovered = results.Count > 0 ? results[0].gameObject : null;

        if (lastHovered != currentHovered)
        {
            if (lastHovered != null)
                ExecuteEvents.Execute(lastHovered, pointerData, ExecuteEvents.pointerExitHandler);
            if (currentHovered != null)
                ExecuteEvents.Execute(currentHovered, pointerData, ExecuteEvents.pointerEnterHandler);
            lastHovered = currentHovered;
        }
    }

    public void EnableTabletControl()
    {
        
    }
}
