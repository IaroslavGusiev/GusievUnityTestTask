using TMPro;
using UnityEngine;

namespace _Bludoku.Scripts
{
    public class UIElement_Text : MonoBehaviour
    {
        private TextMeshProUGUI selfText;

        private TextMeshProUGUI SelfText
        {
            get
            {
                if (selfText == null)
                    selfText = GetComponent<TextMeshProUGUI>();

                return selfText;
            }
        }

        public void ChangeText(string _stringTxt)
        {
            SelfText.text = _stringTxt;
        }

        public void ChangeText(int _intTxt)
        {
            SelfText.text = _intTxt.ToString();
        }
    }
}
