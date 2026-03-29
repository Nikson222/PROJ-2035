using System.Collections.Generic;
using System.Linq;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using _Scripts.Game.Data;
using UnityEngine.UI;

namespace _Scripts.Game
{
    [RequireComponent(typeof(DragHandler))]
    [RequireComponent(typeof(LineRendererUpdater))]
    [RequireComponent(typeof(DotDetector))]
    public class LineHandleController : MonoBehaviour
    {   
        private Dot _startDot;
        private Dot _currentDot;
        private readonly List<Dot> _connectedDots = new();

        private Image _image;
        private DragHandler _dragHandler;
        private LineRendererUpdater _lineRendererUpdater;
        private DotDetector _dotDetector;
        
        private AudioService _audioService;
        private bool _isDragging;

        private GridManager _gridManager;
        private LevelProgressChecker _levelProgressChecker;
        
        private static readonly List<LineHandleController> _activeHandles = new();
        
        [Inject]
        public void Construct(DotColorConfig colorConfig, GridManager gridManager, LevelProgressChecker levelProgressChecker,
            AudioService audioService)
        {
            _gridManager = gridManager;
            _levelProgressChecker = levelProgressChecker;
            _audioService = audioService;
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
            _dragHandler = GetComponent<DragHandler>();
            _lineRendererUpdater = GetComponent<LineRendererUpdater>();
            _dotDetector = GetComponent<DotDetector>();
        }

        public void InitializeDot(Dot startDot) => _startDot = startDot;

        private void OnEnable() => _activeHandles.Add(this);
        private void OnDisable() => _activeHandles.Remove(this);

        private void Start()
        {
            if (_startDot == null)
                return;

            _image.color = _startDot.Color;
            
            _connectedDots.Clear();
            _connectedDots.Add(_startDot);
            _currentDot = _startDot;
            
            transform.position = _startDot.transform.position;

            UpdateLineRenderer();

            _dragHandler.OnDragBeginEvent += OnDragBegin;
            _dragHandler.OnDraggingEvent += OnDragging;
            _dragHandler.OnDragEndEvent += OnDragEnd;
        }

        private void OnDragBegin(PointerEventData e) => _isDragging = true;

        private void OnDragging(PointerEventData e)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent as RectTransform, e.position, e.pressEventCamera, out var localPoint);

            Vector2 previousPosition = _currentDot.transform.position;
            transform.localPosition = localPoint;
            UpdateLineRenderer();

            if (_connectedDots.Count >= 2)
            {
                Dot prevDot = _connectedDots[^2];
                if (Vector2.Distance(transform.position, prevDot.transform.position) < _dotDetector.detectionRadius)
                {
                    _currentDot.SetFilled(false);
                    _connectedDots.RemoveAt(_connectedDots.Count - 1);
                    _currentDot = prevDot;
                    UpdateLineRenderer();
                    return;
                }
            }

            if (ConnectionConditionsFalse())
                return;
            
            CheckAndConnectIntermediateDot(previousPosition, transform.position);
        }

        private void OnDragEnd(PointerEventData e)
        {
            _isDragging = false;
            transform.position = _currentDot.transform.position;
            UpdateLineRenderer();
            _levelProgressChecker.TryCompleteLevel();
        }

        private bool ConnectionConditionsFalse()
        {
            if (CheckHandleOnOwnSegment())
                return true;
            if (IsLineCrossingForbiddenDot(_currentDot.transform.position, transform.position))
                return true;
            if (IsSelfIntersecting(_currentDot.transform.position, transform.position))
                return true;
            return false;
        }

        private bool IsNotSuitableDot(Dot target)
        {
            if (target == null || _connectedDots.Contains(target))
                return true;
            if (target.IsOccupied)
                return true;
            if (!target.IsUniversal && target.ColorType != _startDot.ColorType)
                return true;
            if (IsIntersectingWithOthers(_currentDot.transform.position, target.transform.position))
                return true;
            return false;
        }

        private void UpdateLineRenderer()
        {
            var points = _connectedDots.Select(d => d.transform.position).ToList();
            if (_isDragging)
                points.Add(transform.position);
            _lineRendererUpdater.UpdateLine(points, _startDot.Color);
        }

        private bool CheckHandleOnOwnSegment()
        {
            Vector2 handlePos = transform.position;
            float threshold = _dotDetector.detectionRadius * (2f / 3f);
            if (_connectedDots.Count < 2)
                return false;

            for (int i = _connectedDots.Count - 2; i >= 0; i--)
            {
                Vector2 segStart = _connectedDots[i].transform.position;
                Vector2 segEnd = _connectedDots[i + 1].transform.position;
                if (IsPointNearLine(handlePos, segStart, segEnd, threshold))
                {
                    float dStart = Vector2.Distance(handlePos, segStart);
                    float dEnd = Vector2.Distance(handlePos, segEnd);
                    int newIndex = (dStart < dEnd) ? i : i + 1;

                    if (newIndex < _connectedDots.Count - 1)
                    {
                        for (int j = _connectedDots.Count - 1; j > newIndex; j--)
                        {
                            _connectedDots[j].SetFilled(false);
                            _connectedDots.RemoveAt(j);
                        }
                        _currentDot = _connectedDots[newIndex];
                        UpdateLineRenderer();
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsIntersectingWithOthers(Vector2 a, Vector2 b)
        {
            foreach (var other in _activeHandles)
            {
                if (other == this || other._startDot.Color == _startDot.Color)
                    continue;

                foreach (var seg in other.GetSegments())
                    if (LineIntersectionUtility.DoLinesIntersect(a, b, seg.start, seg.end))
                        return true;
            }

            return false;
        }

        private List<Segment> GetSegments()
        {
            var segments = new List<Segment>();
            for (int i = 0; i < _connectedDots.Count - 1; i++)
            {
                segments.Add(new Segment(
                    _connectedDots[i].transform.position,
                    _connectedDots[i + 1].transform.position
                ));
            }

            return segments;
        }

        private bool IsLineCrossingForbiddenDot(Vector2 a, Vector2 b)
        {
            float forbiddenRadius = _dotDetector.detectionRadius * 1.25f;

            foreach (var cell in _gridManager.Cells)
            {
                if (!cell.HasDot())
                    continue;

                Dot dot = cell.MyDot;

                if (_connectedDots.Contains(dot))
                    continue;

                if (dot.IsOccupied || dot.IsObstacle || (!dot.IsUniversal && dot.ColorType != _startDot.ColorType))
                {
                    Vector2 center = dot.transform.position;

                    if (DoesLineIntersectCircle(a, b, center, forbiddenRadius))
                        return true;
                }
            }

            return false;
        }

        private bool DoesLineIntersectCircle(Vector2 A, Vector2 B, Vector2 C, float radius)
        {
            Vector2 AB = B - A;
            Vector2 AC = C - A;
            float abLenSq = AB.sqrMagnitude;

            if (abLenSq == 0)
                return Vector2.Distance(A, C) <= radius * 2;

            float t = Mathf.Clamp01(Vector2.Dot(AC, AB) / abLenSq);
            Vector2 closestPoint = A + t * AB;
            float dist = Vector2.Distance(closestPoint, C);

            bool result = dist <= radius * 2;
            return result;
        }

        private void CheckAndConnectIntermediateDot(Vector2 lineStart, Vector2 lineEnd)
        {
            float forbiddenRadius = _dotDetector.detectionRadius * 1.25f;
            float connectRadius = _dotDetector.detectionRadius;

            foreach (var cell in _gridManager.Cells)
            {
                if (!cell.HasDot())
                    continue;

                Dot dot = cell.MyDot;
                if (_connectedDots.Contains(dot))
                    continue;

                bool isForbidden = dot.IsOccupied || dot.IsObstacle || (!dot.IsUniversal && dot.ColorType != _startDot.ColorType);

                if (isForbidden)
                {
                    if (DoesLineIntersectCircle(lineStart, lineEnd, dot.transform.position, forbiddenRadius)) return;

                    continue;
                }

                if (IsPointNearLine(dot.transform.position, lineStart, lineEnd, connectRadius))
                {
                    if (_connectedDots.Count >= 2)
                    {
                        Dot prevDot = _connectedDots[_connectedDots.Count - 2];
                        if (!IsAngleValid(prevDot, _currentDot, dot))
                            continue;
                    }
                    if (IsIntersectingWithOthers(lineStart, dot.transform.position))
                        continue;

                    _connectedDots.Add(dot);
                    
                    _currentDot = dot;
                    _audioService.PlaySound(SoundType.Connect);
                    dot.SetFilled(true, _startDot.Color);
                    UpdateLineRenderer();
                    break;
                }
            }
        }
        
        private bool IsPointNearLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd, float threshold)
        {
            Vector2 line = lineEnd - lineStart;
            Vector2 toPoint = point - lineStart;
            float lineLen = line.magnitude;
            float t = Mathf.Clamp01(Vector2.Dot(toPoint, line.normalized) / lineLen);
            Vector2 closest = lineStart + t * line;
            float dist = Vector2.Distance(point, closest);
            return dist < threshold;
        }

        private bool IsSelfIntersecting(Vector2 a, Vector2 b)
        {
            for (int i = 0; i < _connectedDots.Count - 1; i++)
            {
                Vector2 segStart = _connectedDots[i].transform.position;
                Vector2 segEnd = _connectedDots[i + 1].transform.position;

                if (ApproximatelyEqual(a, segStart) || ApproximatelyEqual(a, segEnd) ||
                    ApproximatelyEqual(b, segStart) || ApproximatelyEqual(b, segEnd))
                    continue;

                if (LineIntersectionUtility.DoLinesIntersect(a, b, segStart, segEnd))
                    return true;
            }

            return false;
        }

        private bool ApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.01f) =>
            Vector2.Distance(a, b) < tolerance;

        private bool IsAngleValid(Dot previousDot, Dot currentDot, Dot newDot)
        {
            Vector2 dir1 = (currentDot.transform.position - previousDot.transform.position).normalized;
            Vector2 dir2 = (newDot.transform.position - currentDot.transform.position).normalized;
            float angle = Vector2.Angle(dir1, dir2);
            return angle <= 130f;
        }

        private void OnDestroy()
        {
            _dragHandler.OnDragBeginEvent -= OnDragBegin;
            _dragHandler.OnDraggingEvent -= OnDragging;
            _dragHandler.OnDragEndEvent -= OnDragEnd;
        }

        public class Pool : MemoryPool<LineHandleController>
        {
            protected override void OnSpawned(LineHandleController item) => item.gameObject.SetActive(true);
            protected override void OnDespawned(LineHandleController item) => item.gameObject.SetActive(false);
        }
    }
}
