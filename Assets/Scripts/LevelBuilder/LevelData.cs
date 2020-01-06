using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used to serialize level data to use for reading and writing from json file
[System.Serializable]
public class LevelData
{
    //Not sure what kind of objects we will save here yet
    public int levelId;
    public string levelName;
    public DataTypes.BiomeType biomeType;
    public List<HexData> hexData = new List<HexData>();
    public Vector3 gridCellSize;
    public Vector3 gridCellPosition;
    public float pointMultiplier = 1;
    public bool isBossLevel;
    public BossMovement movementType = BossMovement.None;

    public override string ToString() {
        return "Level: " + biomeType.ToString() + ", " + levelId + ", " + (isBossLevel ? "Boss Level " : "Not Boss Level " + "HexCount: " + hexData.Count);
    }
}

