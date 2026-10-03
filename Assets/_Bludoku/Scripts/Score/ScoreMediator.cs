using UnityEngine;
using _Bludoku.Scripts.Combo;

namespace _Bludoku.Scripts.Score
{
    public class ScoreMediator : MonoBehaviour
    {
        [SerializeField] private ScoreView scoreView;
        [SerializeField] private ComboMediator comboMediator;
        [SerializeField] private ScoreBoosterView boosterView;
        
        private int _highComboThreshold;

        public void Initialize(int highComboThreshold)
        {
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
            ScoreSystem.AddSetScore(move.ClearedCells);
            scoreView.UpdateScore();
        }

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
