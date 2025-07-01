using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;

namespace Akila.FPSFramework.UI
{
    [ExecuteAlways]
    [AddComponentMenu("Akila/FPS Framework/UI/Carousel Selector")]
    public class CarouselSelector : Selectable
    {
        [Header("Options (plain text)")]
        public List<string> options = new List<string>();
        public int value = 0;

        [Header("UI References")]
        public TextMeshProUGUI label;
        public Button rightButton;
        public Button leftButton;

        [Header("Value Changed Event")]
        public UnityEvent<int> onValueChange;


        [Header("Label Localization")]
        [SerializeField]
        private LocalizeStringEvent localizeLabelEvent;
        public bool LocalizationNeeded;

        protected override void OnEnable()
        {
            base.OnEnable();
            rightButton?.onClick.AddListener(GoRight);
            leftButton?.onClick.AddListener(GoLeft);

        }

        protected override void Start()
        {

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            rightButton?.onClick.RemoveAllListeners();
            leftButton?.onClick.RemoveAllListeners();
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            if (eventData.moveDir == MoveDirection.Left)
            {
                GoLeft();
                eventData.Use();
            }
            else if (eventData.moveDir == MoveDirection.Right)
            {
                GoRight();
                eventData.Use();
            }
        }

        private void GoRight()
        {
            if (options.Count == 0) return;
            value = (value + 1) % options.Count;
            UpdateLabel();
            onValueChange?.Invoke(value);
        }

        private void GoLeft()
        {
            if (options.Count == 0) return;
            value = (value - 1 + options.Count) % options.Count;
            UpdateLabel();
            onValueChange?.Invoke(value);
        }

        private void UpdateLabel()
        {
            if(!LocalizationNeeded)
            {
                Debug.Log($"{options[value]}");
                label.text = options[value];
            }

            if (localizeLabelEvent == null || options.Count == 0) return;

            if (LocalizationNeeded)
            {
                localizeLabelEvent.StringReference.TableEntryReference = options[value];
                Debug.Log($"{options[value]}");
                
                localizeLabelEvent.RefreshString();
            }
        }

        /// <summary>
        /// 옵션 리스트를 새로 설정하고 초기값(0)으로 리셋합니다.
        /// </summary>
        public void AddOptions(string[] newOptions)
        {
            options.Clear();
            options.AddRange(newOptions);
            value = 0;
            UpdateLabel();
            onValueChange?.Invoke(value);
        }

        /// <summary>
        /// 옵션 리스트를 비우고 값도 0으로 리셋합니다.
        /// </summary>
        public void ClearOptions()
        {
            options.Clear();
            value = 0;
            UpdateLabel();
            onValueChange?.Invoke(value);
        }
    }
}
