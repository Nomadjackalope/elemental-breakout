using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skill Data")]
public class Skill: ScriptableObject {

    public Sprite icon;
    public string skillName;
    public string description;
    public PowerUps powerUp;
    public DataTypes.BiomeType biome;
    [Range(1,3)]
    public int Row;
    public bool isAnActive = false;
    public CoolDownType whileCoolDown = CoolDownType.None; // vs xHits between activations
    public CoolDownType whileActive = CoolDownType.None; // vs xhits before deactivation

    public override string ToString() {
        return skillName + " " + powerUp + " has icon: " + (icon != null);
    }
}

public enum CoolDownType {
    TimeBased,
    XHits,
    None
}