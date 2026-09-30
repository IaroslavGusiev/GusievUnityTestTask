using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Bludoku.Scripts.Score
{
    public class ScoreView  : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Transform scoreTransform;
        [SerializeField] private TMP_Text highScoreText;
        
        private int _lastScore;
        
        private const float AnimationDuration = 0.2f;
        
        public void UpdateScore(bool animate = true)
        {
            if (_lastScore == ScoreSystem.Score)
            {
                animate = false;
            }
            
            _lastScore = ScoreSystem.Score;
            scoreText.text = ScoreSystem.Score.ToString();
            highScoreText.text = ScoreSystem.HighScore.ToString();
            
            if (animate)
            {
                AnimateScore();
            }
        }

        private void AnimateScore()
        {
            scoreText.transform.DOPunchScale(Vector3.one * 0.5f, AnimationDuration, 1, 0.5f);
        }
    }
}