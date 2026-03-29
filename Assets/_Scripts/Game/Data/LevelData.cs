using System.Collections.Generic;
using _Scripts.Game.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public int GridWidth;
    public int GridHeight;
    public List<DotData> Dots;

    public float TimeFor3Stars;
    public float TimeFor2Stars;
    public float TimeFor1Star;
}