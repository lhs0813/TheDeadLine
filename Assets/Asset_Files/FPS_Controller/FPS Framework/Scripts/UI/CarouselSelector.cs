using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Akila.FPSFramework.UI
{
    [ExecuteAlways]
    [AddComponentMenu("Akila/FPS Framework/UI/Carousel Selector")]
    public class CarouselSelector : Selectable
    {
        public List<string> options = new List<string>();
        public int value = 0;

        public TextMeshProUGUI label;
        public Button rightButton;
        public Button leftButton;
        public UnityEvent<int> onValueChange;


        protected override void OnEnable()
        {
            base.OnEnable();
            rightButton?.onClick.AddListener(GoRight);
            leftButton?.onClick.AddListener(GoLeft);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            rightButton?.onClick.RemoveAllListeners();
            leftButton?.onClick.RemoveAllListeners();
        }

        void UpdateLabel()
        {
            if (label == null)
            {
                Debug.LogError("Label is not set.", gameObject);
                return;
            }
            label.text = options.Count > 0 ? options[value] : "";
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    GoLeft();
                    eventData.Use();  // 이벤트 소비
                    break;
                case MoveDirection.Right:
                    GoRight();
                    eventData.Use();
                    break;
                // MoveDirection.Up/Down은 필요하면 처리
            }

            UpdateLabel();
        }

        private void GoRight()
        {
            if (options.Count == 0) return;
            value = (value + 1) % options.Count;
            onValueChange?.Invoke(value);
            UpdateLabel();
        }

        private void GoLeft()
        {
            if (options.Count == 0) return;
            value = (value - 1 + options.Count) % options.Count;
            onValueChange?.Invoke(value);
            UpdateLabel();
        }

        private void Update()
        {
            // if (value < 0) value = options.Count - 1;
            // if (value > options.Count - 1) value = 0;

            // value = Mathf.Clamp(value, 0, options.Count - 1);

            // if(label == null)
            // {
            //     Debug.LogError("Label is not set.", gameObject);
            // }
            // else
            // {
            //     label.text = options[value];
            // }
        }

        public void AddOptions(string[] options)
        {
            this.options.AddRange(options);
        }

        public void ClearOptions()
        {
            options.Clear();
        }
    }
}
