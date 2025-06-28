using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class InputSchemeTextController : MonoBehaviour
{
    [Header("Sprite Assets per Scheme")]
    public TMP_SpriteAsset keyboardSpriteAsset;
    public TMP_SpriteAsset XboxSpriteAsset;
    public TMP_SpriteAsset PlayStationSpriteAsset;

    private TextMeshProUGUI _label;

    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // 스킴 변경 및 패드 종류 변경 이벤트 구독
        InputSchemeManager.OnSchemeChanged += OnSchemeChanged;
        InputSchemeManager.OnGamepadTypeChanged += OnGamepadTypeChanged;

        // 초기 상태 반영
        UpdateSpriteAsset(InputSchemeManager.CurrentScheme, InputSchemeManager.CurrentGamepadType);
    }

    private void OnDisable()
    {
        InputSchemeManager.OnSchemeChanged -= OnSchemeChanged;
        InputSchemeManager.OnGamepadTypeChanged -= OnGamepadTypeChanged;
    }

    private void OnSchemeChanged(InputSchemeManager.InputScheme scheme)
    {
        if (scheme == InputSchemeManager.InputScheme.Gamepad)
        {
            // Gamepad 모드일 땐 GamepadType 이벤트에서 처리
            return;
        }

        // Keyboard/Mouse 모드
        _label.spriteAsset = keyboardSpriteAsset;
        _label.SetAllDirty();
    }

    private void OnGamepadTypeChanged(InputSchemeManager.GamepadType padType)
    {
        // Gamepad 종류별로 스프라이트 애셋 교체
        switch (padType)
        {
            case InputSchemeManager.GamepadType.PlayStation:
                _label.spriteAsset = PlayStationSpriteAsset;
                break;
            case InputSchemeManager.GamepadType.Xbox:
                _label.spriteAsset = XboxSpriteAsset;
                break;
            default:
                // Other/None: 기본 게임패드 애셋으로 처리
                _label.spriteAsset = XboxSpriteAsset;
                break;
        }
        _label.SetAllDirty();
    }

    private void UpdateSpriteAsset(InputSchemeManager.InputScheme scheme, InputSchemeManager.GamepadType padType)
    {
        if (scheme == InputSchemeManager.InputScheme.Gamepad)
            OnGamepadTypeChanged(padType);
        else
            OnSchemeChanged(scheme);
    }
}
