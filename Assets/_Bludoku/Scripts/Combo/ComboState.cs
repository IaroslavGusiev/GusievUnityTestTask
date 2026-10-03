namespace _Bludoku.Scripts.Combo
{
    public readonly struct ComboState
    {
        public const int ActivationThreshold = 2;

        public int Count { get; }
        public int ConsecutiveMisses { get; }
        public bool IsActive => Count >= ActivationThreshold;

        internal ComboState(int count, int consecutiveMisses)
        {
            Count = count;
            ConsecutiveMisses = consecutiveMisses;
        }
    }
}
