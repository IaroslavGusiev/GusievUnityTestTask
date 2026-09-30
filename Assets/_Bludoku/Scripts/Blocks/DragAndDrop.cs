using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Bludoku.Scripts.Blocks
{
    public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public event Action OnDragStarted;
        public event Action OnDragging;
        public event Action OnDropped;

        [SerializeField] private Transform holdPoint;
        [SerializeField] private float lerpSpeed = 15f;

        private Vector3 _targetPosition;
        private Vector3 _grabOffset;

        private void Start()
        {
            _targetPosition = transform.position;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, lerpSpeed * Time.deltaTime);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Vector3 worldPos = ScreenToWorld(eventData.position);
            _grabOffset = transform.position - holdPoint.position;
            _targetPosition = worldPos + _grabOffset;
            OnDragStarted?.Invoke();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Vector3 worldPos = ScreenToWorld(eventData.position);
            _targetPosition = worldPos + _grabOffset;
            OnDragging?.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            OnDropped?.Invoke();
        }

        public void MoveTo(Vector3 worldPosition)
        {
            _targetPosition = worldPosition;
        }

        private static Vector3 ScreenToWorld(Vector2 screenPosition)
        {
            Vector3 pos = new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z);
            return Camera.main.ScreenToWorldPoint(pos);
        }
    }
}
