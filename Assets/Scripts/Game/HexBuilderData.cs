using UnityEngine;

[CreateAssetMenu(menuName = "Hex Builder Data")]
public class HexBuilderData : ScriptableObject {
    public Sprite sprite;
    public DataTypes.HexType hexType;
    public Sprite spriteHealth2;
    public Sprite spriteHealth3;
    public Sprite spriteHealth4;
    public Sprite spriteHealth5;
    public int pointValue = -1;
    public AudioClip onBreak;
}