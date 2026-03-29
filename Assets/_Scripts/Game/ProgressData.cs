using System;
using System.Collections.Generic;
using _Scripts.Data;

namespace _Scripts.Game.Progress._Scripts.Data
{
    [Serializable]
    public class ProgressData : SaveData
    {
        public Dictionary<int, int> LevelStars;
        public List<int> UnlockedLevels;

        public ProgressData(Dictionary<int, int> levelStars, List<int> unlockedLevels)
        {
            LevelStars = levelStars;
            UnlockedLevels = unlockedLevels;
        }
    }
}