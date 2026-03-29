using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class LineHandleSpawner : MonoBehaviour
    {
        [SerializeField] private RectTransform _parent;

        private GridManager _gridManager;
        private AssetProvider _assetProvider;

        [Inject]
        public void Construct(GridManager gridManager, LineHandleController.Pool lineHandlePool, AssetProvider assetProvider)
        {
            _gridManager = gridManager;
            _assetProvider = assetProvider;
        }

        public void SpawnHandles()
        {
            var startDots = _gridManager.GetStartDots();

            foreach (var startDot in startDots)
            {
                var handle = _assetProvider.InstantiateLineHandle(_parent);
                
                handle.transform.position = startDot.transform.position;
                handle.InitializeDot(startDot);
            }
        }
    }
}