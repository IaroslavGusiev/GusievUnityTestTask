using DG.Tweening;
using UnityEngine;

namespace _Bludoku.Scripts.Score
{
    public class ScoreBoosterView : MonoBehaviour
    {
        [Header("--- Components ---")]
        [SerializeField] private Transform booster;
        [SerializeField] private HeartSparklesView sparkles;

        [Header("--- Visibility Parameters ---")]
        [SerializeField] private bool hideWhenInactive = true;

        [Header("--- Activation Parameters ---")]
        [SerializeField, Min(0.01f)] private float activationDuration = 0.8f;
        [SerializeField, Range(0.5f, 1f)] private float activationScale = 0.9f;
        [SerializeField, Min(0.01f)] private float deactivationDuration = 0.2f;

        [Header("--- Heartbeat Parameters ---")]
        [SerializeField, Range(1f, 1.2f)] private float heartbeatScale = 1.08f;
        [SerializeField, Range(1f, 1.2f)] private float highComboHeartbeatScale = 1.12f;
        [SerializeField, Min(0.1f)] private float heartbeatPause = 0.6f;
        [SerializeField, Min(0.01f)] private float firstBeatRiseDuration = 0.1f;
        [SerializeField, Min(0.01f)] private float firstBeatFallDuration = 0.12f;
        [SerializeField, Min(0.01f)] private float secondBeatRiseDuration = 0.08f;
        [SerializeField, Min(0.01f)] private float secondBeatFallDuration = 0.15f;
        [SerializeField, Range(0f, 1f)] private float secondBeatStrength = 0.5f;

        [Header("--- Arrival Parameters ---")]
        [SerializeField, Range(0f, 0.5f)] private float arrivalPunchScale = 0.14f;
        [SerializeField, Range(0f, 0.5f)] private float highComboArrivalPunchScale = 0.2f;
        [SerializeField, Min(0.01f)] private float arrivalDuration = 0.35f;
        [SerializeField, Min(1)] private int arrivalVibrato = 3;
        [SerializeField, Range(0f, 1f)] private float arrivalElasticity = 0.5f;

        private Tween _animation;

        private bool _isHighCombo;
        private bool _initialized;
        private bool _isBoosterEnabled;
        private Vector3 _normalScale;

        private void OnEnable()
        {
            if (_isBoosterEnabled)
            {
                StartHeartbeat();
            }
        }

        private void OnDisable()
        {
            if (_initialized == false)
            {
                return;
            }

            StopAnimation();

            if (sparkles != null)
            {
                sparkles.Clear();
            }

            if (booster != null)
            {
                RestoreIdleScale();
            }
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _normalScale = booster.localScale;
            RestoreIdleAppearance();
            _initialized = true;
        }

        public void SetBoosterEnabled(bool boosterEnabled, bool animate = true, bool highCombo = false)
        {
            highCombo &= boosterEnabled;

            if (HasSameState(boosterEnabled, highCombo))
            {
                return;
            }

            bool wasEnabled = _isBoosterEnabled;
            StopAnimation();

            _isBoosterEnabled = boosterEnabled;
            _isHighCombo = highCombo;

            UpdateAppearance(animate, wasEnabled);
        }

        public void PlayArrival()
        {
            if (CanAnimate() == false)
            {
                return;
            }

            PrepareActiveView();
            sparkles.EmitArrival(_isHighCombo);
            _animation = CreateArrivalAnimation();
        }

        private void UpdateAppearance(bool animate, bool wasEnabled)
        {
            if (isActiveAndEnabled == false)
            {
                RestoreIdleAppearance();
                return;
            }

            if (_isBoosterEnabled == false)
            {
                Deactivate(animate);
            }
            else if (ShouldPlayActivation(animate, wasEnabled))
            {
                PlayActivation();
            }
            else
            {
                StartHeartbeat();
            }
        }

        private void PlayActivation()
        {
            if (hideWhenInactive == false)
            {
                booster.localScale = _normalScale * activationScale;
            }

            _animation = booster
                .DOScale(_normalScale, activationDuration)
                .SetEase(Ease.OutElastic)
                .OnComplete(ResumeHeartbeat);
        }

        private void Deactivate(bool animate)
        {
            sparkles.Clear();
            Vector3 target = GetIdleScale();

            if (animate)
            {
                _animation = booster
                    .DOScale(target, deactivationDuration)
                    .SetEase(Ease.InBack);
            }
            else
            {
                booster.localScale = target;
            }
        }

        private void StartHeartbeat()
        {
            if (CanAnimate() == false)
            {
                return;
            }

            PrepareActiveView();
            _animation = CreateHeartbeatSequence();
        }

        private void PrepareActiveView()
        {
            StopAnimation();
            booster.localScale = _normalScale;
            sparkles.SetVisible(true);
        }

        private Sequence CreateHeartbeatSequence()
        {
            float firstBeat = _isHighCombo
                ? highComboHeartbeatScale
                : heartbeatScale;

            float secondBeat = Mathf.Lerp(1f, firstBeat, secondBeatStrength);

            Sequence sequence = DOTween.Sequence();

            AppendFirstBeat(sequence, firstBeat);
            AppendSecondBeat(sequence, secondBeat);

            sequence
                .AppendInterval(heartbeatPause)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            return sequence;
        }

        private void AppendFirstBeat(Sequence sequence, float strength)
        {
            sequence.Append(booster
                .DOScale(_normalScale * strength, firstBeatRiseDuration)
                .SetEase(Ease.OutCubic));
            sequence.AppendCallback(EmitHeartbeatSparkles);
            sequence.Append(booster
                .DOScale(_normalScale, firstBeatFallDuration)
                .SetEase(Ease.OutSine));
        }

        private void AppendSecondBeat(Sequence sequence, float strength)
        {
            sequence.Append(booster
                .DOScale(_normalScale * strength, secondBeatRiseDuration)
                .SetEase(Ease.OutCubic));
            sequence.Append(booster
                .DOScale(_normalScale, secondBeatFallDuration)
                .SetEase(Ease.OutSine));
        }

        private void EmitHeartbeatSparkles()
        {
            if (CanAnimate())
            {
                sparkles.EmitHeartbeat(_isHighCombo);
            }
        }

        private Tween CreateArrivalAnimation()
        {
            float strength = _isHighCombo
                ? highComboArrivalPunchScale
                : arrivalPunchScale;

            return booster
                .DOPunchScale(_normalScale * strength, arrivalDuration, arrivalVibrato, arrivalElasticity)
                .OnComplete(ResumeHeartbeat);
        }

        private void ResumeHeartbeat()
        {
            _animation = null;
            StartHeartbeat();
        }

        private void StopAnimation()
        {
            _animation?.Kill();
            _animation = null;
        }

        private void RestoreIdleAppearance()
        {
            sparkles.Clear();
            RestoreIdleScale();
        }

        private void RestoreIdleScale()
        {
            booster.localScale = GetIdleScale();
        }

        private Vector3 GetIdleScale() =>
            _isBoosterEnabled || hideWhenInactive == false
                ? _normalScale
                : Vector3.zero;

        private static bool ShouldPlayActivation(bool animate, bool wasEnabled) =>
            animate && wasEnabled == false;

        private bool HasSameState(bool boosterEnabled, bool highCombo) =>
            _isBoosterEnabled == boosterEnabled && _isHighCombo == highCombo;

        private bool CanAnimate() =>
            _isBoosterEnabled && isActiveAndEnabled;
    }
}
