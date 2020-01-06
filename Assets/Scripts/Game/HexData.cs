using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HexData{
    public DataTypes.HexType hexType;
    public int health = 1;
    public int points = 50;
    [HideInInspector]
    public Vector2 hexScaleFactor;//Used by the level editor for scaling with grid
    [HideInInspector]
    public Vector3Int cellLocation;//Used by the level editor for Saving and Loading with grid    
}
