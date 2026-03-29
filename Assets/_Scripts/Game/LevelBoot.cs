using _Scripts._Infrastructure;
using _Scripts.Game.Data;
using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class LevelBoot : MonoBehaviour
    {
        [SerializeField] private LineHandleSpawner _lineHandleSpawner;

        private GridManager _gridManager;
        private LevelDatabase _levelDatabase;
        private PlayerProfile _playerProfile;

        [Inject]
        public void Construct(GridManager gridManager, PlayerProfile playerProfile, LevelDatabase levelDatabase)
        {
            _gridManager = gridManager;
            _levelDatabase = levelDatabase;
            _playerProfile = playerProfile;
        }

        public void Start()
        {
            var levelData = _levelDatabase.GetLevelData(_playerProfile.LastChosenLevelIndex);

            _gridManager.CreateGrid(levelData.GridWidth, levelData.GridHeight);
            _gridManager.PlaceDots(levelData.Dots);
            _lineHandleSpawner.SpawnHandles();
        }
    }
}