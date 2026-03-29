using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class AssetProvider : MonoBehaviour
    {
        [SerializeField] private GameObject _dotPrefab;
        [SerializeField] private GameObject _gridCellPrefab;

        private DiContainer _container;
        
        private LineHandleController.Pool _lineHandlePool;

        [Inject]
        public void Construct(DiContainer container, LineHandleController.Pool lineHandlePool)
        {
            _container = container;
            _lineHandlePool = lineHandlePool;
        }

        public Dot InstantiateDot(RectTransform dotParent)
        {
            return _container.InstantiatePrefabForComponent<Dot>(_dotPrefab, dotParent);
        }

        public GridCell InstantiateGridCell(RectTransform gridParent)
        {
            return _container.InstantiatePrefabForComponent<GridCell>(_gridCellPrefab, gridParent);
        }

        public LineHandleController InstantiateLineHandle(RectTransform lineParent)
        {
            var handle = _lineHandlePool.Spawn();
            handle.transform.SetParent(lineParent.transform, false);
            
            return handle;
        }
    }
}