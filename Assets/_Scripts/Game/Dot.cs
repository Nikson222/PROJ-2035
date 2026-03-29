using _Scripts.Game.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game
{
    public class Dot : MonoBehaviour
    {
        [SerializeField] private Image dotImage;
        [SerializeField] private Image fillImage;
        [SerializeField] private RectTransform _rectTransform;

        [Inject] private DotColorConfig _colorConfig;

        private DotColorType _dotColorType;
        private Color _dotColor;
        private bool _isStartDot;
        private bool _isUniversal;
        private bool _isObstacle;
        private GridCell _myCell;
        private Vector2Int _position;

        public bool IsOccupied { get; private set; }
        public DotColorType ColorType => _dotColorType;
        public Color Color => _dotColor;
        public bool IsUniversal => _isUniversal;
        public bool IsStartDot => _isStartDot;
        public bool IsObstacle => _isObstacle;
        public RectTransform RectTransform => _rectTransform;
        public GridCell Cell => _myCell;

        public void InitColorConfig(DotColorConfig colorConfig) => _colorConfig = colorConfig;
        
        public void SetCell(GridCell cell)
        {
            _myCell = cell;
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        public void Setup(DotData data)
        {
            _dotColorType = data.ColorType;
            _dotColor = _colorConfig.GetColor(_dotColorType);
            _isStartDot = data.IsStartDot;
            _isUniversal = data.IsUniversalDot;
            _isObstacle = data.IsObstacle;
            _position = data.Position;

            dotImage.enabled = true;
            
            dotImage.color = _isUniversal ? _colorConfig.GetColor(DotColorType.Universal) : _dotColor;

            if (fillImage != null)
            {
                fillImage.enabled = _isStartDot;
                fillImage.color = _isUniversal ? _colorConfig.GetColor(DotColorType.Universal) : _dotColor;
                IsOccupied = _isStartDot;
            }

            if (_isObstacle)
            {
                dotImage.enabled = false;
                fillImage.enabled = true;
                fillImage.color = _colorConfig.GetColor(DotColorType.Obstacle);
            }
        }

        public void SetFilled(bool filled, Color? fillColor = null)
        {
            if (fillImage == null) return;

            IsOccupied = filled;
            fillImage.enabled = filled;
            fillImage.color = fillColor ?? _dotColor;
            dotImage.color = fillColor ?? (_isUniversal ? _colorConfig.GetColor(DotColorType.Universal) : _dotColor);
        }

        public DotData GetDotData()
        {
            return new DotData
            {
                Position = _position,
                ColorType = _dotColorType,
                IsStartDot = _isStartDot,
                IsUniversalDot = _isUniversal,
                IsObstacle = _isObstacle
            };
        }

        public void UpdatePosition(Vector2Int newPos)
        {
            _position = newPos;
        }
    }
}
