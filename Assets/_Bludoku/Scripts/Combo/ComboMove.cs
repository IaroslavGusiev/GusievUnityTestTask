using System;

namespace _Bludoku.Scripts.Combo
{
    public readonly struct ComboMove
    {
        public int ClearedCells { get; }
        
        public bool HasClear => ClearedCells > 0;

        public ComboMove(int clearedCells)
        {
            if (clearedCells < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clearedCells));
            }

            ClearedCells = clearedCells;
        }
    }
}
