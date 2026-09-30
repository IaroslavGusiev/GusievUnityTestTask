using _Bludoku.Scripts.Score;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Bludoku.Scripts.UI
{
    public class GameOver : Panel
    {
        [SerializeField] private Button secondChanceButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private TMP_Text scoreText;

        protected override void Start()
        {
            base.Start();
            
            secondChanceButton.onClick.AddListener(GameController.Instance.SecondChance);
            newGameButton.onClick.AddListener(GameController.Instance.NewGame);
        }

        public override void Show()
        {
            base.Show();
            
            scoreText.text = ScoreSystem.Score.ToString();
        }
    }
}