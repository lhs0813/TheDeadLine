using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RetroShadersPro.URP;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;

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

        public void SetVolume(float value)
        {
            Debug.Log($"[SetVolume] called with value={value}");
            // 입력값을 1~100 사이로 제한
            float v = Mathf.Clamp(value, 0f, 99f);
            // 0~1 범위로 정규화
            AudioListener.volume = v / 100f;
        }
        
        public void SetMusicVolume(float value)
        {
            Debug.Log($"[SetMusicVolume] called with value={value}");

            float t = Mathf.Clamp01(value / 100f);             // 0~1 정규화
            float volumeDb = Mathf.Lerp(-80f, 0f, t);           // -80dB ~ 0dB 보간

            UnityEngine.AddressableAssets.Addressables
                .LoadAssetAsync<UnityEngine.Audio.AudioMixer>("Mixer")
                .Completed += handle =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    handle.Result.SetFloat("Music_Volume", volumeDb);
                    Debug.Log($"[SetMusicVolume] Music_Volume set to {volumeDb} dB");
                }
                else
                {
                    Debug.LogError("[SetMusicVolume] Failed to load Mixer from Addressables");
                }
            };
        }

        public void SetSFXVolume(float value)
        {
            Debug.Log($"[SetSFXVolume] called with value={value}");

            float t = Mathf.Clamp01(value / 100f);             // 0~1 정규화
            float volumeDb = Mathf.Lerp(-80f, 0f, t);           // -80dB ~ 0dB 보간

            UnityEngine.AddressableAssets.Addressables
                .LoadAssetAsync<UnityEngine.Audio.AudioMixer>("Mixer")
                .Completed += handle =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    handle.Result.SetFloat("SFX_Volume", volumeDb);
                    Debug.Log($"[SetSFXVolume] SFX_Volume set to {volumeDb} dB");
                }
                else
                {
                    Debug.LogError("[SetSFXVolume] Failed to load Mixer from Addressables");
                }
            };
        }



        public void SetBrightness(float value)
        {
            Debug.Log($"[SetBrightness] called with value={value}");

            float t = Mathf.Clamp01(value / 100f);
            float mapped = Mathf.Lerp(0f, 2f, t);  // 0 ~ 2 보간

            // Addressables 로드 및 적용
            UnityEngine.AddressableAssets.Addressables
                .LoadAssetAsync<VolumeProfile>("Volume_CRT")
                .Completed += handle =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    VolumeProfile profile = handle.Result;

                    if (profile.TryGet(out CRTSettings crt))
                    {
                        crt.brightness.value = mapped;
                        Debug.Log($"[SetBrightness] Brightness set to {mapped}");
                    }
                    else
                    {
                        Debug.LogWarning("[SetBrightness] CRTSettings not found in Volume_CRT");
                    }
                }
                else
                {
                    Debug.LogError("[SetBrightness] Failed to load Volume_CRT from Addressables");
                }
            };
        }

    }
}