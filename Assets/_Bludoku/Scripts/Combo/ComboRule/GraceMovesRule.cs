using System;

namespace _Bludoku.Scripts.Combo
{
    public sealed class GraceMovesRule : IComboRule
    {
        private readonly int _allowedNonClearingMoves;

        public GraceMovesRule(int allowedNonClearingMoves = 2)
        {
            if (allowedNonClearingMoves < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(allowedNonClearingMoves));
            }

            _allowedNonClearingMoves = allowedNonClearingMoves;
        }

        public ComboDecision Evaluate(ComboMove move, ComboState state)
        {
            if (move.HasClear)
            {
                return ComboDecision.Advance;
            }

            return state.Count > 0 && state.ConsecutiveMisses < _allowedNonClearingMoves
                ? ComboDecision.Hold
                : ComboDecision.Break;
        }
    }
}
