#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Game.LevelEditor
{
    [RequireComponent(typeof(RectTransform))]
    public class EditorDot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private Dot _dot;
        private EditorGridManager _gridManager;
        private Transform _originalParent;
        private LevelEditorController _editorController;

        private float _lastClickTime;
        private const float DoubleClickThreshold = 1f;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
        }

        public void Initialize(Dot dot, EditorGridManager gridManager, LevelEditorController editorController)
        {
            _dot = dot;
            _gridManager = gridManager;
            _editorController = editorController;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalParent = _rectTransform.parent;
            _rectTransform.SetParent(_canvas.transform, true);
            _rectTransform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera,
                    out var localPoint))
            {
                _rectTransform.anchoredPosition = localPoint;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var currentCell = _dot.Cell;
            var targetCell = GetClosestCell();

            if (targetCell == null)
            {
                ResetToOriginalParent();
                return;
            }

            var otherDot = targetCell.GetMyDot();

            if (otherDot == null)
            {
                MoveDot(_dot, currentCell, targetCell);
            }
            else
            {
                MoveDot(_dot, currentCell, targetCell);
                MoveDot(otherDot, targetCell, currentCell);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (eventData.clickCount == 2)
                {
                    _dot.Cell?.ClearDot();
                    _editorController?.DeleteDot(_dot);
                    DestroyImmediate(_dot.gameObject);
                    _editorController?.SelectDot(null);
                }
            }
            
            if (eventData.button != PointerEventData.InputButton.Left) return;

            _editorController?.SelectDot(_dot);


            EventSystem.current.SetSelectedGameObject(null);
        }


        private void ResetToOriginalParent()
        {
            _rectTransform.SetParent(_originalParent, true);
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        private void MoveDot(Dot dot, GridCell from, GridCell to)
        {
            from?.ClearDot();

            var rect = dot.GetComponent<RectTransform>();
            rect.SetParent(to.RectTransform, true);
            rect.anchoredPosition = Vector2.zero;

            dot.SetCell(to);
            dot.UpdatePosition(to.GetGridPosition());
            to.SetMyDot(dot);
        }

        private GridCell GetClosestCell()
        {
            float minDist = float.MaxValue;
            GridCell closest = null;
            var dotPos = _rectTransform.position;

            for (int x = 0; x < _gridManager.GetWidth(); x++)
            {
                for (int y = 0; y < _gridManager.GetHeight(); y++)
                {
                    var cell = _gridManager.GetCell(new Vector2Int(x, y));
                    if (cell == null) continue;

                    float dist = Vector3.Distance(dotPos, cell.GetWorldCenter());
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = cell;
                    }
                }
            }

            return closest;
        }
    }
}

#endif