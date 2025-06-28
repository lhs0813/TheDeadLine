using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Akila.FPSFramework
{
    [CreateAssetMenu(fileName = "New Settings Preset Shared", menuName = "Akila/FPS Framework/Settings System/Settings Preset Shared")]
    public class SettingsPresetShared : SettingsPreset
    {
        public void SetDisplayMode(int value)
        {
            Screen.fullScreen = (value == 0);
        }

        public void SetDisplayResolution(int value)
        {
            List<Resolution> resolutions = FPSFrameworkCore.GetResolutions().ToList();

            Resolution resolution = resolutions[Mathf.Clamp(value, 0, resolutions.Count - 1)];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetFrameLimit(float value)
        {
            int _value = (int)value;

            Application.targetFrameRate = _value;
        }

        public void SetVerticalSync(int value)
        {
            int resultValue = 0;

            //TODO: Improve this method
            if (value == 0) resultValue = 1;
            if (value == 1) resultValue = 0;

            QualitySettings.vSyncCount = resultValue;
        }

        public void SetTextureQuality(int value)
        {
            QualitySettings.globalTextureMipmapLimit = value;
        }

        public void SetTextureFiltering(int value)
        {
            int resultValue = 0;

            //TODO: Improve this method
            if (value == 0) resultValue = 2;
            if (value == 1) resultValue = 1;
            if (value == 2) resultValue = 0;

            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)resultValue;
        }

        public void SetSensitivityMultiplier(float value)
        {
            FPSFrameworkCore.SensitivityMultiplier = value;
        }

        public void SetXSensitivityMultiplier(float value)
        {
            FPSFrameworkCore.XSensitivityMultiplier = value;
        }

        public void SetYSensitivityMultiplier(float value)
        {
            FPSFrameworkCore.YSensitivityMultiplier = value;
        }

        public void SetFieldOfView(float value)
        {
            FPSFrameworkCore.FieldOfView = value;
        }

        public void SetWeaponFieldOfView(float value)
        {
            FPSFrameworkCore.WeaponFieldOfView = value;
        }

        /// <summary>
        /// 드롭다운 인덱스로 로케일을 선택합니다.
        /// 0 → 첫 번째 로케일, 1 → 두 번째 로케일, …
        /// </summary>
        public void SetLanguage(int localeIndex)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (localeIndex < 0 || localeIndex >= locales.Count)
            {
                Debug.LogWarning($"Invalid locale index: {localeIndex}");
                return;
            }

            LocalizationSettings.SelectedLocale = locales[localeIndex];
        }

        public void SetState(bool _bool)
        {

        }
    }
}