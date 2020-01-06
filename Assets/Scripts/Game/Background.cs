
using UnityEngine;

public class Background : MonoBehaviour {
    public Sprite FireBackground;
    public Sprite WaterBackground;
    public Sprite EarthBackground;
    public Sprite GrowthBackground;
    public Sprite ShadowBackground;
    public Sprite PoisonBackground;


    public void setBiome(DataTypes.BiomeType biome) {
        GetComponent<SpriteRenderer>().sprite = getBackground(biome);
    }

    public Sprite getBackground(DataTypes.BiomeType biome) {
        switch (biome)
        {
            case DataTypes.BiomeType.Fire:
                return FireBackground;
            case DataTypes.BiomeType.Water:
                return WaterBackground;
            case DataTypes.BiomeType.Earth:
                return EarthBackground;
            case DataTypes.BiomeType.Growth:
                return GrowthBackground;
            case DataTypes.BiomeType.Shadow:
                return ShadowBackground;
            case DataTypes.BiomeType.Poison:
                return PoisonBackground;
            default:
            break;
        }

        return FireBackground;
    }
}