using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Infrastructure.SaveLoad;
using _Scripts._Infrastructure.Constants;
using _Scripts.Game.Progress._Scripts.Data;

namespace _Scripts.Game.Progress
{
    public class ProgressManager : ISavable
    {
        private Dictionary<int, int> _levelStars = new Dictionary<int, int>();
        private HashSet<int> _unlockedLevels = new HashSet<int>();

        public string SavePath => SavePathConstants.ProgressDataPath;
        public Type DataType => typeof(ProgressData);

        public int GetStarsForLevel(int levelIndex)
        {
            return _levelStars.TryGetValue(levelIndex, out int stars) ? stars : 0;
        }

        public bool IsLevelUnlocked(int levelIndex)
        {
            return _unlockedLevels.Contains(levelIndex);
        }

        public void SetStarsForLevel(int levelIndex, int stars)
        {
            _levelStars[levelIndex] = stars;
        }

        public void UnlockLevel(int levelIndex)
        {
            _unlockedLevels.Add(levelIndex);
        }

        public object GetData()
        {
            return new ProgressData(_levelStars, new List<int>(_unlockedLevels));
        }

        public void SetData(object data)
        {
            if (data is not ProgressData progressData)
                return;

            _levelStars = new Dictionary<int, int>(progressData.LevelStars);
            _unlockedLevels = new HashSet<int>(progressData.UnlockedLevels);
        }

        public void SetInitialData()
        {
            _levelStars.Clear();
            _unlockedLevels.Clear();
            _unlockedLevels.Add(0);
        }

        public int GetLastUnlockLevelID()
        {
            return _unlockedLevels.Last();
        }
    }
}