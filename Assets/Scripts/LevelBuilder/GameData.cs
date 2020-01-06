using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that will load all game data, including array of level data
[System.Serializable]
public class GameData
{
    public List<LevelData> allLevelData = new List<LevelData>();
}
