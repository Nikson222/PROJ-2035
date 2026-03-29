using System;
using UnityEngine;

namespace _Scripts.Game.Data
{
    [Serializable]
    public class DotData
    {
        public Vector2Int Position;
        public DotColorType ColorType;
        public bool IsStartDot;
        public bool IsUniversalDot;
        public bool IsObstacle;
    }
}