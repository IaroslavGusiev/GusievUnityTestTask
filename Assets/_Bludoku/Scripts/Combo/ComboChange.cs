namespace _Bludoku.Scripts.Combo
{
    public enum ComboChangeReason
    {
        Advanced,
        Held,
        Broken,
        Reset,
        Restored
    }

    public readonly struct ComboChange
    {
        public ComboState Previous { get; }
        public ComboState Current { get; }
        public ComboChangeReason Reason { get; }

        internal ComboChange(ComboState previous, ComboState current, ComboChangeReason reason)
        {
            Previous = previous;
            Current = current;
            Reason = reason;
        }
    }
}
