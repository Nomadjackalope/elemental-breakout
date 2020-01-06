using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Convos : MonoBehaviour {

    public static string getHintFrom(ConvoIds id) {
        switch (id)
        {
            case ConvoIds.Boss:
            return "Congratulations! You beat a boss!";
            case ConvoIds.Final:
            return "Congratulations! You have beat all biomes!. \n\nNow try to beat your high scores.";

            case ConvoIds.Fire:
            return "That was an intense battle. Congratulations on your victory.";
            case ConvoIds.Water:
            return "You didn't let water control you and fought through. Great work.";
            case ConvoIds.Earth:
            return "You certainly avoided danger and smashed your way to success.";
            case ConvoIds.Poison:
            return "It takes determination to succeed in the poisonous caverns. Congratulations.";
            case ConvoIds.Shadow:
            return "This might have been your toughest challenge. I am glad you made it.";
            case ConvoIds.Growth:
            return "The forest can get you down but you figured out its secrets.";

            case ConvoIds.Biomes:
            return "There are so many places to explore! They have different essence which you can use for more skills.";

            case ConvoIds.Reward:
            return "For that accomplishment you have been rewarded 100 ";

            default:
            return "NO HINT AVAILABLE FOR THIS ID";
        }
    }

    public static string getHintStringFromBiome(DataTypes.BiomeType biome) {
        if(biome == DataTypes.BiomeType.Default) {
            return "No hint for biome: " + biome;
        }

        return getHintFrom(getHintIdFromBiome(biome));
    }

    public static ConvoIds getHintIdFromBiome(DataTypes.BiomeType biome) {
        
        ConvoIds id = ConvoIds.Fire;

        switch (biome)
        {
            case DataTypes.BiomeType.Fire:
                id = ConvoIds.Fire;
                break;
            case DataTypes.BiomeType.Water:
                id = ConvoIds.Water;
                break;
            case DataTypes.BiomeType.Earth:
                id = ConvoIds.Earth;
                break;
            case DataTypes.BiomeType.Poison:
                id = ConvoIds.Poison;
                break;
            case DataTypes.BiomeType.Shadow:
                id = ConvoIds.Shadow;
                break;
            case DataTypes.BiomeType.Growth:
                id = ConvoIds.Growth;
                break;
            default:
                break;
        }

        return id;
    }
}

[JsonConverter(typeof(StringEnumConverter))]
[System.Serializable]
public enum ConvoIds
{
    Boss,
    Final,
    Fire,
    Water,
    Earth,
    Poison,
    Shadow,
    Growth,
    Biomes,
    Reward
}
