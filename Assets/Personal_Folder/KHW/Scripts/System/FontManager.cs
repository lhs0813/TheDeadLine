using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class FontManager : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset _koreanFont;
    [SerializeField] private TMP_FontAsset _englishFont;
    [SerializeField] private TMP_FontAsset _japaneseFont;
    [SerializeField] private TMP_FontAsset _simplifiedChineseFont;
    [SerializeField] private TMP_FontAsset _russianFont;
    [SerializeField] private TMP_FontAsset _brasilFont;

    private Dictionary<string, TMP_FontAsset> _fontMap;
    public static TMP_FontAsset currentFont;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _fontMap = new Dictionary<string, TMP_FontAsset>()
        {
            { "ko",      _koreanFont          },
            { "en",      _englishFont         },
            { "ja-JP",   _japaneseFont        },
            { "zh-Hans", _simplifiedChineseFont },
            { "ru-RU",   _russianFont          },
            { "pt-BR", _brasilFont }
        };

        // Localization 준비 완료 직후와 언어 변경 시 폰트 적용
        LocalizationSettings.InitializationOperation.Completed += _ =>
        {
            ApplyFont(LocalizationSettings.SelectedLocale);
            LocalizationSettings.SelectedLocaleChanged += ApplyFont;
        };

        // 씬 전환 시에도 폰트 재적용
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= ApplyFont;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyFont(LocalizationSettings.SelectedLocale);
    }

    private void ApplyFont(Locale locale)
    {
        if (!_fontMap.TryGetValue(locale.Identifier.Code, out var font))
            return;

        currentFont = font;

        // 모든 TextMeshProUGUI 컴포넌트를 찾아 폰트와 머티리얼을 교체하고
        // 텍스트 강제 재할당 및 메시 업데이트를 수행합니다.
        foreach (var txt in FindObjectsOfType<TextMeshProUGUI>(true))
        {
            txt.font = font;
            txt.fontSharedMaterial = font.material;

            // Force a full re‐render so old SubMeshes are cleared
            txt.text = txt.text;
            txt.ForceMeshUpdate(true, true);
        }
    }
}
