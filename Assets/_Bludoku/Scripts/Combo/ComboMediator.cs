using System;
using UnityEngine;
using _Bludoku.Scripts.Boards;

namespace _Bludoku.Scripts.Combo
{
    public enum ComboRuleType
    {
        ConsecutiveClears = 0,
        GraceMoves = 1
    }
    
    public class ComboMediator : MonoBehaviour
    {
        public event Action<ComboChange> StateChanged;
        public event Action<ComboMove, ComboState> MoveProcessed;
        public event Action<ClearResult, ComboState> PlacementProcessed;
        
        [SerializeField] private Board board;

        private ComboSystem _system;

        public ComboState State => _system?.State ?? default;

        public void Initialize(ComboRuleType ruleType, int allowedNonClearingMoves = 2)
        {
            IComboRule rule = CreateRule(ruleType, allowedNonClearingMoves);

            _system = new ComboSystem(rule);
            _system.StateChanged += ForwardStateChanged;
            board.OnFigurePlaced += FigurePlaced;
        }

        public void Dispose()
        {
            if (board != null)
            {
                board.OnFigurePlaced -= FigurePlaced;
            }

            if (_system != null)
            {
                _system.StateChanged -= ForwardStateChanged;
            }
        }

        public void ResetCombo() => 
            _system.Reset();

        public void RestoreCombo(int count) => 
            _system.Restore(count);

        private void FigurePlaced(ClearResult result)
        {
            var move = new ComboMove(result.ClearedCount);
            _system.ProcessMove(move);
            ComboState state = _system.State;
            
            MoveProcessed?.Invoke(move, state);
            PlacementProcessed?.Invoke(result, state);
        }

        private static IComboRule CreateRule(ComboRuleType ruleType, int allowedNonClearingMoves)
        {
            return ruleType switch
            {
                ComboRuleType.ConsecutiveClears => new ConsecutiveClearsRule(),
                ComboRuleType.GraceMoves => new GraceMovesRule(allowedNonClearingMoves),
                _ => throw new ArgumentOutOfRangeException(nameof(ruleType))
            };
        }

        private void ForwardStateChanged(ComboChange change) => 
            StateChanged?.Invoke(change);
    }
}
