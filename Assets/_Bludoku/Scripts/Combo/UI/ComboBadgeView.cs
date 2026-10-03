using TMPro;
using DG.Tweening;
using UnityEngine;

namespace _Bludoku.Scripts.Combo
{
    public class ComboBadgeView : MonoBehaviour
    {
        private const string ComboText = "COMBO {0}";

        [Header("--- Components ---")]
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private GraceMoveHolder graceMoves;
        
        [Header("--- Text Animations Parameters ---")]
        [SerializeField, Min(1)] private int textPunchVibrato = 1;
        [SerializeField, Min(0.01f)] private float textPunchDuration = 0.22f;
        [SerializeField, Range(0f, 0.5f)] private float textPunchScale = 0.15f;
        [SerializeField, Range(0f, 1f)] private float textPunchElasticity = 0.5f;
        
        private Tween _textTween;
        private Vector3 _textScale;
        private ComboState _state;
        private bool _initialized;
        
        public RectTransform FlightTarget => comboText.rectTransform;
        
        private void OnDisable() => 
            StopAnimations();
        
        private void OnDestroy() => 
            StopAnimations();

        public void Initialize(ComboRuleType ruleType, int allowedMisses)
        {
            if (_initialized)
            {
                return;
            }
            
            _textScale = comboText.rectTransform.localScale;
            graceMoves.Initialize(ruleType, allowedMisses);
            _initialized = true;
        }

        public void ShowState(ComboState state, bool animate)
        {
            if (_initialized == false)
            {
                return;
            }
            
            animate &= isActiveAndEnabled;
            UpdateText(state, animate);
            graceMoves.ShowState(state, animate);
            _state = state;
        }

        public void StopAnimations()
        {
            if (_initialized == false)
            {
                return;
            }
            
            StopTextAnimation();
            graceMoves.StopAnimations();
        }

        public void PlayArrival()
        {
            if (isActiveAndEnabled && _state.IsActive)
            {
                PunchText();
            }
        }

        private void UpdateText(ComboState state, bool animate)
        {
            if (animate == false || state.IsActive == false)
            {
                StopTextAnimation();
            }
            
            comboText.SetText(ComboText, state.Count);
            
            comboText.alpha = state.IsActive 
                ? 1f 
                : 0f;
            
            if (ShouldPunchText(state, animate))
            {
                PunchText();
            }
        }

        private void PunchText()
        {
            StopTextAnimation();
            
            _textTween = comboText.rectTransform.DOPunchScale(
                _textScale * textPunchScale,
                textPunchDuration, 
                textPunchVibrato, 
                textPunchElasticity);
        }

        private void StopTextAnimation()
        {
            _textTween?.Kill();
            _textTween = null;
            comboText.rectTransform.localScale = _textScale;
        }

        private bool ShouldPunchText(ComboState state, bool animate) =>
            animate && state.IsActive && (_state.IsActive == false || state.Count > _state.Count);
    }
}
