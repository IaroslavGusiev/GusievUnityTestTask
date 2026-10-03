using UnityEngine;
using UnityEngine.SceneManagement;
using _Bludoku.Scripts.MainMenu.Settings;

namespace _Bludoku.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private PlayButtonView playButton;
        [SerializeField] private SettingsPanel settingsPanel;

        private void Start()
        {
            playButton.SetLevelNumber(SaveSystem.CurrentLevelNumber);
            playButton.SetOnClick(OnPlayClicked);
        }


        private void OnPlayClicked() => 
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
