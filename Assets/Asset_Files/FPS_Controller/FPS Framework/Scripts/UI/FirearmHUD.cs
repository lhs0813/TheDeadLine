using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Weapons/Firearm HUD")]
    public class FirearmHUD : MonoBehaviour
    {
        [Header("Text")]
        public TextMeshProUGUI firearmNameText;
        public TextMeshProUGUI ammoTypeNameText;
        public TextMeshProUGUI remainingAmmoText;
        public TextMeshProUGUI remainingAmmoTypeText;
        public GameObject outOfAmmoAlert;
        public GameObject lowAmmoAlert;

        [Header("Colors")]
        public Color normalColor = Color.white;
        public Color alertColor = Color.red;

        [Header("Image")]
        public Image gunImage;

        public Firearm firearm { get; set; }

        public Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.worldCamera = Camera.main.transform.GetChild(0).gameObject.GetComponent<Camera>();
            _canvas.planeDistance = 100f;
        }
        private void Update()
        {
            if (!firearm)
            {
                return;
            }

            gameObject.SetActive(firearm.isHudActive);

            firearmNameText.SetText(firearm.Name);
            ammoTypeNameText.SetText(firearm.ammoProfile.identifier.displayName);
            remainingAmmoText.SetText(firearm.remainingAmmoCount.ToString());
            remainingAmmoTypeText.SetText(firearm.remainingAmmoTypeCount.ToString());

            outOfAmmoAlert.SetActive(firearm.remainingAmmoCount <= 0);
            lowAmmoAlert.SetActive(firearm.remainingAmmoCount <= firearm.preset.magazineCapacity / 3 && firearm.remainingAmmoCount > 0);

            remainingAmmoText.color = firearm.remainingAmmoCount <= firearm.preset.magazineCapacity / 3 ? alertColor : normalColor;
            remainingAmmoTypeText.color = firearm.remainingAmmoTypeCount <= 0 ? alertColor : normalColor;

            gunImage.sprite = firearm.gunImage;
        }

        private void LateUpdate()
        {
            if(firearm == null)
            {
                Destroy(gameObject);
            }
        }
    }
}