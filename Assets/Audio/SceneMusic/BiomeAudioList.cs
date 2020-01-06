using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "BiomeAudioList")]
public class BiomeAudioList : ScriptableObject {
    public SceneAudio fireBiomeAudio;
    public SceneAudio earthBiomeAudio;
    public SceneAudio waterBiomeAudio;
    public SceneAudio growthBiomeAudio;
    public SceneAudio shadowBiomeAudio;
    public SceneAudio poisonBiomeAudio;
    public SceneAudio bossAudio;

    public SceneAudio GetAudioForBiome(DataTypes.BiomeType biome) {
        switch (biome)
        {
            case DataTypes.BiomeType.Fire:
                return fireBiomeAudio;
            case DataTypes.BiomeType.Water:
                return waterBiomeAudio;
            case DataTypes.BiomeType.Earth:
                return earthBiomeAudio;
            case DataTypes.BiomeType.Growth:
                return growthBiomeAudio;
            case DataTypes.BiomeType.Shadow:
                return shadowBiomeAudio;
            case DataTypes.BiomeType.Poison:
                return poisonBiomeAudio;
            default:
                return null;
        }
    }
}