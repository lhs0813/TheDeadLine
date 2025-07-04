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

    private Dictionary<string, TMP_FontAsset> _fontMap;
    public static TMP_FontAsset currentFont;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _fontMap = new Dictionary<string, TMP_FontAsset>()
        {
            { "ko", _koreanFont },
            { "en", _englishFont },
            { "ja-JP", _japaneseFont },
            { "zh-Hans", _simplifiedChineseFont },
        };

        // Localization 초기화 완료 시 + 로케일 변경 시 기존 로직
        LocalizationSettings.InitializationOperation.Completed += op =>
        {
            ApplyFont(LocalizationSettings.SelectedLocale);
            LocalizationSettings.SelectedLocaleChanged += ApplyFont;
        };

        // 씬이 로드될 때마다 호출
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지
        LocalizationSettings.SelectedLocaleChanged -= ApplyFont;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 바뀌면 현재 로케일로 폰트 다시 적용
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyFont(LocalizationSettings.SelectedLocale);
    }

    // 공통 적용 로직
    private void ApplyFont(Locale locale)
    {
        var code = locale.Identifier.Code;
        if (!_fontMap.TryGetValue(code, out var font)) return;

        currentFont = font;

        foreach (var txt in FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
            txt.font = font;
    }
}
