using UnityEngine;

namespace _Bludoku.Scripts.MainMenu
{
    public static class SettingsManager
    {
        private const string KEY_SOUND = "Settings_Sound";
        private const string KEY_MUSIC = "Settings_Music";
        private const string KEY_NIGHTMODE = "Settings_NightMode";
        private const string KEY_VIBRATION = "Settings_Vibration";

        public static bool IsSoundEnabled { get; private set; }
        public static bool IsMusicEnabled { get; private set; }
        public static bool IsNightModeEnabled { get; private set; }
        public static bool IsVibrationEnabled { get; private set; }

        static SettingsManager()
        {
            IsSoundEnabled = PlayerPrefs.GetInt(KEY_SOUND, 1) == 1;
            IsMusicEnabled = PlayerPrefs.GetInt(KEY_MUSIC, 1) == 1;
            IsNightModeEnabled = PlayerPrefs.GetInt(KEY_NIGHTMODE, 1) == 1;
            IsVibrationEnabled = PlayerPrefs.GetInt(KEY_VIBRATION, 1) == 1;
            ApplySettings();
        }

        public static void SetSound(bool enabled)
        {
            IsSoundEnabled = enabled;
            PlayerPrefs.SetInt(KEY_SOUND, enabled ? 1 : 0);
            PlayerPrefs.Save();
            ApplySettings();
        }

        public static void SetMusic(bool enabled)
        {
            IsMusicEnabled = enabled;
            PlayerPrefs.SetInt(KEY_MUSIC, enabled ? 1 : 0);
            PlayerPrefs.Save();
            ApplySettings();
        }

        public static void SetNightMode(bool enabled)
        {
            IsNightModeEnabled = enabled;
            PlayerPrefs.SetInt(KEY_NIGHTMODE, enabled ? 1 : 0);
            PlayerPrefs.Save();
            ApplySettings();
        }

        public static void SetVibration(bool enabled)
        {
            IsVibrationEnabled = enabled;
            PlayerPrefs.SetInt(KEY_VIBRATION, enabled ? 1 : 0);
            PlayerPrefs.Save();
            ApplySettings();
        }

        static void ApplySettings()
        {
            AudioListener.volume = IsSoundEnabled ? 1f : 0f;
            
        }
    }
}