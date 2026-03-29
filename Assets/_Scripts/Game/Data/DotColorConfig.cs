using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game.Data
{
    [CreateAssetMenu(fileName = "ColorConfig", menuName = "Game/ColorConfig")]
    public class DotColorConfig : ScriptableObject
    {
        [System.Serializable]
        public class ColorEntry
        {
            public DotColorType Type;
            public Color Color;
        }

        public List<ColorEntry> Entries;

        public Color GetColor(DotColorType type)
        {
            foreach (var entry in Entries)
                if (entry.Type == type)
                    return entry.Color;

            Debug.LogWarning($"ColorConfig: No color found for {type}, returning white.");
            return Color.white;
        }
    }
}