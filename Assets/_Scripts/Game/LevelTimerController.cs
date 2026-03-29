using System;
using _Scripts._Infrastructure;
using _Scripts.Game.Data;
using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class LevelTimerController : MonoBehaviour
    {
        [SerializeField] private LevelTimerUI _timerUI;

        private LevelTimer _levelTimer = new();
        private LevelData _levelData;

        [Inject]
        private void Construct(PlayerProfile playerProfile, LevelDatabase levelDatabase)
        {
            _levelData = levelDatabase.GetLevelData(playerProfile.LastChosenLevelIndex);
        }

        private void Start()
        {
            Init(_levelData);
        }

        private void Init(LevelData data)
        {
            _levelData = data;

            _levelTimer.OnTimeChanged += _timerUI.UpdateTime;
            _levelTimer.OnStarChanged += _timerUI.SetStars;
            _levelTimer.OnTimeExpired += HandleTimerExpired;
            
            _levelTimer.StartTimer(data);
            _timerUI.StartTimer();
        }

        private void Update()
        {
            _levelTimer.Tick();
        }

        private void HandleTimerExpired()
        {
            _timerUI.StopTimer();
            Stop();
        }

        public int GetCurrentStars() => _levelTimer.GetCurrentStars();
        public void Stop() => _levelTimer.StopTimer();
    }
}