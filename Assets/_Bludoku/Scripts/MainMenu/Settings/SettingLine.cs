using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Bludoku.Scripts.MainMenu.Settings
{
    public class SettingLine : MonoBehaviour
    {
        [SerializeField] private Button clickButton;
        [SerializeField] private SettingsToggle toggle;

        public bool IsOn { get; private set; }
        public event Action<bool> onValueChanged;

        private void Awake()
        {
            if (clickButton != null)
                clickButton.onClick.AddListener(Toggle);
        }

        public void SetState(bool value, bool animate = true)
        {
            IsOn = value;
            toggle.SetState(IsOn,  animate);
        }

        private void Toggle()
        {
            SetState(!IsOn);
            onValueChanged?.Invoke(IsOn);
        }
    }
}