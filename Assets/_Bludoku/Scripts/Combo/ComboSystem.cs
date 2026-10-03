using System;

namespace _Bludoku.Scripts.Combo
{
    public sealed class ComboSystem
    {
        public event Action<ComboChange> StateChanged;

        private readonly IComboRule _rule;

        public ComboState State { get; private set; }

        public ComboSystem(IComboRule rule) => 
            _rule = rule ?? throw new ArgumentNullException(nameof(rule));

        public void ProcessMove(ComboMove move)
        {
            switch (_rule.Evaluate(move, State))
            {
                case ComboDecision.Advance:
                    AdvanceCombo();
                    break;
                
                case ComboDecision.Hold:
                    HoldCombo();
                    break;
                
                case ComboDecision.Break:
                    SetState(default, ComboChangeReason.Broken);
                    break;
                
                default:
                    throw new InvalidOperationException("The combo rule returned an unsupported decision.");
            }
        }

        public void Restore(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            SetState(new ComboState(count, 0), ComboChangeReason.Restored);
        }

        public void Reset() => 
            SetState(default, ComboChangeReason.Reset);

        private void HoldCombo()
        {
            if (State.Count > 0)
            {
                SetState(new ComboState(State.Count, State.ConsecutiveMisses + 1), ComboChangeReason.Held);
            }
        }

        private void SetState(ComboState state, ComboChangeReason reason)
        {
            if (HasSameState(state))
            {
                return;
            }

            ComboState previous = State;
            State = state;
            StateChanged?.Invoke(new ComboChange(previous, state, reason));
        }

        private void AdvanceCombo() =>
            SetState(new ComboState(State.Count + 1, 0), ComboChangeReason.Advanced);

        private bool HasSameState(ComboState state) =>
            State.Count == state.Count && State.ConsecutiveMisses == state.ConsecutiveMisses;
    }
}
