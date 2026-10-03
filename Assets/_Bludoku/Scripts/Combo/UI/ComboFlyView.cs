using TMPro;
using System;
using DG.Tweening;
using UnityEngine;

namespace _Bludoku.Scripts.Combo
{
    public class ComboFlyView : MonoBehaviour
    {
        private const string ComboText = "COMBO <size={1}%>{0}</size>";

        [Header("--- Components ---")]
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private ParticleSystem trail;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rectTransform;

        [Header("--- Appearance ---")]
        [SerializeField, Min(0.01f)] private float popDuration = 0.2f;
        [SerializeField, Min(0.01f)] private float settleDuration = 0.14f;
        [SerializeField, Min(0f)] private float holdDuration = 0.22f;
        [SerializeField, Range(0.1f, 1f)] private float startScale = 0.4f;
        [SerializeField, Min(0f)] private float riseDistance = 30f;
        [SerializeField] private float startTilt = -6f;
        [SerializeField] private float settleTilt = 2f;

        [Header("--- Combo styles ---")]
        [SerializeField] private ComboVisualSettings visualSettings;
        [SerializeField, Range(1f, 1.5f)] private float popScale = 1.18f;
        [SerializeField, Range(1f, 1.5f)] private float highComboPopScale = 1.3f;
        [SerializeField, Range(1f, 1.3f)] private float highComboScale = 1.15f;
        [SerializeField, Range(100, 125)] private int numberSizePercent = 110;

        [Header("--- Flight ---")]
        [SerializeField, Min(0.01f)] private float flyDuration = 0.75f;
        [SerializeField, Range(0f, 1f)] private float fadeStart = 0.75f;
        [SerializeField, Range(0.1f, 1f)] private float arrivalScale = 0.35f;
        
        [Header("--- Bezier handles ---")]
        [SerializeField] private float aDeg = 135f;
        [SerializeField] private float aLength = 0.35f;
        [SerializeField] private float bDeg = 155f;
        [SerializeField] private float bLength = 0.8f;
        
        private Sequence _sequence;
        private Vector3 _normalScale;
        private Color _normalTextColor;
        private Quaternion _normalRotation;
        private ParticleSystem.MinMaxGradient _normalTrailColor;

        public bool IsFlying => _sequence != null && _sequence.IsActive();
        
        private void OnDisable() => 
            Cancel();

        private void OnDestroy() => 
            Cancel();

        public void Initialize()
        {
            _normalTextColor = comboText.color;
            _normalScale = rectTransform.localScale;
            _normalTrailColor = trail.main.startColor;
            _normalRotation = rectTransform.localRotation;
        }

        public void Fly(Vector3 start, Vector3 end, int count, bool highCombo, Action onArrived)
        {
            Cancel();
            ApplyComboStyle(count, highCombo);
            PrepareFlight(start);
            PlayFlightAnimation(start, end, highCombo, onArrived);
        }

        public void Cancel()
        {
            _sequence?.Kill();
            _sequence = null;
            ResetView();
        }

        private void ApplyComboStyle(int count, bool highCombo)
        {
            comboText.SetText(ComboText, count, numberSizePercent);
            comboText.color = ComboVisualSettings.ResolveColor(visualSettings, highCombo);
            
            ParticleSystem.MainModule main = trail.main;
            main.startColor = comboText.color;
        }

        private void PrepareFlight(Vector3 start)
        {
            rectTransform.localPosition = start;
            rectTransform.localScale = _normalScale * startScale;
            rectTransform.localRotation = _normalRotation * Quaternion.Euler(0f, 0f, startTilt);
            
            canvasGroup.alpha = 1f;
            gameObject.SetActive(true);
        }

        private void PlayFlightAnimation(Vector3 start, Vector3 end, bool highCombo, Action onArrived)
        {
            Vector3 launchPosition = start + Vector3.up * riseDistance;
            
            _sequence = DOTween.Sequence();
            AppendAppearance(_sequence, launchPosition, highCombo);
            
            _sequence.AppendInterval(holdDuration);
            AppendTravel(_sequence, launchPosition, end);
            
            _sequence.OnComplete(() => CompleteFlight(onArrived));
        }

        private void AppendAppearance(Sequence sequence, Vector3 launchPosition, bool highCombo)
        {
            float appearDuration = popDuration + settleDuration;
            Vector3 peakScale = _normalScale * (highCombo ? highComboPopScale : popScale);
            Vector3 settledScale = _normalScale * (highCombo ? highComboScale : 1f);

            sequence.Append(rectTransform.DOScale(peakScale, popDuration)
                .SetEase(Ease.OutCubic));
            sequence.Join(rectTransform.DOLocalRotateQuaternion(
                _normalRotation * Quaternion.Euler(0f, 0f, settleTilt), popDuration)
                .SetEase(Ease.OutCubic));
            sequence.Append(rectTransform.DOScale(settledScale, settleDuration)
                .SetEase(Ease.OutSine));
            sequence.Join(rectTransform.DOLocalRotateQuaternion(_normalRotation, settleDuration)
                .SetEase(Ease.OutSine));
            sequence.Insert(0f, rectTransform.DOLocalMove(launchPosition, appearDuration)
                .SetEase(Ease.OutQuad));
        }

        private void AppendTravel(Sequence sequence, Vector3 start, Vector3 end)
        {
            float travelStart = sequence.Duration();
            
            sequence.AppendCallback(() => trail.Play());
            sequence.Append(CreateTravelTween(start, end));
            
            sequence.Join(rectTransform
                .DOScale(_normalScale * arrivalScale, flyDuration)
                .SetEase(Ease.InQuad));
            
            sequence.Insert(travelStart + flyDuration * fadeStart, canvasGroup
                .DOFade(0f, flyDuration * (1f - fadeStart))
                .SetEase(Ease.InQuad));
        }

        private Tween CreateTravelTween(Vector3 start, Vector3 end)
        {
            Vector3 direction = end - start;
            
            float sign = end.x > start.x 
                ? -1f
                : 1f;
            
            Vector3 startHandle = start + Quaternion.AngleAxis(sign * aDeg, Vector3.forward) * direction * aLength;
            Vector3 endHandle = end + Quaternion.AngleAxis(sign * bDeg, Vector3.forward) * direction * bLength;
            
            var progress = 0f;
            
            return DOTween.To(() => progress, value =>
            {
                progress = value;
                rectTransform.localPosition = EvaluateBezierPosition(start, startHandle, endHandle, end, value);
            }, 1f, flyDuration).SetEase(Ease.InQuad);
        }

        private void CompleteFlight(Action onArrived)
        {
            _sequence = null;
            ResetView();
            onArrived?.Invoke();
        }

        private void ResetView()
        {
            ResetTrail();
            
            canvasGroup.alpha = 0f;
            rectTransform.localScale = _normalScale;
            rectTransform.localRotation = _normalRotation;
            comboText.color = _normalTextColor;
            
            gameObject.SetActive(false);
        }

        private void ResetTrail()
        {
            trail.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleSystem.MainModule main = trail.main;
            main.startColor = _normalTrailColor;
        }

        private static Vector3 EvaluateBezierPosition(Vector3 start, Vector3 startHandle, Vector3 endHandle, Vector3 end, float progress)
        {
            float remaining = 1f - progress;
            
            return remaining * remaining * remaining * start + 3f * remaining * remaining * progress * startHandle +
                   3f * remaining * progress * progress * endHandle + progress * progress * progress * end;
        }
    }
}
