using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class LineSegment : MonoBehaviour
    {
        public LineRenderer LineRenderer;

        public void SetPositions(Vector3 start, Vector3 end)
        {
            LineRenderer.positionCount = 2;
            LineRenderer.SetPosition(0, start);
            LineRenderer.SetPosition(1, end);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    
    
        public class Pool : MonoMemoryPool<LineSegment>
        {
            protected override void OnCreated(LineSegment item)
            {
                base.OnCreated(item);
                item.SetActive(false);
            }

            protected override void OnDespawned(LineSegment segment)
            {
                segment.SetActive(false);
            }

            protected override void OnSpawned(LineSegment segment)
            {
                segment.SetActive(true);
            }
        }
    }
}