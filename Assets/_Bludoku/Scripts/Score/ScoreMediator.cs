using _Bludoku.Scripts.Boards;
using UnityEngine;

namespace _Bludoku.Scripts.Score
{
    public class ScoreMediator : MonoBehaviour
    {
        [SerializeField] private ScoreView scoreView;
        [SerializeField] private Board board;
        [SerializeField] private ScoreBoosterView boosterView;
        
        private readonly ScoreBoostSystem _scoreBoostSystem = new();

        private void Awake()
        {
            board.OnFigurePlaced += FigurePlaced;
        }

        private void Start()
        {
            ScoreSystem.LoadScore();
            boosterView.SetBoosterEnabled(ScoreSystem.IsBoosterEnabled);
            _scoreBoostSystem.IsBoosted = ScoreSystem.IsBoosterEnabled;
            scoreView.UpdateScore(false);
        }

        public void ResetScore()
        {
            ScoreSystem.ResetScore();
            UpdateView();
        }

        private void FigurePlaced(ClearResult result)
        {
            _scoreBoostSystem.FigurePlaced(result.ClearedCount);
            boosterView.SetBoosterEnabled(_scoreBoostSystem.IsBoosted);
            ScoreSystem.SetBoosterEnabled(_scoreBoostSystem.IsBoosted);
            ScoreSystem.AddSetScore(result.ClearedCount);
            scoreView.UpdateScore();
        }

        private void UpdateView()
        {
            boosterView.SetBoosterEnabled(false);
            _scoreBoostSystem.IsBoosted = false;
            scoreView.UpdateScore(false);
        }
    }
}