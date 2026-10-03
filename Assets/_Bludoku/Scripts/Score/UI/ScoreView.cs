using TMPro;
using DG.Tweening;
using UnityEngine;

namespace _Bludoku.Scripts.Score
{
    public class ScoreView : MonoBehaviour
    {
        [Header("--- Texts ---")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text highScoreText;

        [Header("--- Score Animation Parameters ---")]
        [SerializeField, Min(1)] private int punchVibrato = 1;
        [SerializeField, Range(0f, 0.5f)] private float punchScale = 0.15f;
        [SerializeField, Min(0.01f)] private float animationDuration = 0.2f;
        [SerializeField, Range(0f, 1f)] private float punchElasticity = 0.5f;
        
        private int _lastScore;
        private Vector3 _normalScale;
        private Tween _scoreTween;
        private bool _initialized;
        
        private void Awake() => 
            CacheReferences();
        
        private void OnDisable() => 
            StopAnimation();

        private void CacheReferences()
        {
            if (_initialized)
            {
                return;
            }
            
            _normalScale = scoreText.transform.localScale;
            _initialized = true;
        }
        
        public void UpdateScore(bool animate = true)
        {
            CacheReferences();
            
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
            else
            {
                StopAnimation();
            }
        }

        private void AnimateScore()
        {
            StopAnimation();
            
            _scoreTween = scoreText.transform.DOPunchScale(
                _normalScale * punchScale, 
                animationDuration, 
                punchVibrato, 
                punchElasticity);
        }

        private void StopAnimation()
        {
            if (_initialized == false)
            {
                return;
            }
            
            _scoreTween?.Kill();
            _scoreTween = null;
            scoreText.transform.localScale = _normalScale;
        }
    }
}
