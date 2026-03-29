using System.Collections.Generic;
using _Scripts.Game.Data;
using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _gridParent;
        public float cellSize = 100f;

        private GridCell[,] _gridCells;
        private AssetProvider _assetProvider;
        private int _width, _height;

        public GridCell[,] Cells => _gridCells;

        [Inject]
        public void Construct(AssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public int GetWidth() => _width;
        public int GetHeight() => _height;

        public void CreateGrid(int width, int height)
        {
            _width = width;
            _height = height;

            foreach (Transform child in _gridParent)
                DestroyImmediate(child.gameObject);

            _gridCells = new GridCell[width, height];
            var offset = new Vector2(-(width - 1) * cellSize / 2f, (height - 1) * cellSize / 2f);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = _assetProvider.InstantiateGridCell(_gridParent);
                    cell.Initialize(new Vector2Int(x, y), cellSize, offset);
                    _gridCells[x, y] = cell;
                }
            }
        }

        public void PlaceDots(List<DotData> dotsData)
        {
            if (dotsData == null) return;

            foreach (var data in dotsData)
            {
                var cell = _gridCells[data.Position.x, data.Position.y];
                var dot = _assetProvider.InstantiateDot(cell.RectTransform);
                dot.Setup(data);
                dot.SetCell(cell);
                cell.SetMyDot(dot);
            }
        }

        public GridCell GetCell(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height
                ? _gridCells[pos.x, pos.y]
                : null;
        }

        public List<Dot> GetStartDots()
        {
            var startDots = new List<Dot>();

            foreach (var cell in _gridCells)
            {
                if (cell?.MyDot != null && cell.MyDot.IsStartDot)
                    startDots.Add(cell.MyDot);
            }

            return startDots;
        }
    }
}
