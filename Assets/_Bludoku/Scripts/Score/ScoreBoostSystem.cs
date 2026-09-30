namespace _Bludoku.Scripts.Score
{
    public class ScoreBoostSystem
    {
        private int _movesCount;
        private int _comboCount;
        
        private const int MovesThreshold = 3;
        private const int BoostCombo = 2;
        
        public bool IsBoosted
        {
            get => _comboCount >= BoostCombo;
            set
            {
                if (value)
                {
                    _comboCount = BoostCombo;
                }
                else
                {
                    _comboCount = 0;
                }
            }
        }

        public void FigurePlaced(int removes)
        {
            if (removes == 0)
            {
                _movesCount++;
            }
            else
            {
                _movesCount = 0;
                _comboCount++;
            }
            
            if (_movesCount >= MovesThreshold)
            {
                _comboCount = 0;
            }
        }
    }
}