using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Bludoku.Scripts.MainMenu.Settings
{
    public class SettingsToggle : MonoBehaviour
    {
        [SerializeField] private Image knob;
        [SerializeField] private Color offColor;
        [SerializeField] private Color onColor;
        [SerializeField] private float offX;
        [SerializeField] private float onX;

        private const float AnimationDuration = 0.2f;

        public void SetState(bool state, bool animate = true)
        {
            if (animate)
            {
                if (state)
                {
                    knob.DOColor(onColor, AnimationDuration);
                    knob.rectTransform.DOAnchorPosX(onX, AnimationDuration);
                }
                else
                {
                    knob.DOColor(offColor, AnimationDuration);
                    knob.rectTransform.DOAnchorPosX(offX, AnimationDuration);
                }
            }
            else
            {
                knob.color = state ? onColor : offColor;
                Vector2 pos = knob.rectTransform.anchoredPosition;
                pos.x = state ? onX : offX;
                knob.rectTransform.anchoredPosition = pos;
            }
        }
    }
}