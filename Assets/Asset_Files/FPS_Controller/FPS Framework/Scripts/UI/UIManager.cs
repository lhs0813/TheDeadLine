using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/UI Mananger")]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public DamageIndicator DamageIndicator { get; set; }
        public HealthDisplay HealthDisplay { get; set; }
        public Hitmarker Hitmarker { get; set; }
        public KillFeed KillFeed { get; set; }

        public Canvas _canvas;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            DamageIndicator = GetComponentInChildren<DamageIndicator>();
            HealthDisplay = GetComponentInChildren<HealthDisplay>();
            Hitmarker = GetComponentInChildren<Hitmarker>();
            KillFeed = GetComponentInChildren<KillFeed>();

            _canvas = GetComponent<Canvas>();
            _canvas.worldCamera = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
            _canvas.planeDistance = 100f;

        }

        /// <summary>
        /// updates player name in player's card
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            HealthDisplay.actorNameText.text = name;
        }

        public void LoadGame(string name)
        {
            LoadingScreen.LoadScene(name);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}