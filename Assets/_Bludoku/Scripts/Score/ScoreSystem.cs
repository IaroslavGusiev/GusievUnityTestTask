using UnityEngine;

namespace _Bludoku.Scripts.Score
{
    public static class ScoreSystem
    {
        private static int _score;
        private static int _highScore;
        private static bool _isBoosterEnabled;
        
        private const string ScoreKey = "CurrentScore";
        private const string HighScoreKey = "HighScore";
        private const string BoosterKey = "Booster";
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
            _score = PlayerPrefs.GetInt(ScoreKey, 0);
            _highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            _isBoosterEnabled = PlayerPrefs.GetInt(BoosterKey) == 1;
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

        private static void SaveScore()
        {
            PlayerPrefs.SetInt(BoosterKey, IsBoosterEnabled ? 1 : 0);
            PlayerPrefs.SetInt(ScoreKey, Score);
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }
    }
}
