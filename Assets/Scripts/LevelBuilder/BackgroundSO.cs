using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Background")]
public class BackgroundSO: ScriptableObject {
    public Sprite background;
    public DataTypes.BiomeType biome;
}