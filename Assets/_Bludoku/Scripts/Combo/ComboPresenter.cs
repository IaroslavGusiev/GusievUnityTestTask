using System.Linq;
using UnityEngine;
using _Bludoku.Scripts.Score;
using _Bludoku.Scripts.Boards;
using System.Collections.Generic;
using _Bludoku.Scripts.Extentions;

namespace _Bludoku.Scripts.Combo
{
    [DisallowMultipleComponent]
    public sealed class ComboPresenter : MonoBehaviour
    {
        [Header("--- Components ---")]
        [SerializeField] private ComboMediator comboMediator;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera boardCamera;
        [SerializeField] private ComboBadgeView badge;
        [SerializeField] private ComboFlyView flightTemplate;
        [SerializeField] private ScoreBoosterView boosterView;
        [SerializeField] private RectTransform flightContainer;

        [Header("--- FlightPoolSize ---")]
        [SerializeField, Min(1)] private int flightPoolSize = 3;

        private readonly List<ComboFlyView> _comboFlyViews = new();
        private int _highComboThreshold;

        private void OnDisable() =>
            SuspendPresentation();

        public void Initialize(ComboRuleType ruleType, int allowedMisses, int highComboThreshold)
        {
            _highComboThreshold = Mathf.Max(ComboState.ActivationThreshold, highComboThreshold);
            
            badge.Initialize(ruleType, allowedMisses);
            CreateFlightPool();

            comboMediator.StateChanged += StateChanged;
            comboMediator.PlacementProcessed += PlacementProcessed;
            ResumePresentation();
        }

        public void Dispose()
        {
            if (comboMediator != null)
            {
                comboMediator.StateChanged -= StateChanged;
                comboMediator.PlacementProcessed -= PlacementProcessed;
            }

            SuspendPresentation();
        }

        public void SuspendPresentation()
        {
            CancelFlights();

            if (badge != null)
            {
                badge.StopAnimations();
            }
        }

        public void ResumePresentation() =>
            badge.ShowState(comboMediator.State, false);

        private void CreateFlightPool()
        {
            _comboFlyViews.Clear();
            
            for (var i = 0; i < Mathf.Max(1, flightPoolSize); i++)
            {
                ComboFlyView flight = Instantiate(flightTemplate, flightContainer);
                flight.name = "ComboFlyView " + (i + 1);
                flight.Initialize();
                _comboFlyViews.Add(flight);
            }
        }

        private void StateChanged(ComboChange change)
        {
            if (change.Reason is ComboChangeReason.Broken or ComboChangeReason.Reset or ComboChangeReason.Restored)
            {
                CancelFlights();
            }
            
            if (isActiveAndEnabled)
            {
                badge.ShowState(change.Current, ShouldAnimateBadge(change.Reason));
            }
        }

        private void PlacementProcessed(ClearResult result, ComboState state)
        {
            if (isActiveAndEnabled == false || result.ClearedCount == 0 || state.IsActive == false)
            {
                return;
            }
            
            if (TryGetFlightPoints(result.EffectCenter, out Vector2 start, out Vector2 end) == false)
            {
                return;
            }
            
            GetAvailableFlight()
                .Fly(start, end, state.Count, state.Count >= _highComboThreshold, HandleArrival);
        }

        private bool TryGetFlightPoints(Vector3 origin, out Vector2 start, out Vector2 end)
        {
            end = default;
            
            return flightContainer.TryWorldToLocalPoint(origin, boardCamera, canvas, out start) 
                   && flightContainer.TryUIToLocalPoint(badge.FlightTarget, canvas, out end);
        }

        private ComboFlyView GetAvailableFlight()
        {
            ComboFlyView flight = _comboFlyViews.FirstOrDefault(view => view.IsFlying == false) 
                                  ?? _comboFlyViews.First();
            
            _comboFlyViews.Remove(flight);
            _comboFlyViews.Add(flight);
            
            return flight;
        }

        private void HandleArrival()
        {
            if (isActiveAndEnabled == false || comboMediator.State.IsActive == false)
            {
                return;
            }
            
            badge.PlayArrival();
            boosterView.PlayArrival();
        }

        private void CancelFlights()
        {
            foreach (ComboFlyView flight in _comboFlyViews)
            {
                if (flight != null)
                {
                    flight.Cancel();
                }
            }
        }

        private static bool ShouldAnimateBadge(ComboChangeReason reason) =>
            reason is ComboChangeReason.Advanced or ComboChangeReason.Held or ComboChangeReason.Broken;
    }
}
