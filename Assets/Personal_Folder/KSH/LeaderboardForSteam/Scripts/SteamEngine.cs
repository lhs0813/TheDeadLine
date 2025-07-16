using System;
using UnityEngine;
using Steamworks;

namespace LeastSquares
{
    public class SteamEngine : MonoBehaviour
    {
        public static SteamEngine Instance { get; private set; }

        private static bool _initialized;
        private static uint _initializedAppId;
        public uint appId = 480;

        private void Awake()
        {
            // 싱글톤 중복 방지
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Steam 초기화 중복 방지
            if (_initialized && _initializedAppId != appId)
            {
                throw new ArgumentException("SteamEngine은 AppId당 하나만 존재할 수 있습니다.");
            }

            if (_initialized) return;

            try
            {
                SteamClient.Init(appId);
                Debug.Log($"SteamClient 초기화 완료 (AppId: {appId})");
                _initialized = true;
                _initializedAppId = appId;
            }
            catch (Exception e)
            {
                Debug.LogError($"SteamClient 초기화 실패: {e.Message}");
            }
        }

        private void Update()
        {
            if (_initialized)
            {
                SteamClient.RunCallbacks();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                SteamClient.Shutdown();
                Debug.Log("SteamClient 정상 종료");
                _initialized = false;
                _initializedAppId = 0;
                Instance = null;
            }
        }
    }
}
