using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "EssenceIcons")]
public class EssenceIcons: ScriptableObject {

    public Sprite FireIcon;
    public Sprite EarthIcon;
    public Sprite WaterIcon;
    public Sprite GrowthIcon;
    public Sprite PoisonIcon;
    public Sprite ShadowIcon;

    public Sprite getIconFrom(DataTypes.BiomeType biome) {
        switch (biome)
        {
            case DataTypes.BiomeType.Fire:
                return FireIcon;
            case DataTypes.BiomeType.Earth:
                return EarthIcon;
            case DataTypes.BiomeType.Water:
                return WaterIcon;
            case DataTypes.BiomeType.Growth:
                return GrowthIcon;
            case DataTypes.BiomeType.Poison:
                return PoisonIcon;
            case DataTypes.BiomeType.Shadow:
                return ShadowIcon;
            default:
                return FireIcon;
        }
    }
    
}