using UnityEngine;
using _Bludoku.Scripts.Combo;
using _Bludoku.Scripts.Analytics;

namespace _Bludoku.Scripts.Score
{
    public class ScoreMediator : MonoBehaviour
    {
        [SerializeField] private ScoreView scoreView;
        [SerializeField] private ComboMediator comboMediator;
        [SerializeField] private ScoreBoosterView boosterView;
        
        private int _highComboThreshold;
        private IAnalyticsProvider _analytics;

        public void Initialize(int highComboThreshold, IAnalyticsProvider analytics)
        {
            _analytics = analytics;
            _highComboThreshold = Mathf.Max(ComboState.ActivationThreshold, highComboThreshold);
            
            boosterView.Initialize();
            
            comboMediator.MoveProcessed += MoveProcessed;
            comboMediator.StateChanged += ComboStateChanged;
        }

        public void Dispose()
        {
            if (comboMediator == null)
            {
                return;
            }
            
            comboMediator.MoveProcessed -= MoveProcessed;
            comboMediator.StateChanged -= ComboStateChanged;
        }

        public void RefreshViews()
        {
            UpdateBooster(comboMediator.State, false);
            scoreView.UpdateScore(false);
        }

        public void ResetScore()
        {
            UpdateBooster(comboMediator.State, false);
            ScoreSystem.ResetScore();
            scoreView.UpdateScore(false);
        }

        private void MoveProcessed(ComboMove move, ComboState state)
        {
            UpdateBooster(state);
            AwardScore(move, state);
            scoreView.UpdateScore();
        }

        private void AwardScore(ComboMove move, ComboState state)
        {
            int previousScore = ScoreSystem.Score;
            int baseScore = ScoreSystem.GetBaseSetScore(move.ClearedCells);

            ScoreSystem.AddSetScore(move.ClearedCells);

            int awardedScore = ScoreSystem.Score - previousScore;
            int bonusScore = awardedScore - baseScore;

            if (bonusScore > 0)
            {
                TrackComboBonus(state, baseScore, bonusScore);
            }
        }

        private void TrackComboBonus(ComboState state, int baseScore, int bonusScore) =>
            _analytics.Track(new AnalyticsEvent(AnalyticsEvents.BonusReceived)
                .Add("bonus_type", "combo")
                .Add("combo_count", state.Count)
                .Add("base_score", baseScore)
                .Add("bonus_score", bonusScore)
                .Add("score", ScoreSystem.Score));

        private void UpdateBooster(ComboState state, bool animate = true)
        {
            ScoreSystem.SetBoosterEnabled(state.IsActive);
            
            boosterView.SetBoosterEnabled(
                state.IsActive, 
                animate: animate, 
                highCombo: state.Count >= _highComboThreshold);
        }

        private void ComboStateChanged(ComboChange change) => 
            UpdateBooster(change.Current, ShouldAnimateBooster(change.Reason));

        private static bool ShouldAnimateBooster(ComboChangeReason reason) =>
            reason != ComboChangeReason.Restored && reason != ComboChangeReason.Reset;
    }
}
