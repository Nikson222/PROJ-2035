using UnityEngine;
using Zenject;
using _Scripts.Game.Data;

namespace _Scripts.Game
{
    public class DotDetector : MonoBehaviour
    {
        public float detectionRadius = 100f;
        private GridManager _gridManager;

        [Inject]
        private void Construct(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        public Dot DetectDot(Vector2 worldPosition, DotColorType colorType)
        {
            if (_gridManager?.Cells == null) return null;

            Dot found = null;
            float closest = float.MaxValue;

            foreach (var cell in _gridManager.Cells)
            {
                if (!cell.HasDot()) continue;

                var dot = cell.GetMyDot();
                float dist = Vector2.Distance(dot.RectTransform.position, worldPosition);

                if (dist <= detectionRadius && dist < closest && (!dot.IsOccupied || dot.IsUniversal))
                {
                    if (dot.IsUniversal || dot.ColorType == colorType)
                    {
                        found = dot;
                        closest = dist;
                    }
                }
            }

            return found;
        }
    }
}