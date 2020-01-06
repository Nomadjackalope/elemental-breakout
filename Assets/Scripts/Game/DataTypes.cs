using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class DataTypes {

    [JsonConverter(typeof(StringEnumConverter))]
    [System.Serializable]
    public enum HexType
    {
        Earth,
        Fire,
        Growth,
        Poison,
        Water,
        Shadow,
        Test,
        FireBiome,
        EarthBiome,
        WaterBiome,
        GrowthBiome,
        ShadowBiome,
        PoisonBiome,
        HighPointGrowth,
        Default
    }

    [JsonConverter(typeof(StringEnumConverter))]
    [System.Serializable]
    public enum BiomeType
    {
        Fire,
        Earth,
        Water,
        Growth,
        Poison,
        Shadow,
        Random,
        Default
    }

    public static HexType getHexEssenceFrom(BiomeType biome) {
        switch (biome)
        {
            case BiomeType.Fire:
                return HexType.Fire;
            case BiomeType.Earth:
                return HexType.Earth;
            case BiomeType.Water:
                return HexType.Water;
            case BiomeType.Growth:
                return HexType.Growth;
            case BiomeType.Shadow:
                return HexType.Shadow;
            case BiomeType.Poison:
                return HexType.Poison;
            
            default:
                return HexType.Default;
        }
    }

    public static HexType getHexFrom(BiomeType biome) {
        switch (biome)
        {
            case BiomeType.Fire:
                return HexType.FireBiome;
            case BiomeType.Earth:
                return HexType.EarthBiome;
            case BiomeType.Water:
                return HexType.WaterBiome;
            case BiomeType.Growth:
                return HexType.GrowthBiome;
            case BiomeType.Shadow:
                return HexType.ShadowBiome;
            case BiomeType.Poison:
                return HexType.PoisonBiome;
            
            default:
                return HexType.Default;
        }
    }

    public static BiomeType GetBiomeFrom(HexType hex) {
        switch (hex)
        {
            case HexType.Fire:
                return BiomeType.Fire;
            case HexType.FireBiome:
                return BiomeType.Fire;

            case HexType.Earth:
                return BiomeType.Earth;
            case HexType.EarthBiome:
                return BiomeType.Earth;

            case HexType.Water:
                return BiomeType.Water;
            case HexType.WaterBiome:
                return BiomeType.Water;

            case HexType.Growth:
                return BiomeType.Growth;
            case HexType.GrowthBiome:
                return BiomeType.Growth;

            case HexType.Shadow:
                return BiomeType.Shadow;
            case HexType.ShadowBiome:
                return BiomeType.Shadow;

            case HexType.Poison:
                return BiomeType.Poison;
            case HexType.PoisonBiome:
                return BiomeType.Poison;

            default:
                return BiomeType.Default;
        }
    }

    public static BiomeType GetBiomeFrom(string name) {
        switch (name)
        {
            case "Fire":
                return BiomeType.Fire;
            case "Earth":
                return BiomeType.Earth;
            case "Water":
                return BiomeType.Water;
            case "Growth":
                return BiomeType.Growth;
            case "Shadow":
                return BiomeType.Shadow;
            case "Poison":
                return BiomeType.Poison;
        }

        return BiomeType.Fire;
    }

    public static bool IsBiome(HexType hex) {
        return 
            hex == HexType.FireBiome ||
            hex == HexType.WaterBiome ||
            hex == HexType.EarthBiome ||
            hex == HexType.GrowthBiome ||
            hex == HexType.ShadowBiome ||
            hex == HexType.PoisonBiome;

    }

    public static bool IsElement(HexType hex) {
        return 
            hex == HexType.Fire ||
            hex == HexType.Water ||
            hex == HexType.Earth ||
            hex == HexType.Growth ||
            hex == HexType.Shadow ||
            hex == HexType.Poison;

    }

    public static Color GetColorFrom(DataTypes.BiomeType biome) {
        switch (biome)
        {
            case BiomeType.Fire:
                return Colors.FireRed;
            case BiomeType.Water:
                return Colors.WaterBlue;
            case BiomeType.Earth:
                return Colors.EarthBrown;
            case BiomeType.Growth:
                return Colors.GrowthGreen;
            case BiomeType.Shadow:
                return Colors.ShadowGrey;
            case BiomeType.Poison:
                return Colors.PoisonPink;
            default:
                return Color.white;
        }
    }

    public static Color GetBackgroundColorFrom(DataTypes.BiomeType biome) {
        switch (biome)
        {
            case BiomeType.Fire:
                return Colors.FireRedBackground;
            case BiomeType.Water:
                return Colors.WaterBlueBackground;
            case BiomeType.Earth:
                return Colors.EarthBrownBackground;
            case BiomeType.Growth:
                return Colors.GrowthGreenBackground;
            case BiomeType.Shadow:
                return Colors.ShadowGreyBackground;
            case BiomeType.Poison:
                return Colors.PoisonPinkBackground;
            default:
                return Color.white;
        }
    }

    public static Color GetAccentColorFrom(DataTypes.BiomeType biome) {
        switch (biome)
        {
            case BiomeType.Fire:
                return Colors.FireRedAccent;
            case BiomeType.Water:
                return Colors.WaterBlueAccent;
            case BiomeType.Earth:
                return Colors.EarthBrownAccent;
            case BiomeType.Growth:
                return Colors.GrowthGreenAccent;
            case BiomeType.Shadow:
                return Colors.ShadowGreyAccent;
            case BiomeType.Poison:
                return Colors.PoisonPinkAccent;
            default:
                return Color.white;
        }
    }

    public static Color GetButtonColorFrom(DataTypes.BiomeType biome) {
        switch (biome)
        {
            case BiomeType.Fire:
                return Colors.FireRedButton;
            case BiomeType.Water:
                return Colors.WaterBlueButton;
            case BiomeType.Earth:
                return Colors.EarthBrownButton;
            case BiomeType.Growth:
                return Colors.GrowthGreenButton;
            case BiomeType.Shadow:
                return Colors.ShadowGreyButton;
            case BiomeType.Poison:
                return Colors.PoisonPinkButton;
            default:
                return Color.white;
        }
    }

    public static Color GetEssenceColorFrom(DataTypes.BiomeType biome) {
        switch (biome)
        {
            case BiomeType.Shadow:
                return Color.white;
            default:
                return GetAccentColorFrom(biome);
        }
    }

}

public class Colors {
    public static Color FireRed = new Color(0.71f, 0.03f, 0.05f);
    public static Color WaterBlue = new Color(0.12f, 0.31f, 0.74f);
    public static Color ShadowGrey = new Color(0.10f, 0.12f, 0.12f);
    public static Color PoisonPink = new Color(0.71f, 0f, 0.55f);
    public static Color GrowthGreen = new Color(0.15f, 0.63f, 0.12f);
    public static Color EarthBrown = new Color(0.51f, 0.33f, 0.16f);

    public static Color FireRedBackground = new Color(0.102f, 0.102f, 0.102f);//new Color(0.376f, 0.376f, 0.376f);
    public static Color WaterBlueBackground = new Color(0.891f, 0.786f, 0.779f);
    public static Color ShadowGreyBackground = new Color(0.612f, 0.608f, 0.592f);
    public static Color PoisonPinkBackground = new Color(0.098f, 0.098f, 0.098f);//new Color(0.33f, 0.33f, 0.15f);
    public static Color GrowthGreenBackground = new Color(0.295f, 0.301f, 0.203f);
    public static Color EarthBrownBackground = new Color(0.443f, 0.393f, 0.328f);//new Color(0.51f, 0.33f, 0.16f);

    public static Color FireRedAccent = new Color(1f, 0.173f, 0f); //0.41
    public static Color WaterBlueAccent = new Color(0f, 0.22f, 0.588f);
    public static Color ShadowGreyAccent = new Color(0.082f, 0.078f, 0.11f);
    public static Color PoisonPinkAccent = new Color(0.75f, 0.32f, 0.75f);
    public static Color GrowthGreenAccent = new Color(0.624f, 0.918f, 0.188f);
    public static Color EarthBrownAccent = new Color(0.236f, 0.138f, 0.0f);//new Color(0.745f, 0.682f, 0.592f);

    public static Color FireRedButton = new Color(0.858f, 0.481f, 0.5f);
    public static Color WaterBlueButton = new Color(0.283f, 0.491f, 0.924f);
    public static Color ShadowGreyButton = new Color(0.16f, 0.16f, 0.16f);
    public static Color PoisonPinkButton = new Color(0.905f, 0.534f, 0.888f);
    public static Color GrowthGreenButton = new Color(0.537f, 0.773f, 0.522f);
    public static Color EarthBrownButton = new Color(0.83f, 0.76f, 0.63f);

    public static Color FireTextShadow = new Color(0.858f, 0.481f, 0.5f, 0.635f);
    public static Color WaterTextShadow = new Color(0.344f, 0.573f, 1f, 0.635f);
    public static Color ShadowTextShadow = new Color(0f, 0f, 0f, 0.635f);
    public static Color PoisonTextShadow = new Color(1f, 0f, 0.926f, 0.635f);
    public static Color GrowthTextShadow = new Color(0.057f, 0.858f, 0.012f, 0.635f);
    public static Color EarthTextShadow = new Color(1f, 0.572f, 0f, 0.635f);

    public static Color FireShadowColor = new Color(0.718f,  0.227f, 0.078f, 0.57f);
    public static Color PoisonShadowColor = new Color(0.576f, 0.184f, 0.569f, 0.57f);

    public static Color WoodBrown = new Color(0.470f, 0.267f, 0.063f);
}
