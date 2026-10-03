namespace _Bludoku.Scripts.Score
{
    public static class ScoreSystem
    {
        private static int _score;
        private static int _highScore;
        private static bool _isBoosterEnabled;
        
        private const int ScoreForSet = 1;
        private const float BoosterMultiplier = 1.5f;

        public static int Score => _score;
        public static int HighScore => _highScore;
        public static bool IsBoosterEnabled => _isBoosterEnabled;

        public static void SetBoosterEnabled(bool enabled)
        {
            _isBoosterEnabled = enabled;
        }

        public static void LoadScore()
        {
            ScoreSaveLoad.ScoreData data = ScoreSaveLoad.Load();
            _score = data.score;
            _highScore = data.highScore;
            _isBoosterEnabled = data.boosterEnabled;
        }
        
        public static int GetBaseSetScore(int setsCount) =>
            setsCount * ScoreForSet;

        public static void AddSetScore(int setsCount)
        {
            int scoreToAdd = GetBaseSetScore(setsCount);
            scoreToAdd = (int)(scoreToAdd * (IsBoosterEnabled ? BoosterMultiplier : 1));
            
            AddScore(scoreToAdd);
        }
        
        public static void AddScore(int score)
        {
            _score = Score + score;
            if (HighScore < Score)
            {
                _highScore = Score;
            }
            
            SaveScore();
        }

        public static void ResetScore()
        {
            _score = 0;
            SaveScore();
        }

        private static void SaveScore() =>
            ScoreSaveLoad.Save(Score, HighScore, IsBoosterEnabled);
    }
}
