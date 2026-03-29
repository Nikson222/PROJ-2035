using _Scripts._Infrastructure;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI;
using UnityEngine;
using Zenject;
using _Scripts.Game.Data;
using _Scripts.Game.Progress;

namespace _Scripts.Game
{
    public class LevelProgressChecker
    {
        private GridManager _gridManager;
        private ProgressManager _progressManager;
        private LevelTimerController _levelTimerController;
        private UIPanelService _panelService;
        private AudioService _audioService;
        
        private int _currentLevelIndex;

        [Inject]
        public void Construct(GridManager gridManager, ProgressManager progressManager, LevelDatabase levelDatabase,
            PlayerProfile playerProfile, LevelTimerController levelTimer, UIPanelService panelService, AudioService audioService)
        {
            _gridManager = gridManager;
            _progressManager = progressManager;
            _levelTimerController = levelTimer;
            _panelService = panelService;
            _audioService = audioService;
            
            SetLevelIndex(playerProfile.LastChosenLevelIndex);
        }

        private void SetLevelIndex(int index)
        {
            _currentLevelIndex = index;
        }

        public void TryCompleteLevel()
        {
            foreach (var cell in _gridManager.Cells)
            {
                if (cell.HasDot() && !cell.MyDot.IsStartDot && !cell.MyDot.IsOccupied && !cell.MyDot.IsObstacle)
                    return;
            }

            _levelTimerController.Stop();

            int starsEarned = _levelTimerController.GetCurrentStars();
            _progressManager.SetStarsForLevel(_currentLevelIndex, starsEarned);

            if (starsEarned >= 2)
                _progressManager.UnlockLevel(_currentLevelIndex + 1);

            _audioService.PlaySound(SoundType.Win);
            _panelService.OpenPanel<GameOverPanel>();
        }
    }
}