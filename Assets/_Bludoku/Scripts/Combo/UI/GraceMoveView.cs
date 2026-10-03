using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Bludoku.Scripts.Combo
{
    public enum GraceMoveState { Empty, Available, Used }
    
    public class GraceMoveView : MonoBehaviour
    {
        [Header("--- Components ---")]
        [SerializeField] private Image image;

        [Header("--- Sprites ---")]
        [SerializeField] private Sprite availableSprite;
        [SerializeField] private Sprite usedSprite;
        [SerializeField] private Sprite emptySprite;

        [Header("--- Consume Parameters ---")]
        [SerializeField, Range(0.5f, 1f)] private float consumeScale = 0.85f;
        [SerializeField, Range(1f, 1.3f)] private float consumeBumpScale = 1.15f;
        [SerializeField, Min(0.01f)] private float consumeShrinkDuration = 0.08f;
        [SerializeField, Min(0.01f)] private float consumeBumpDuration = 0.1f;
        [SerializeField, Min(0.01f)] private float consumeSettleDuration = 0.12f;

        [Header("--- Refill Parameters ---")]
        [SerializeField, Range(1f, 1.3f)] private float refillBumpScale = 1.18f;
        [SerializeField, Min(0.01f)] private float refillBumpDuration = 0.12f;
        [SerializeField, Min(0.01f)] private float refillSettleDuration = 0.18f;

        [Header("--- Empty Parameters ---")]
        [SerializeField, Range(0.5f, 1f)] private float emptyScale = 0.92f;
        [SerializeField, Min(0.01f)] private float emptyShrinkDuration = 0.08f;
        [SerializeField, Min(0.01f)] private float emptySettleDuration = 0.1f;

        [Header("--- Last Chance Parameters ---")]
        [SerializeField, Range(1f, 1.3f)] private float warningScale = 1.12f;
        [SerializeField, Min(0.01f)] private float warningDuration = 0.45f;

        private Vector3 _normalScale;
        private Tween _animation;
        
        private bool _isWarning;
        private bool _initialized;

        public GraceMoveState State { get; private set; }
        
        private RectTransform RectTransform => image.rectTransform;

        private void OnDisable() =>
            StopAnimations();

        private void OnDestroy() =>
            StopAnimations();

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _normalScale = RectTransform.localScale;
            _initialized = true;
        }

        public void ShowState(GraceMoveState state, bool animate, bool warning, float refillDelay = 0f)
        {
            if (_initialized == false)
            {
                return;
            }

            animate &= isActiveAndEnabled;
            UpdateState(state, animate, refillDelay);
            UpdateWarning(warning && isActiveAndEnabled);
        }

        public void StopAnimations()
        {
            if (_initialized == false)
            {
                return;
            }

            _animation?.Kill();
            _animation = null;
            _isWarning = false;
            
            ResetAppearance();
        }

        private void UpdateState(GraceMoveState state, bool animate, float refillDelay)
        {
            GraceMoveState previous = State;
            
            if (animate && previous == state)
            {
                return;
            }

            StopAnimations();
            State = state;

            if (ShouldAnimateTransition(previous, animate))
            {
                AnimateTransition(state, refillDelay);
            }
            else
            {
                ApplySprite(state);
            }
        }

        private void AnimateTransition(GraceMoveState state, float refillDelay)
        {
            switch (state)
            {
                case GraceMoveState.Used:
                    AnimateConsumed();
                    break;
                case GraceMoveState.Available:
                    AnimateRefill(refillDelay);
                    break;
                case GraceMoveState.Empty:
                    AnimateEmpty();
                    break;
            }
        }

        private void AnimateConsumed()
        {
            Sequence sequence = CreateTransition();

            sequence.Append(RectTransform
                .DOScale(_normalScale * consumeScale, consumeShrinkDuration)
                .SetEase(Ease.OutQuad));
            sequence.AppendCallback(() => ApplySprite(GraceMoveState.Used));
            sequence.Append(RectTransform
                .DOScale(_normalScale * consumeBumpScale, consumeBumpDuration)
                .SetEase(Ease.OutCubic));
            sequence.Append(RectTransform
                .DOScale(_normalScale, consumeSettleDuration)
                .SetEase(Ease.OutSine));
        }

        private Sequence CreateTransition()
        {
            Sequence sequence = DOTween
                .Sequence()
                .SetEase(Ease.Linear);
            
            _animation = sequence;
            sequence.OnComplete(() => _animation = null);
            return sequence;
        }

        private void AnimateRefill(float delay)
        {
            Sequence sequence = CreateTransition();

            sequence.AppendInterval(delay);
            sequence.AppendCallback(() => ApplySprite(GraceMoveState.Available));
            sequence.Append(RectTransform
                .DOScale(_normalScale * refillBumpScale, refillBumpDuration)
                .SetEase(Ease.OutCubic));
            sequence.Append(RectTransform
                .DOScale(_normalScale, refillSettleDuration)
                .SetEase(Ease.OutSine));
        }

        private void AnimateEmpty()
        {
            Sequence sequence = CreateTransition();

            sequence.Append(RectTransform
                .DOScale(_normalScale * emptyScale, emptyShrinkDuration)
                .SetEase(Ease.OutQuad));
            
            sequence.AppendCallback(() => ApplySprite(GraceMoveState.Empty));
            
            sequence.Append(RectTransform
                .DOScale(_normalScale, emptySettleDuration)
                .SetEase(Ease.OutSine));
        }

        private void UpdateWarning(bool warning)
        {
            if (warning == false || State != GraceMoveState.Available)
            {
                StopWarning();
                return;
            }

            if (IsWarningRunning())
            {
                return;
            }

            StartWarning();
        }

        private void StartWarning()
        {
            StopAnimations();
            
            _isWarning = true;
            
            _animation = RectTransform
                .DOScale(_normalScale * warningScale, warningDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopWarning()
        {
            if (_isWarning)
            {
                StopAnimations();
            }
        }

        private void ApplySprite(GraceMoveState state) => image.sprite = state switch
        {
            GraceMoveState.Available => availableSprite,
            GraceMoveState.Used => usedSprite,
            _ => emptySprite
        };

        private void ResetAppearance()
        {
            RectTransform.localScale = _normalScale;
            ApplySprite(State);
        }

        private bool IsWarningRunning() =>
            _isWarning && _animation != null && _animation.IsActive();

        private static bool ShouldAnimateTransition(GraceMoveState previous, bool animate) =>
            animate && previous != GraceMoveState.Empty;
    }
}
