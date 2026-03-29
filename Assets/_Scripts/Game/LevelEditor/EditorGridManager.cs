#if UNITY_EDITOR

using System.Collections.Generic;
using _Scripts.Game.Data;
using UnityEngine;
using Zenject;

namespace _Scripts.Game.LevelEditor
{
    public class EditorGridManager : MonoBehaviour
    {
        [Header("Prefabs")] 
        [SerializeField] private GameObject _gridCellPrefab;
        [SerializeField] private GameObject _dotPrefab;

        [Header("UI Layout")] 
        [SerializeField] private RectTransform _gridParent;
        [SerializeField] private float cellSize = 100f;

        private GridCell[,] _gridCells;
        private readonly List<Dot> _dots = new();
        private LevelEditorController _editorController;
        [Inject] private DotColorConfig _colorConfig;

        public event System.Action<Dot> OnDotSpawned;

        public void Initialize(LevelEditorController controller)
        {
            _editorController = controller;
        }

        public void CreateGrid(int width, int height)
        {
            ClearGrid();
            _gridCells = new GridCell[width, height];

            var offset = new Vector2(-(width - 1) * cellSize / 2f, (height - 1) * cellSize / 2f);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cellGO = Instantiate(_gridCellPrefab, _gridParent);
                    var cell = cellGO.GetComponent<GridCell>();
                    cell.Initialize(new Vector2Int(x, y), cellSize, offset);
                    _gridCells[x, y] = cell;

                    var clickHandler = cellGO.GetComponent<GridCellClickHandler>() 
                                     ?? cellGO.AddComponent<GridCellClickHandler>();
                    clickHandler.Initialize(cell, this);
                }
            }
        }

        public void PlaceDots(List<DotData> dotsData)
        {
            foreach (var data in dotsData)
            {
                var cell = GetCell(data.Position);
                if (cell == null) continue;

                var dot = Instantiate(_dotPrefab, cell.RectTransform).GetComponent<Dot>();
                dot.InitColorConfig(_colorConfig);
                dot.Setup(data);
                dot.SetCell(cell);
                cell.SetMyDot(dot);
                _dots.Add(dot);

                dot.gameObject.AddComponent<EditorDot>().Initialize(dot, this, _editorController);
            }
        }

        public void SpawnDotAt(GridCell cell)
        {
            var dot = Instantiate(_dotPrefab, cell.RectTransform).GetComponent<Dot>();
            dot.InitColorConfig(_colorConfig);
            dot.Setup(new DotData
            {
                Position = cell.GetGridPosition(),
                ColorType = DotColorType.Red,
                IsStartDot = false,
                IsUniversalDot = false,
                IsObstacle = false
            });

            dot.SetCell(cell);
            cell.SetMyDot(dot);
            _dots.Add(dot);
            OnDotSpawned?.Invoke(dot);

            dot.gameObject.AddComponent<EditorDot>().Initialize(dot, this, _editorController);
            _editorController?.SelectDot(dot);
        }

        public void ClearGrid()
        {
            if (_gridCells != null)
            {
                foreach (var cell in _gridCells)
                {
                    if (cell != null)
                        DestroyImmediate(cell.gameObject);
                }
            }

            foreach (var dot in _dots)
            {
                if (dot != null)
                    DestroyImmediate(dot.gameObject);
            }

            _dots.Clear();
        }

        public GridCell GetCell(Vector2Int pos)
        {
            if (_gridCells == null) return null;

            int width = _gridCells.GetLength(0);
            int height = _gridCells.GetLength(1);

            return (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
                ? _gridCells[pos.x, pos.y]
                : null;
        }

        public int GetWidth() => _gridCells?.GetLength(0) ?? 0;
        public int GetHeight() => _gridCells?.GetLength(1) ?? 0;
        public List<Dot> GetAllDots() => new(_dots);
    }
}

#endif
