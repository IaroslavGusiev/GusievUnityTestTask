using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _IntegrationPackage.Scripts
{
    public class UI_Options : UI_Menu
    {
        [SerializeField]
        private RectTransform hapticsRound;

        [SerializeField]
        private RectTransform soundRound;

        [SerializeField]
        private Color activatedColor;

        [SerializeField]
        private Color deactivatedColor;

        public override void OpenMenu()
        {
            base.OpenMenu();

            RefreshInstant();
        }


        public void OnClickHaptic()
        {
            ManagerSave.SetBool("haptics", !ManagerSave.GetBool("haptics"));
            RefreshVisualHaptics();
        }

        public void OnClickSound()
        {
            ManagerSave.SetBool("audio", !ManagerSave.GetBool("audio"));
            RefreshVisualSound();
        }

        public void OnClickClose()
        {

        }

        private void RefreshInstant()
        {
            hapticsRound.GetComponent<Image>().color = ManagerSave.GetBool("haptics") ? activatedColor : deactivatedColor;
            soundRound.GetComponent<Image>().color = ManagerSave.GetBool("audio") ? activatedColor : deactivatedColor;
            hapticsRound.anchoredPosition = new Vector2(ManagerSave.GetBool("haptics") ? 48: -48f, hapticsRound.anchoredPosition.y);
            soundRound.anchoredPosition = new Vector2(ManagerSave.GetBool("audio") ? 48 : -48f, soundRound.anchoredPosition.y);
        }

        private void RefreshVisualHaptics()
        {
            StartCoroutine(SwitchRoutine(hapticsRound, !ManagerSave.GetBool("haptics")));
        }

        private void RefreshVisualSound()
        {
            StartCoroutine(SwitchRoutine(soundRound, !ManagerSave.GetBool("audio")));
        }

        private IEnumerator SwitchRoutine(RectTransform targetRect,bool _posLeft)
        {
            targetRect.GetComponent<Image>().color = !_posLeft ? activatedColor : deactivatedColor;

            float _count = 0;
            float _steps = 6f;
            float _initPosX = targetRect.anchoredPosition.x;
            float _endPos = _posLeft ? -48f : 48f;

            while (_count < _steps)
            {
                targetRect.anchoredPosition = new Vector2(Mathf.Lerp(_initPosX, _endPos, _count / _steps), targetRect.anchoredPosition.y);
                yield return new WaitForFixedUpdate();
                _count++;
            }

            targetRect.anchoredPosition = new Vector2(_endPos, targetRect.anchoredPosition.y);
        }
    }
}
