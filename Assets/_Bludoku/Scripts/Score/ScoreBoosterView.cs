using System;
using DG.Tweening;
using UnityEngine;

namespace _Bludoku.Scripts.Score
{
    public class ScoreBoosterView : MonoBehaviour
    {
        [SerializeField] private Transform booster;
        
        private bool _isBoosterEnabled;
        private Tween _pulseTween;

        private void Awake()
        {
            booster.localScale = Vector3.zero;
        }

        public void SetBoosterEnabled(bool boosterEnabled)
        {
            if (_isBoosterEnabled == boosterEnabled)
                return;

            booster.DOKill();
            _pulseTween?.Kill();
            _pulseTween = null;

            if (boosterEnabled)
            {
                booster.transform.DOScale(Vector3.one, 0.8f)
                    .SetEase(Ease.OutElastic)
                    .OnComplete(StartPulse);
            }
            else
            {
                booster.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
            }

            _isBoosterEnabled = boosterEnabled;
        }

        private void StartPulse()
        {
            if (!_isBoosterEnabled)
                return;

            _pulseTween = booster.transform.DOScale(1.08f, 0.45f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            booster.DOKill();
            _pulseTween?.Kill();
            _pulseTween = null;
        }
    }
}