using System;
using System.Collections.Generic;
using _Bludoku.Scripts.Score;
using TMPro;
using UnityEngine;

namespace _Bludoku.Scripts.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class BestScoreText : MonoBehaviour
    {
        private TMP_Text _text;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            ScoreSystem.LoadScore();
            _text.text = ScoreSystem.HighScore.ToString();
        }
    }
}
