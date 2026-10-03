namespace _Bludoku.Scripts.Combo
{
    public enum ComboDecision
    {
        Advance,
        Hold,
        Break
    }

    public interface IComboRule
    {
        ComboDecision Evaluate(ComboMove move, ComboState state);
    }
}
