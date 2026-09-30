using System;
using DG.Tweening;
using UnityEngine;

namespace _Bludoku.Scripts.UI
{
    public class Panel : MonoBehaviour
    {
        [SerializeField] private bool showOnStart;
        [SerializeField] private float animationDuration = 0.5f;

        protected virtual void Start()
        {
            transform.localScale = showOnStart ? Vector3.one : Vector3.zero;
        }

        public virtual void Show()
        {
            transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);
        }

        public virtual void Hide()
        {
            transform.DOScale(Vector3.zero, animationDuration);
        }
    }
}