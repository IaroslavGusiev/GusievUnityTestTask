namespace _Bludoku.Scripts.Combo
{
    public sealed class ConsecutiveClearsRule : IComboRule
    {
        public ComboDecision Evaluate(ComboMove move, ComboState state)
        {
            return move.HasClear 
                ? ComboDecision.Advance 
                : ComboDecision.Break;
        }
    }
}
