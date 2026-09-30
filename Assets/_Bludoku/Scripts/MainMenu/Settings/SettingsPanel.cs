using _Bludoku.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Bludoku.Scripts.MainMenu.Settings
{
    public class SettingsPanel : Panel
    {
        [SerializeField] private SettingLine soundLine;
        [SerializeField] private SettingLine vibrationLine;
        [SerializeField] private SettingLine nightModeLine;
        [SerializeField] private Button closeButton;

        protected override void Start()
        {
            base.Start();
            
            if (closeButton)
                closeButton.onClick.AddListener(Hide);

            if (soundLine)
                soundLine.onValueChanged += SettingsManager.SetSound;

            if (vibrationLine) 
                vibrationLine.onValueChanged += SettingsManager.SetVibration;
        }

        public override void Show()
        {
            base.Show();
            
            RefreshUI();
        }

        void RefreshUI()
        {
            if (soundLine)
                soundLine.SetState(SettingsManager.IsSoundEnabled, false);
            
            if (vibrationLine)
                vibrationLine.SetState(SettingsManager.IsVibrationEnabled, false);
            
            if (nightModeLine)
                nightModeLine.SetState(SettingsManager.IsNightModeEnabled, false);
        }
    }
}