using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Game
{
    public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float _dragScale = 1.2f;
        [SerializeField] private float dragScaleDuration = 0.2f;
        [SerializeField] private float _idlePulseScale = 1.2f;
        [SerializeField] private float idlePulseDuration = 1f;

        private Tween _idleTween;
        private Vector3 _originalScale;

        public delegate void DragEventHandler(PointerEventData eventData);
        public event DragEventHandler OnDragBeginEvent;
        public event DragEventHandler OnDraggingEvent;
        public event DragEventHandler OnDragEndEvent;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        private void Start()
        {
            _idleTween = transform
                .DOScale(_idlePulseScale * _originalScale, idlePulseDuration)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _idleTween?.Kill();

            transform
                .DOScale(_dragScale * _originalScale, dragScaleDuration);

            OnDragBeginEvent?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDraggingEvent?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEndEvent?.Invoke(eventData);

            transform
                .DOScale(_originalScale, dragScaleDuration)
                .OnComplete(() =>
                {
                    _idleTween = transform
                        .DOScale(_idlePulseScale * _originalScale, idlePulseDuration)
                        .SetLoops(-1, LoopType.Yoyo);
                });
        }
    }
}