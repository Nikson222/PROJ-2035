using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class LineRendererUpdater : MonoBehaviour
    {
        [Inject] private LineSegment.Pool _segmentPool;

        private readonly List<LineSegment> _activeSegments = new();
        
        public void UpdateLine(List<Vector3> positions, Color color)
        {
            foreach (var seg in _activeSegments)
                _segmentPool.Despawn(seg);

            _activeSegments.Clear();

            if (positions.Count < 2) return;

            for (int i = 0; i < positions.Count - 1; i++)
            {
                var segment = _segmentPool.Spawn();
                segment.LineRenderer.startColor = color;
                segment.LineRenderer.endColor = color;
                segment.LineRenderer.material.color = color;
                segment.transform.SetParent(transform, false);
                segment.SetPositions(positions[i], positions[i + 1]);
                _activeSegments.Add(segment);
            }
        }
    }
}