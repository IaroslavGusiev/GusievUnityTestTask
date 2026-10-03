using System;
using UnityEngine;
using System.Collections.Generic;

namespace _Bludoku.Scripts.Combo
{
    public sealed class GraceMoveHolder : MonoBehaviour
    {
        [SerializeField] private List<GraceMoveView> views = new();
        [SerializeField, Min(0f)] private float refillStagger = 0.05f;

        private int _allowedMisses;
        private int _activeViewCount;
        private bool _initialized;
        
        private void OnDisable() => 
            StopAnimations();

        public void Initialize(ComboRuleType ruleType, int allowedMisses)
        {
            if (_initialized)
            {
                return;
            }
            
            if (ruleType != ComboRuleType.GraceMoves)
            {
                gameObject.SetActive(false);
                _initialized = true;
                return;
            }

            if (allowedMisses >= views.Count)
            {
                throw new InvalidOperationException($"GraceMoveHolder supports at most {views.Count - 1} grace moves, but {allowedMisses} were requested.");
            }

            _allowedMisses = allowedMisses;
            _activeViewCount = allowedMisses + 1;
            gameObject.SetActive(true);
            
            InitializeViews();
            _initialized = true;
        }

        public void ShowState(ComboState state, bool animate)
        {
            if (_initialized == false)
            {
                return;
            }
            
            animate &= isActiveAndEnabled;
            UpdateViews(state, animate);
        }

        public void StopAnimations()
        {
            foreach (GraceMoveView view in views)
            {
                view.StopAnimations();
            }
        }

        private void InitializeViews()
        {
            for (var i = 0; i < views.Count; i++)
            {
                GraceMoveView view = views[i];

                view.gameObject.SetActive(i < _activeViewCount);
                
                if (i < _activeViewCount)
                {
                    view.Initialize();
                }
            }
        }

        private void UpdateViews(ComboState state, bool animate)
        {
            bool lastChance = state.IsActive && state.ConsecutiveMisses == _allowedMisses;
            var refillIndex = 0;
            
            for (var i = 0; i < _activeViewCount; i++)
            {
                GraceMoveState target = GetMoveState(state, i);

                GraceMoveView view = views[i];

                bool restoring = view.State == GraceMoveState.Used && target == GraceMoveState.Available;
                
                float delay = restoring 
                    ? refillIndex++ * refillStagger
                    : 0f;
                
                view.ShowState(target, animate, lastChance && i == _activeViewCount - 1, delay);
            }
        }

        private static GraceMoveState GetMoveState(ComboState state, int index)
        {
            if (state.IsActive == false)
            {
                return GraceMoveState.Empty;
            }
            
            return index < state.ConsecutiveMisses 
                ? GraceMoveState.Used 
                : GraceMoveState.Available;
        }
    }
}
