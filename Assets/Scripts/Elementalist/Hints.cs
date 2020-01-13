using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Hints : MonoBehaviour {

    public static string getHintFrom(HintIds id) {
        switch (id)
        {
            case HintIds.BasicExplanation:
            return "Double tap to launch the ball. Drag your finger to move the paddle. Collect the floaty essence. Good luck!";
            case HintIds.SkillTree:
            return "Here you can equip new skills but you have to select at least one biome first.";
            case HintIds.SkillTreeSuggest:
            return "I suggest you start with ";
            case HintIds.Skills:
            return "You can equip up to 3 skills from this biome to this skill set. You'll unlock higher tier slots after purchasing a skill.";
            case HintIds.Boss:
            return "Watch out! Something here is actively working to stop you!";
            case HintIds.ActiveExplanation:
            return "You just purchased an active powerup! To use it, double tap after launching your ball and after the cooldown wears off.";
            case HintIds.Fire:
            return "The red essence blocks on this level seem to have an explosive quality.";
            case HintIds.Earth:
            return "Earth essence blocks can slow your paddle down.";
            case HintIds.Water:
            return "Water blocks can control your ball but with skills you can learn to control them.";
            case HintIds.Poison:
            return "Destroy tough blocks with a poisoned ball.";
            case HintIds.Shadow:
            return "Be careful not to lose lives when all becomes dark.";
            case HintIds.Growth:
            return "Destroy the plants to destroy their root.";
            case HintIds.AutoEnd:
            return "Once you have less than 3 blocks, you'll have 10 seconds to gain more points. A countdown will start when you have 5 seconds left.";
            case HintIds.Coins:
            return "If you don't have enough essence, trade your coins for essence in the shop on the main menu.";
            case HintIds.Random:
            return "You can now play in the wilderness where there are randomly generated maps.";

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

    public static HintIds getHintIdFromBiome(DataTypes.BiomeType biome) {
        
        HintIds id = HintIds.Fire;

        switch (biome)
        {
            case DataTypes.BiomeType.Fire:
                id = HintIds.Fire;
                break;
            case DataTypes.BiomeType.Water:
                id = HintIds.Water;
                break;
            case DataTypes.BiomeType.Earth:
                id = HintIds.Earth;
                break;
            case DataTypes.BiomeType.Poison:
                id = HintIds.Poison;
                break;
            case DataTypes.BiomeType.Shadow:
                id = HintIds.Shadow;
                break;
            case DataTypes.BiomeType.Growth:
                id = HintIds.Growth;
                break;
            default:
                break;
        }

        return id;
    }
}


[JsonConverter(typeof(StringEnumConverter))]
[System.Serializable]
public enum HintIds
    {
        BasicExplanation,
        SkillTree,
        SkillTreeSuggest,
        Boss,
        ActiveExplanation,
        Fire,
        Earth,
        Water,
        Poison,
        Shadow,
        Growth,
        AutoEnd,
        Coins,
        Skills,
        Random
    }