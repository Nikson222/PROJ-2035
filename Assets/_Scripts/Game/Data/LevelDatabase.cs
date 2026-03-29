using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Game.Data
{
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "Game/LevelDatabase")]
    public class LevelDatabase : ScriptableObject
    {
        public List<LevelData> Levels;
    
        public LevelData GetLevelData(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= Levels.Count || Levels[levelIndex] == null)
            {
                Debug.Log("Level index out of range: " + levelIndex);
                
                return Levels.Last();
            }
            
            return Levels[levelIndex];
        }
    }
}