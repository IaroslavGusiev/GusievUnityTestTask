using _Bludoku.Scripts.MainMenu.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Bludoku.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private PlayButtonView playButton;
        [SerializeField] private SettingsPanel settingsPanel;

        void Start()
        {
            playButton.SetLevelNumber(SaveSystem.CurrentLevelNumber);
            playButton.SetOnClick(OnPlayClicked);
        }

        public void OnSettingsClicked()
        {
            settingsPanel.Show();
        }

        void OnPlayClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
