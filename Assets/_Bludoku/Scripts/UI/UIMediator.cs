using UnityEngine;

namespace _Bludoku.Scripts.UI
{
    public class UIMediator : MonoBehaviour
    {
        [SerializeField] private GameOver gameOverPanel;
        [SerializeField] private RectTransform guiPanel;

        public void ShowGameOver()
        {
            gameOverPanel.Show();
            guiPanel.gameObject.SetActive(false);
        }

        public void HideGameOver()
        {
            gameOverPanel.Hide();
            guiPanel.gameObject.SetActive(true);
        }
    }
}