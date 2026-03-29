using UnityEngine;

namespace _Scripts.Game
{
    public partial class GridCell : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private Vector2Int _position;
        private Dot _myDot;

        public RectTransform RectTransform => _rectTransform;
        public Dot MyDot => _myDot;

        private void Awake()
        {
            _rectTransform ??= GetComponent<RectTransform>();
        }

        public void Initialize(Vector2Int position, float cellSize, Vector2 offset)
        {
            _position = position;
            _rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
            _rectTransform.anchoredPosition = offset + new Vector2(position.x * cellSize, -position.y * cellSize);
        }

        public Vector3 GetWorldCenter() => _rectTransform.position;

        public Vector2Int GetGridPosition() => _position;

        public void SetMyDot(Dot dot) => _myDot = dot;

        public void ClearDot() => _myDot = null;

        public bool HasDot() => _myDot != null;

        public Dot GetMyDot() => _myDot;
    }
}