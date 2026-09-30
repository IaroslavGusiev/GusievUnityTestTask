using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Bludoku.Scripts.MainMenu
{
    public class PlayButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;

        private const string LABEL_PREFIX = "LEVEL ";
        
        public void SetLevelNumber(int number)
        {
            if (label != null)
                label.text = LABEL_PREFIX + number;
        }

        public void SetOnClick(Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action());
        }
    }
}
