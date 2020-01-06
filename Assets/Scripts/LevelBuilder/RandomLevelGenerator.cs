using System;
using UnityEngine;

public class RandomLevelGenerator : MonoBehaviour
{
    // Boundaries tl:tr:bl:br -5, 13 : 5, 13 : -5, -5 : 5, -5
    public static int yRange = 12 - (-5);
    public static int xRange = 4 - (-5);

    private static DataTypes.HexType hexType;
    private static int health;
    
    public static float chanceOfSpawn = 0.2f;

    public static LevelData GenerateLevel(DataTypes.BiomeType biome) {
        LevelData levelData = new LevelData() { 
            levelId = -1,
            levelName = "RandomLevel", 
            biomeType = biome ,
            gridCellSize = new Vector3(1.2f, 1, 1), 
            gridCellPosition = Vector3.zero, 
            pointMultiplier = 1,
            isBossLevel = false,
            movementType = BossMovement.None };


        int childCount = (int)(55f / chanceOfSpawn);//UnityEngine.Random.Range(30, 12 * 26);
        Debug.Log("childCount" + childCount);
        for (int i = 0; i < childCount; i++)
        {
            if(UnityEngine.Random.value > chanceOfSpawn || Mathf.FloorToInt((float)i / (float)xRange) - 5 > 13) {
                continue;
            }

            hexType = getBiomeHexType(biome, 0.15f);//getRandomHexType();

            if(DataTypes.IsBiome(hexType)) {
                health = Mathf.FloorToInt(UnityEngine.Random.Range(1, 4));
            } else if(hexType == DataTypes.HexType.Growth) {
                health = 3;
            } else {
                health = 1;
            }

            HexData hexData = new HexData() {
                hexType = hexType,
                health = health,
                points = 50,
                hexScaleFactor = Vector3.one,
                cellLocation = getCellLocation(i)
            };
            levelData.hexData.Add(hexData);
            
        }

        return levelData;
    }

    private static Vector3Int getCellLocation(int i) {
        // Start bottom left and go up
        float num = i;
        int x = Mathf.FloorToInt((num % (float)xRange)) - xRange / 2;
        int y = Mathf.FloorToInt(num / (float)xRange) - 5;
        // print("x, y: " + x + ", " + y);
        return new Vector3Int(x, y, 0);
    }

    private static DataTypes.HexType getRandomHexType() {
        DataTypes.HexType hex = DataTypes.HexType.Default;
        
        for (int i = 0; i < 25; i++)
        {
            if(hex == DataTypes.HexType.Test || hex == DataTypes.HexType.Default) {
                hex = (DataTypes.HexType)Mathf.FloorToInt(UnityEngine.Random.Range(0, Enum.GetValues(typeof(DataTypes.HexType)).Length - 1));
            } else {
                i = 25;
            }
        }

        return hex;
    }

    private static DataTypes.HexType getBiomeHexType(DataTypes.BiomeType biome, float chanceOfEssence)  {
        if(UnityEngine.Random.value <= chanceOfEssence) {
            return DataTypes.getHexEssenceFrom(biome);
        }

        return DataTypes.getHexFrom(biome);
    }


}