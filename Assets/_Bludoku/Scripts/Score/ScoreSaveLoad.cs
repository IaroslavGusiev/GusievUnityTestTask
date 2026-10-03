using System;
using _Bludoku.Scripts.Save;

namespace _Bludoku.Scripts.Score
{
    public static class ScoreSaveLoad
    {
        public static ScoreData Load()
        {
            if (JsonSaveStorage.TryLoad(SaveFiles.Score, out ScoreData data) == false ||
                data.score < 0 || data.highScore < data.score)
            {
                return new ScoreData();
            }

            return data;
        }

        public static void Save(int score, int highScore, bool boosterEnabled)
        {
            JsonSaveStorage.Save(SaveFiles.Score, new ScoreData
            {
                score = score,
                highScore = highScore,
                boosterEnabled = boosterEnabled
            });
        }

        [Serializable]
        public class ScoreData
        {
            public int score;
            public int highScore;
            public bool boosterEnabled;
        }
    }
}
